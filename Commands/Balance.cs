using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace PlayToEarn.Commands
{
    public class Rank : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "balance";

        public string Help => "Show your current balance";

        public string Syntax => "/balance";

        public List<string> Aliases => new();

        public List<string> Permissions => new();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            string playerBalance = PlayToEarnPlugin.instance.Database.GetBalance(caller.Id);
            UnturnedChat.Say(caller, PlayToEarnPlugin.instance.Translate("balance_command", playerBalance));
        }
    }
}
