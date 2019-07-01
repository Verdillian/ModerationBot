using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ModerationBot.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModerationBot.Util;

namespace ModerationBot.Modules.Filters
{
    public class ProfanityFilter : ModuleBase
    {
        public async Task ProfanityCheckAsync(SocketMessage pMsg)
        {
            var message = pMsg as SocketUserMessage;
            if (message == null)
                return;

            for (int i = 0; i <= ProfanityConfig.Load().Filters - 1; i++)
            {
                if (message.ToString().ToLower().Contains(ProfanityConfig.Load().FilteredWords[i].ToLower()))
                {
                    await ProfanityMessage.WarningMessageAsync(pMsg, ProfanityConfig.Load().FilteredWords[i]);
                    await ProfanityMessage.LogMessageAsync(CommandHandler.GetBot(), pMsg, ProfanityConfig.Load().FilteredWords[i]);
                    await ProfanityMessage.DMMessageAsync(CommandHandler.GetBot(), pMsg, ProfanityConfig.Load().FilteredWords[i]);
                    await ProfanityBanAsync(CommandHandler.GetBot(), pMsg);

                }
            }
        }

        public async Task ProfanityBanAsync(DiscordSocketClient bot, SocketMessage pMsg)
        {
            var server = bot.Guilds.FirstOrDefault(x => x.Id == Context.Guild.Id);
            await server.AddBanAsync(pMsg.Author, 7, "Profanity detected in discord chat. Check server logs for more information.");
            await Program.Logger(new LogMessage(LogSeverity.Info, "Moderation", "Profanity was detected by the user " + pMsg.Author.Username + "."));
        }
    }
}
