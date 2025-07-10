using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace infCore.Server
{
    public class healthUpdater : BaseScript
    {
        public healthUpdater() 
        {
            Task.Run(() => { updateHealthTask(); });
        }

        private async Task updateHealthTask()
        {
            while (true)
            {
                foreach (var kvp in userCache.usersHealth)
                {
                    int userId = kvp.Key;
                    var health = kvp.Value;

                    health.hunger = Math.Max(0, health.hunger - config.hungerDecreaseRate);
                    health.thirst = Math.Max(0, health.thirst - config.thirstDecreaseRate);
                }

                await BaseScript.Delay(60000);
            }
        }
    }
}
