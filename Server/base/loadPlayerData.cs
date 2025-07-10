using CitizenFX.Core;
using CitizenFX.Core.Native;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace infCore.Server
{
    public class loadPlayerData : BaseScript
    {
        public loadPlayerData()
        {

        }

        //Player Loading Into Server
        [EventHandler("playerConnecting")]
        public  void onPlayerConnecting(string playerName, dynamic kickReason, dynamic defferals, [FromSource] Player source)
        {
            using (var conn = getData.getConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int userId = -1;
                        bool isBanned = false;

                        defferals.presentCard(serverData.createLoadingCard("You are connecting to the server..."));

                        // Check if user exists by any identifier
                        var identifiersList = source.Identifiers.ToList();
                        var parameters = string.Join(",", identifiersList.Select((_, i) => $"@identifier{i}"));
                        var query = $"SELECT user_id, banned FROM user_identifiers WHERE identifier IN ({parameters}) LIMIT 1";

                        using (var cmd = new MySqlCommand(query, conn, transaction))
                        {
                            for (int i = 0; i < identifiersList.Count; i++)
                            {
                                cmd.Parameters.AddWithValue($"@identifier{i}", identifiersList[i]);
                            }

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    userId = reader.GetInt32("user_id");
                                    isBanned = reader.GetBoolean("banned");
                                }
                            }
                        }

                        // Create new user atomically if doesn't exist
                        if (userId == -1)
                        {
                            // Use INSERT with SELECT to get next ID atomically
                            string firstIdentifier = source.Identifiers.First();
                            using (var cmd = new MySqlCommand(@"
                        INSERT INTO user_identifiers (identifier, user_id, banned) 
                        SELECT @firstIdentifier, COALESCE(MAX(user_id), 0) + 1, 0 
                        FROM user_identifiers", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@firstIdentifier", firstIdentifier);
                                cmd.ExecuteNonQuery();
                            }

                            // Get the user_id we just created
                            using (var cmd = new MySqlCommand("SELECT user_id FROM user_identifiers WHERE identifier = @firstIdentifier", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@firstIdentifier", firstIdentifier);
                                userId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Insert remaining identifiers
                            foreach (string identifier in source.Identifiers.Skip(1))
                            {
                                using (var cmd = new MySqlCommand("INSERT INTO user_identifiers (identifier, user_id, banned) VALUES (@identifier, @userId, 0)", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@identifier", identifier);
                                    cmd.Parameters.AddWithValue("@userId", userId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Create user data for new users
                            using (var cmd = new MySqlCommand("INSERT INTO user_data (user_id) VALUES (@user_id)", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@user_id", userId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // User exists, update/insert missing identifiers
                            foreach (string identifier in source.Identifiers)
                            {
                                using (var cmd = new MySqlCommand("INSERT INTO user_identifiers (identifier, user_id, banned) VALUES (@identifier, @userId, @banned) ON DUPLICATE KEY UPDATE banned = @banned", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@identifier", identifier);
                                    cmd.Parameters.AddWithValue("@userId", userId);
                                    cmd.Parameters.AddWithValue("@banned", isBanned ? 1 : 0);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        Debug.WriteLine($"^6[infCore]^7Player {playerName} ({userId}) authenticated successfully.");
                        transaction.Commit();

                        if (isBanned)
                        {
                            Debug.WriteLine($"^1[infCore]^7Player {playerName} ({userId}) is banned from the server.");
                            defferals.presentCard(serverData.createLoadingCard("You are banned from this server!"));
                            return;
                        }
                        defferals.done();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"^1[infCore]^7Error loading player data: {ex.Message}");
                        defferals.presentCard(serverData.createLoadingCard("Error on authentication please try again!"));
                    }
                }
            }
        }

        [EventHandler("playerJoining")]
        public void onPlayerJoining([FromSource] Player source)
        {
            addOnlinePlayer(source);
        }

        [EventHandler("onResourceStart")]
        public void onResourceStart(string resourceName)
        {
            if (resourceName == API.GetCurrentResourceName())
            {
                addOnlinePlayers();
            }
        }

        [EventHandler("playerDropped")]
        public void onPlayerLeaving([FromSource] Player source, string reason)
        {
            Debug.WriteLine($"^6[infCore]^7Player {source.Name} ({userCache.userIds[source.Handle]}) has left the server. Reason: {reason}");
            removeOnlinePlayer(source);
        }

        public void addOnlinePlayers()
        {
            PlayerList players = Players;
            foreach (Player player in players)
            {
                if (player.Handle != null && player.Handle != "")
                {
                    addOnlinePlayer(player);
                }
            }
        }

        public void addOnlinePlayer([FromSource] Player source)
        {
            try
            {
                int userId = -1;
                using (var conn = getData.getConnection())
                {
                    conn.Open();
                    //Search for user ID
                    var identifiersList = source.Identifiers.ToList();
                    var parameters = string.Join(",", identifiersList.Select((_, i) => $"@identifier{i}"));
                    var query = $"SELECT user_id FROM user_identifiers WHERE identifier IN ({parameters}) LIMIT 1";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        for (int i = 0; i < identifiersList.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@identifier{i}", identifiersList[i]);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId = reader.GetInt32("user_id");
                            }
                        }
                    }
                    //Found user ID add the user
                    Debug.WriteLine($"^6[infCore]^7Player {source.Name} ({userId}) is joining the server.");
                    userCache.usersSources.TryAdd(userId, source.Handle);
                    userCache.userIds.TryAdd(source.Handle, userId);
                    using (var cmd = new MySqlCommand("SELECT * FROM user_data WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userCache.userMoneyClass userMoney = JsonConvert.DeserializeObject<userCache.userMoneyClass>(reader["money"].ToString());
                                userCache.usersMoney.TryAdd(userId, userMoney);

                                userCache.userFactionClass userFaction = JsonConvert.DeserializeObject<userCache.userFactionClass>(reader["faction"].ToString());
                                userCache.userFactions.TryAdd(userId, userFaction);

                                List<string> userGroups = JsonConvert.DeserializeObject<List<string>>(reader["groups"].ToString());
                                userCache.userGroups.TryAdd(userId, userGroups);

                                userCache.userAdminlevel.TryAdd(userId, reader.GetInt32("admin_level"));

                                userCache.userHealthClass userHealth = JsonConvert.DeserializeObject<userCache.userHealthClass>(reader["health"].ToString());
                                userCache.usersHealth.TryAdd(userId, userHealth);

                                Vector3 position = JsonConvert.DeserializeObject<Vector3>(reader["position"].ToString());
                                userCache.userLocations.TryAdd(userId, position);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding online player: {ex.Message}");
            }
        }

        [EventHandler("infCore:spawnPlayer")]
        public void onSpawnPlayer([FromSource] Player source, bool isFirstSpawn)
        {
            if (!isFirstSpawn)
                return;

            int userId = userCache.userIds[source.Handle];
            Vector3 position = userCache.userLocations[userId];
            API.SetEntityCoords(API.GetPlayerPed(source.Handle), position.X, position.Y, position.Z, false, false, false, true);
        }


        public void removeOnlinePlayer([FromSource] Player source)
        {
            try
            {
                if (userCache.userIds.TryGetValue(source.Handle, out int userId))
                {
                    using (var conn = getData.getConnection())
                    {
                        conn.Open();

                        var position = API.GetEntityCoords(API.GetPlayerPed(source.Handle));
                        Vector3 spawnPosition = new Vector3(position.X, position.Y, position.Z+0.5f);
                        var money = userCache.usersMoney.TryGetValue(userId, out var m) ? m : new userCache.userMoneyClass();
                        var faction = userCache.userFactions.TryGetValue(userId, out var f) ? f : new userCache.userFactionClass();
                        var groups = userCache.userGroups.TryGetValue(userId, out var g) ? g : new List<string>();
                        var adminLevel = userCache.userAdminlevel.TryGetValue(userId, out var a) ? a : 0;
                        var health = userCache.usersHealth.TryGetValue(userId, out var h) ? h : new userCache.userHealthClass();

                        using (var cmd = new MySqlCommand(@"
                            UPDATE user_data 
                            SET 
                                position = @position,
                                money = @money,
                                faction = @faction,
                                groups = @groups,
                                admin_level = @adminLevel,
                                health = @health 
                            WHERE user_id = @userId", conn))
                        {
                            cmd.Parameters.AddWithValue("@position", JsonConvert.SerializeObject(position));
                            cmd.Parameters.AddWithValue("@money", JsonConvert.SerializeObject(money));
                            cmd.Parameters.AddWithValue("@faction", JsonConvert.SerializeObject(faction));
                            cmd.Parameters.AddWithValue("@groups", JsonConvert.SerializeObject(groups));
                            cmd.Parameters.AddWithValue("@adminLevel", adminLevel);
                            cmd.Parameters.AddWithValue("@health", JsonConvert.SerializeObject(health));
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    userCache.usersSources.TryRemove(userId, out _);
                    userCache.userIds.TryRemove(source.Handle, out _);
                    userCache.usersMoney.TryRemove(userId, out _);
                    userCache.userFactions.TryRemove(userId, out _);
                    userCache.userGroups.TryRemove(userId, out _);
                    userCache.userAdminlevel.TryRemove(userId, out _);
                    userCache.usersHealth.TryRemove(userId, out _);
                    userCache.userLocations.TryRemove(userId, out _);

                    Debug.WriteLine($"^6[infCore]^7Player {source.Name} ({userId}) left and was saved to the DB.");
                }
                else
                {
                    Debug.WriteLine($"^6[infCore]^7Player {source.Name} left but wasn't found in cache.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"^6[infCore]^7Error removing player: {ex.Message}");
            }
        }

    }
}
