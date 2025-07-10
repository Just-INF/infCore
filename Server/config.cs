using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace infCore.Server
{
    public class config
    {
        public static string infCoreVersion = "1.0.0";

        // Default Configuration
        public static userCache.userMoneyClass defaultMoney = new userCache.userMoneyClass
        {
            money = 0,
            bank = 10000,
            infCoins = 0
        };

        public static List<string> defaultGroups = new List<string> { "user" };
        public static string defaultFactionName = "Civilian";

        public static Vector3 defaultSpawnPosition = new Vector3(0, 0, 0); // Default spawn position for players

        public static int hungerDecreaseRate = 1; // Hunger decrease rate per 60 seconds
        public static int thirstDecreaseRate = 2; // Thirst decrease rate per 60 seconds
    }
}
