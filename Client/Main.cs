using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client
{
    public class Main : BaseScript
    {
        public Main()
        {
            try
            {
                config = JsonConvert.DeserializeObject<ConfigModel>(LoadResourceFile(GetCurrentResourceName(), "config.json"));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Parsing While Config File: " + e.Message);
            }
            StartChecking();
        }
        private static ConfigModel config = new ConfigModel();
        private static List<int> removedNpc = new List<int>();
        private async Task OnTick()
        {
            await Delay(1000);
            var ped = Game.PlayerPed.Position;
            List<int> npcList = ConvertDynamicToInt(GetGamePool("CPed"));
            if (npcList.Count > 0)
            {
                foreach (var id in npcList)
                {

                    //HasEntityBeenDamagedByEntity((int)id, PlayerPedId(), true)

                    if (IsPedDeadOrDying(id, true) && GetPedSourceOfDeath(id) == GetPlayerPed(-1) && !removedNpc.Contains(id))
                    {
                        if (config.OnlyHumanoid && IsPedHuman(id))
                            TriggerServerEvent(config.ServerEvetName, config.TaskId, config.TaskCount);
                        else if (!config.OnlyHumanoid && !IsPedHuman(id))
                            TriggerServerEvent(config.ServerEvetName, config.TaskId, config.TaskCount);
                        removedNpc.Add(id);
                    }
                }
            }

        }

        private void StartChecking()
        {
            Tick += OnTick;
        }
        private List<int> ConvertDynamicToInt(dynamic data)
        {
            var list = new List<int>();
            if (data == null) return list;
            try
            {
                string test = string.Join(",", data);
                return test.Split(',')?.Select(Int32.Parse)?.ToList();

            }
            catch (Exception e)
            {
                return list;
            }
            return list;
        }
    }
}
