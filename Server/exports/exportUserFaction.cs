using CitizenFX.Core;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace infCore.Server
{
    public class exportUserFaction : BaseScript
    {
        public exportUserFaction()
        {
            Exports.Add("getUserFaction", new Func<int, string>(getUserFactionName));
            Exports.Add("getUserFactionRank", new Func<int, int>(getUserFactionRank));
            Exports.Add("hasUserFaction", new Func<int, string, bool>(hasUserFaction));

            Exports.Add("setUserFaction", new Action<int, string>(setUserFaction));
            Exports.Add("setUserFactionRank", new Action<int, int>(setUserFactionRank));

            Exports.Add("getUsersByFaction", new Func<string, List<int>>(getUsersByFaction));
            Exports.Add("getDefaultFactionName", new Func<string>(getDefaultFactionName));
        }

        public string getUserFactionName(int userId)
        {
            try {
                if (userCache.userFactions.TryGetValue(userId, out userCache.userFactionClass faction))
                    return faction.name;

                using (var conn = getData.getConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT faction FROM user_data WHERE user_id = @user_id", conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userCache.userFactionClass userFaction = JsonConvert.DeserializeObject<userCache.userFactionClass>(reader["faction"].ToString());
                                return userFaction.name;
                            }
                            else
                                return config.defaultFactionName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in getUserFactionName: {ex.Message}");
                return config.defaultFactionName;
            }
        }

        public int getUserFactionRank(int userId)
        {
            try
            {
                if (userCache.userFactions.TryGetValue(userId, out userCache.userFactionClass faction))
                    return faction.rank;

                using (var conn = getData.getConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT faction FROM user_data WHERE user_id = @user_id", conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userCache.userFactionClass userFaction = JsonConvert.DeserializeObject<userCache.userFactionClass>(reader["faction"].ToString());
                                return userFaction.rank;
                            }
                            else
                                return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in getUserFactionRank: {ex.Message}");
                return 0;
            }
        }
        public bool hasUserFaction(int userId, string factionName)
        {
            if (userCache.userFactions.TryGetValue(userId, out userCache.userFactionClass faction) && faction.name == factionName)
                return true;
            using (var conn = getData.getConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT faction FROM user_data WHERE user_id = @user_id", conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userCache.userFactionClass userFaction = JsonConvert.DeserializeObject<userCache.userFactionClass>(reader["faction"].ToString());
                            return userFaction.name == factionName;
                        }
                    }
                }
            }

            return false;
        }

        public void setUserFaction(int userId, string factionName)
        {
            if (userCache.userFactions.TryGetValue(userId, out userCache.userFactionClass faction))
                faction.name = factionName;
        }
        public void setUserFactionRank(int userId, int rank)
        {
            if (userCache.userFactions.TryGetValue(userId, out userCache.userFactionClass faction))
                faction.rank = rank;
        }


        public List<int> getUsersByFaction(string factionName)
        {
            try {
                using (var conn = getData.getConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT user_id FROM user_data WHERE JSON_EXTRACT(faction, '$.name') = @factionName", conn))
                    {
                        cmd.Parameters.AddWithValue("@factionName", factionName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            List<int> userIds = new List<int>();
                            while (reader.Read())
                                userIds.Add(reader.GetInt32("user_id"));

                            return userIds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in getUsersByFaction: {ex.Message}");
                return new List<int>();
            }
        }

        public string getDefaultFactionName()
        {
            return config.defaultFactionName;
        }
    }
}
