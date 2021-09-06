using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Simple911SystemServer
{
    public class Class1 : BaseScript
    {

        public List<int> OnDuty = new List<int>();

        public Class1()
        {
            EventHandlers["onResourceStart"] += new Action<string>(OnResourceStart);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
        }

        public void OnResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            Debug.WriteLine("Simple 911 Script by Popcornography started successfully.");

            RegisterCommand("onduty", new Action<int, List<object>, string>((source, args, raw) =>
            {
                bool isInList = OnDuty.IndexOf(source) != -1;
                if (!isInList)
                {
                    OnDuty.Add(source);
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new[] { 16, 43, 76 },
                        args = new[] { "OnDuty", $"You are now On Duty!" }
                    });
                } else
                {
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new[] { 16, 43, 76 },
                        args = new[] { "OnDuty", $"You are already On Duty!" }
                    });
                }
            }), false);

            RegisterCommand("offduty", new Action<int, List<object>, string>((source, args, raw) =>
            {
                bool isInList = OnDuty.IndexOf(source) != -1;
                if (isInList)
                {
                    OnDuty.Remove(source);
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new[] { 16, 43, 76 },
                        args = new[] { "OffDuty", $"You are now Off Duty!" }
                    });
                }
                else
                {
                    OnDuty.Remove(source);
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new[] { 16, 43, 76 },
                        args = new[] { "OffDuty", $"You were already Off Duty!" }
                    });
                }
            }), false);

            RegisterCommand("leocount", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerClientEvent("chat:addMessage", new
                {
                    color = new[] { 16, 43, 76 },
                    args = new[] { "OnDuty", $"There are { OnDuty.Count } Units on duty!" }
                });
            }), false);

            RegisterCommand("911", new Action<int, List<object>, string>((source, args, raw) =>
            {
                string calldetails = null;
                if (args.Count > 0)
                {
                    calldetails = args[0].ToString();
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new[] { 16, 43, 76 },
                        args = new[] { "911", $"911 Call Recieved!" }
                    });

                    /*string player = source.ToString();
                    int ped = GetPlayerPed(player);
                    Vector3 playercoords = GetEntityCoords(ped);
                    float posx = playercoords.X;
                    float posy = playercoords.Y;
                    float posz = playercoords.Z;*/

                    //Loop through each person on duty, send them the details of the call
                    OnDuty.ForEach((officer) => Players[officer].TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 16, 43, 76 },
                        args = new[] { "911", $"{ calldetails }" }
                    }));
                } else
                {
                    TriggerClientEvent("chat:addMessage", new
                    {
                        color = new[] { 16, 43, 76 },
                        args = new[] { "911", $"You need to specifcy the details of the 911 call!" }
                    });
                }
            }), false);
        }

        public void OnPlayerDropped([FromSource] Player player, string reason)
        {
            int playerid = Int32.Parse(player.Handle);
            bool isInList = OnDuty.IndexOf(playerid) != -1;
            if (isInList)
            {
                OnDuty.Remove(playerid);
            }
        }
    }
}
