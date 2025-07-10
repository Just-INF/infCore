using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace infCore.Server.exports
{
    public class exportUserHealth : BaseScript
    {
        public exportUserHealth()
        {
            Exports.Add("getUserHunger", new Func<int, int>(getUserHunger));
            Exports.Add("getUserThirst", new Func<int, int>(getUserThirst));
            Exports.Add("getUserStress", new Func<int, int>(getUserStress));

            Exports.Add("setUserHunger", new Action<int, int>(setUserHunger));
            Exports.Add("setUserThirst", new Action<int, int>(setUserThirst));
            Exports.Add("setUserStress", new Action<int, int>(setUserStress));
        }
        public int getUserHunger(int userId)
        {
            if (userCache.usersHealth.TryGetValue(userId, out userCache.userHealthClass health))
                return health.hunger;

            return 100; // Default value
        }
        public int getUserThirst(int userId)
        {
            if (userCache.usersHealth.TryGetValue(userId, out userCache.userHealthClass health))
                return health.thirst;

            return 100; // Default value
        }
        public int getUserStress(int userId)
        {
            if (userCache.usersHealth.TryGetValue(userId, out userCache.userHealthClass health))
                return health.stress;

            return 0; // Default value
        }

        //Set
        public void setUserHunger(int userId, int hunger)
        {
            if (userCache.usersHealth.TryGetValue(userId, out userCache.userHealthClass health))
                health.hunger = hunger;
            else
                userCache.usersHealth[userId] = new userCache.userHealthClass { hunger = hunger };
        }
        public void setUserThirst(int userId, int thirst)
        {
            if (userCache.usersHealth.TryGetValue(userId, out userCache.userHealthClass health))
                health.thirst = thirst;
            else
                userCache.usersHealth[userId] = new userCache.userHealthClass { thirst = thirst };
        }
        public void setUserStress(int userId, int stress)
        {
            if (userCache.usersHealth.TryGetValue(userId, out userCache.userHealthClass health))
                health.stress = stress;
            else
                userCache.usersHealth[userId] = new userCache.userHealthClass { stress = stress };
        }
    }
}
