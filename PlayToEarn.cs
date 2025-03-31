extern alias UnityEngineCoreModule;

using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityCoreModule = UnityEngineCoreModule.UnityEngine;

namespace PlayToEarn
{
    public class PlayToEarnPlugin : RocketPlugin<PlayToEarnConfiguration>
    {
        public static PlayToEarnPlugin instance;
        private PlayToEarnTickrate tickrate;
        public DatabaseMgr Database;
        public override void LoadPlugin()
        {
            Database = new(this);
            base.LoadPlugin();
            // Instanciating events
            Rocket.Unturned.U.Events.OnPlayerConnected += OnPlayerConnected;
            Rocket.Unturned.U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerUpdateStat += OnPlayerStatsUpdate;
            if (Configuration.Instance.TickrateRewardEarnPerTime > 0)
                tickrate = gameObject.AddComponent<PlayToEarnTickrate>();
            instance = this;

            Logger.Log("PlayToEarn instanciated, by GxsperMain");
        }

        private void OnPlayerStatsUpdate(UnturnedPlayer player, EPlayerStat stat)
        {
            switch (stat)
            {
                case EPlayerStat.KILLS_PLAYERS:
                    if (Configuration.Instance.KillPlayersReward != "0")
                    {
                        if (Configuration.Instance.KillPlayerNotify)
                        {
                            int[] colors = Configuration.Instance.KillPlayerRGBColorNotify.Split(',').Select(int.Parse).ToArray();
                            ChatManager.serverSendMessage(
                                Translate("killed_player", Configuration.Instance.KillPlayersReward),
                                new UnityCoreModule.Color(colors[0], colors[1], colors[2]),
                                null,
                                player.SteamPlayer(),
                                EChatMode.SAY,
                                Configuration.Instance.ChatIconURL,
                                true
                            );
                        }
                        Database.IncrementWallet(player.Id, Configuration.Instance.KillPlayersReward);
                    }
                    break;
                case EPlayerStat.KILLS_ZOMBIES_NORMAL:
                    if (Configuration.Instance.KillZombieReward != "0")
                    {
                        if (Configuration.Instance.KillZombieNotify)
                        {
                            int[] colors = [.. Configuration.Instance.KillZombieRGBColorNotify.Split(',').Select(int.Parse)];
                            ChatManager.serverSendMessage(
                                Translate("killed_zombie", Utils.FormatCoinToHumanReadable(Configuration.Instance.KillZombieReward)),
                                new UnityCoreModule.Color(colors[0], colors[1], colors[2]),
                                null,
                                player.SteamPlayer(),
                                EChatMode.SAY,
                                Configuration.Instance.ChatIconURL,
                                true
                            );
                        }

                        Database.IncrementWallet(player.Id, Configuration.Instance.KillZombieReward);
                    }
                    break;
                case EPlayerStat.KILLS_ZOMBIES_MEGA:
                    if (Configuration.Instance.KillMegaZombieReward != "0")
                    {
                        if (Configuration.Instance.KillMegaZombieNotify)
                        {
                            int[] colors = [.. Configuration.Instance.KillMegaRGBColorNotify.Split(',').Select(int.Parse)];
                            ChatManager.serverSendMessage(
                                Translate("killed_mega_zombie", Utils.FormatCoinToHumanReadable(Configuration.Instance.KillMegaZombieReward)),
                                new UnityCoreModule.Color(colors[0], colors[1], colors[2]),
                                null,
                                player.SteamPlayer(),
                                EChatMode.SAY,
                                Configuration.Instance.ChatIconURL,
                                true
                            );
                        }
                        Database.IncrementWallet(player.Id, Configuration.Instance.KillMegaZombieReward);
                    }
                    break;
            }
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (Configuration.Instance.ItemCollectReward != "0")
                player.Events.OnInventoryAdded += ItemReceived;

            Database.AddNewPlayer(player.Id);

            if (tickrate != null)
            {
                if (tickrate.Contains(player))
                {
                    player.Kick("Already playing");
                    return;
                }
                tickrate.AddPlayer(player);
            }
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (Configuration.Instance.ItemCollectReward == "0")
                player.Events.OnInventoryAdded -= ItemReceived;
            tickrate?.RemovePlayer(player);
        }

        private void ItemReceived(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar itemJar)
        {
            if (Configuration.Instance.ItemCollectId == itemJar.item.id)
            {
                player.Inventory.removeItem((byte)inventoryGroup, inventoryIndex);
                Database.IncrementWallet(player.Id, Configuration.Instance.ItemCollectReward);

                if (Configuration.Instance.NotifyItemCollectReward)
                {
                    int[] colors = [.. Configuration.Instance.ItemCollectRGBColorNotify.Split(',').Select(int.Parse)];
                    ChatManager.serverSendMessage(
                        Translate("balance_earned_itemcollect", Utils.FormatCoinToHumanReadable(Configuration.Instance.ItemCollectReward)),
                        new UnityCoreModule.Color(colors[0], colors[1], colors[2]),
                        null,
                        player.SteamPlayer(),
                        EChatMode.SAY,
                        Configuration.Instance.ChatIconURL,
                        true
                    );
                }
            }
        }

        public override TranslationList DefaultTranslations => new()
        {
            {"killed_player", "Player killed, you earned: {0} PTE"},
            {"killed_zombie", "Zombie killed, you earned: {0} PTE"},
            {"killed_mega_zombie", "Mega Zombie killed, you earned: {0} PTE"},
            {"balance_command", "Your current balance is {0}" },
            {"wallet_command", "Wallet set!" },
            {"balance_earned_time", "You earned {0} PTE for playing" },
            {"balance_earned_itemcollect", "You earned {0} PTE for collecting" },
        };
    }

    class PlayToEarnTickrate : MonoBehaviour
    {
        readonly List<UnturnedPlayer> playersToEarnPoints = [];
        uint actualTick = 0;

        public void Start()
        {
            actualTick = PlayToEarnPlugin.instance.Configuration.Instance.TickrateRewardEarnPerTime;
        }

        public void Update()
        {
            if (actualTick <= 0)
            {
                actualTick = PlayToEarnPlugin.instance.Configuration.Instance.TickrateRewardEarnPerTime;
                foreach (UnturnedPlayer player in playersToEarnPoints)
                {
                    // Add points into database
                    PlayToEarnPlugin.instance.Database.IncrementWallet(player.Id, PlayToEarnPlugin.instance.Configuration.Instance.RewardEarnPerTime);
                    int[] colors = [.. PlayToEarnPlugin.instance.Configuration.Instance.RewardEarnRGBColorNotify.Split(',').Select(int.Parse)];
                    // Notify points earned
                    if (PlayToEarnPlugin.instance.Configuration.Instance.NotifyRewardEarnPerTime)
                        ChatManager.serverSendMessage(
                            PlayToEarnPlugin.instance.Translate("balance_earned_time", Utils.FormatCoinToHumanReadable(PlayToEarnPlugin.instance.Configuration.Instance.RewardEarnPerTime)),
                            new UnityCoreModule.Color(colors[0], colors[1], colors[2]),
                            null,
                            player.SteamPlayer(),
                            EChatMode.SAY,
                            PlayToEarnPlugin.instance.Configuration.Instance.ChatIconURL,
                            true
                        );
                }
            }
            actualTick--;
        }

        public void AddPlayer(UnturnedPlayer player) => playersToEarnPoints.Add(player);

        public void RemovePlayer(UnturnedPlayer player) => playersToEarnPoints.Remove(player);

        public bool Contains(UnturnedPlayer player) => playersToEarnPoints.Contains(player);
    }
}

class Utils
{
    public static string FormatCoinToHumanReadable(object quantity)
    {
        string quantityString = quantity.ToString();

        if (quantityString.Length <= 17)
            return "0.00";
        else
            quantityString = quantityString.Substring(0, quantityString.Length - 17);

        if (quantityString.Length == 1)
            return $"0.0{quantityString}";
        if (quantityString.Length == 2)
            return $"0.{quantityString}";
        else
            return quantityString.Substring(0, quantityString.Length - 2) + "." + quantityString.Substring(quantityString.Length - 2);
    }
}