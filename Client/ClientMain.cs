using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace infCore.Client
{
    public class ClientMain : BaseScript
    {
        bool isFirstSpawn = true;
        public ClientMain()
        {
            
        }

        [EventHandler("playerSpawned")]
        private void OnPlayerSpawned()
        {
            TriggerServerEvent("infCore:spawnPlayer", isFirstSpawn);
            isFirstSpawn = false;
            ExecuteCommand("clear");
        }
    } 
}