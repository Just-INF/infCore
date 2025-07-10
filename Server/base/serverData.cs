using Newtonsoft.Json;

namespace infCore.Server
{
    public class serverData
    {
        public static string user_identifiers = @"CREATE TABLE IF NOT EXISTS user_identifiers (
                                                    identifier VARCHAR(255) PRIMARY KEY,
                                                    user_id INT NOT NULL,
                                                    banned TINYINT(1) NOT NULL DEFAULT 0
                                                );";

        public static string user_data = $@"CREATE TABLE IF NOT EXISTS user_data (
                                            user_id INT NOT NULL PRIMARY KEY,
                                            money JSON NOT NULL DEFAULT ('{JsonConvert.SerializeObject(config.defaultMoney)}'),
                                            groups JSON NOT NULL DEFAULT ('{JsonConvert.SerializeObject(config.defaultGroups)}'),
                                            faction JSON NOT NULL DEFAULT ('{JsonConvert.SerializeObject(new userCache.userFactionClass())}'),
                                            admin_level INT NOT NULL DEFAULT 0, 
                                            health JSON NOT NULL DEFAULT ('{JsonConvert.SerializeObject(new userCache.userHealthClass())}'),
                                            position JSON NOT NULL DEFAULT ('{JsonConvert.SerializeObject(config.defaultSpawnPosition)}')  
                                        );";

        public static string createLoadingCard(string messageToDisplay)
        {
            return @"{
                        ""type"": ""AdaptiveCard"",
                        ""$schema"": ""https://adaptivecards.io/schemas/adaptive-card.json"",
                        ""version"": ""1.5"",
                        ""body"": [
                            {
                                ""type"": ""Image"",
                                ""url"": ""https://i.imgur.com/HF9X29q.png"",
                                ""altText"": ""Connecting to FiveM Server"",
                                ""size"": ""Stretch""
                            },
                            {
                                ""type"": ""TextBlock"",
                                ""text"": ""💜Asteapta putin pana te conectam pe server💜"",
                                ""wrap"": true,
                                ""weight"": ""Bolder"",
                                ""size"": ""Large"",
                                ""horizontalAlignment"": ""Center"",
                                ""spacing"": ""Medium""
                            },
                            {
                                ""type"": ""TextBlock"",
                                ""text"": "+messageToDisplay+@",
                                ""wrap"": true,
                                ""horizontalAlignment"": ""Center"",
                                ""spacing"": ""Small""
                            },
                            {
                                ""type"": ""ProgressRing"",
                                ""horizontalAlignment"": ""Center""
                            }
                        ]
                    }";
        }
    }
}
