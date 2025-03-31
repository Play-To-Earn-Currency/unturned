using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace PlayToEarn.Commands
{
    public class Scoreboard : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "wallet";

        public string Help => "Setup your wallet";

        public string Syntax => "/wallet";

        public List<string> Aliases => new();

        public List<string> Permissions => new();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PlayToEarnPlugin.instance.Database.UpdateWallet(caller.Id, command[0]);            
            UnturnedChat.Say(caller, PlayToEarnPlugin.instance.Translate("wallet_command"));
        }
    }
}
