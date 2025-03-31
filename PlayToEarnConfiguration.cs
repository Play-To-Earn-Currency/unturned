using Rocket.API;
using System.Collections.Generic;

namespace PlayToEarn
{
    public class PlayToEarnConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress = "127.0.0.1";
        public string DatabaseName = "pte_wallets";
        public string DatabaseUsername = "pte_admin";
        public string DatabasePassword = "supersecretpasswrd";
        public int DatabasePort = 3306;
        public string PlayToEarnTableName = "unturned";
        public string ChatIconURL = "https://add-image-url.com";
        public string RewardEarnPerTime = "100000000000000000";
        public string RewardEarnRGBColorNotify = "0,255,0";
        public bool NotifyRewardEarnPerTime = true;
        public uint TickrateRewardEarnPerTime = 36000;
        public string KillZombieReward = "1000000000000000";
        public bool KillZombieNotify = false;
        public string KillZombieRGBColorNotify = "0,255,0";
        public string KillMegaZombieReward = "300000000000000000";
        public bool KillMegaZombieNotify = true;
        public string KillMegaRGBColorNotify = "0,255,0";
        public bool NotifyItemCollectReward = true;
        public string ItemCollectReward = "300000000000000000";
        public string ItemCollectRGBColorNotify = "0,255,0";
        public ushort ItemCollectId = 10;
        public string KillPlayersReward = "0";
        public bool KillPlayerNotify = true;
        public string KillPlayerRGBColorNotify = "255,0,0";

        public void LoadDefaults()
        {
        }
    }
}
