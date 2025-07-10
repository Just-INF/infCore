using CitizenFX.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace infCore.Server
{
    public class userCache
    {
        //User Identifiers
        public static ConcurrentDictionary<int, string> usersSources = new ConcurrentDictionary<int, string>(); // UserID (int) -> Source (string)
        public static ConcurrentDictionary<string, int> userIds = new ConcurrentDictionary<string, int>(); // Source (string) -> UserID (int)

        //User money
        public class userMoneyClass
        {
            public int money { get; set; }
            public int bank { get; set; }
            public int infCoins { get; set; }
        }
        public static ConcurrentDictionary<int, userMoneyClass> usersMoney = new ConcurrentDictionary<int, userMoneyClass>(); // UserID (int) -> UserMoney (userMoney)

        //User Groups
        public static ConcurrentDictionary<int, List<string>> userGroups = new ConcurrentDictionary<int, List<string>>(); // UserID (int) -> Groups (List<string>)

        //User Factions
        public class userFactionClass
        {
            public string name { get; set; } = config.defaultFactionName;
            public int rank { get; set; } = 0;
        }
        public static ConcurrentDictionary<int, userFactionClass> userFactions = new ConcurrentDictionary<int, userFactionClass>(); // UserID (int) -> UserFaction (userFaction)

        //User Admin Level
        public static ConcurrentDictionary<int, int> userAdminlevel = new ConcurrentDictionary<int, int>(); // UserID (int) -> Admin Level (int)

        //Hunger Thirst Stress
        public class userHealthClass
        {
            public int hunger { get; set; } = 100;
            public int thirst { get; set; } = 100;
            public int stress { get; set; } = 0;
        }
        public static ConcurrentDictionary<int, userHealthClass> usersHealth = new ConcurrentDictionary<int, userHealthClass>(); // UserID (int) -> UserHealth (userHealth)
    
        //User Location
        public static ConcurrentDictionary<int, Vector3> userLocations = new ConcurrentDictionary<int, Vector3>(); // UserID (int) -> Location (Vector3)
    }
}
