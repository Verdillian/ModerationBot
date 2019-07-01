using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ModerationBot.Config;
using ModerationBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModerationBot.Modules.Filters
{
    class ProfanityMessage : ModuleBase
    {
        public static async Task WarningMessageAsync(SocketMessage pMsg, string word)
        {
            var message = pMsg as SocketUserMessage;
            await message.DeleteAsync();
            var user = pMsg.Author;

            var warningEmbed = new EmbedBuilder() { Color = Colors.errorCol };
            warningEmbed.Description = user + " | Do not use that profanity, your message has been deleted.";
            var msg = await pMsg.Channel.SendMessageAsync("", false, warningEmbed.Build());
            //await Delete.DelayDeleteMessage(msg, 10);
            
        }

        public static async Task LogMessageAsync(DiscordSocketClient bot, SocketMessage pMsg, string word)
        {
            var message = pMsg as SocketUserMessage;
            var user = pMsg.Author;
            var userId = pMsg.Author.Id;
            var channel = pMsg.Channel.ToString();
            var fullMsg = pMsg.ToString();
            var logsChannel = BotConfig.Load().BotLogs;

            var context = new SocketCommandContext(bot, message);
            var server = bot.Guilds.FirstOrDefault(x => x.Id == BotConfig.Load().ServerID);
            var logChannel = server.GetTextChannel(BotConfig.Load().BotLogs);

            var logEmbed = new EmbedBuilder() { Color = Colors.errorCol };
            logEmbed.WithAuthor("Profanity detected in discord chat");
            logEmbed.Description = "Full message: " + fullMsg;
            var userField = new EmbedFieldBuilder() { Name = "Discord User", Value = user };
            var userIdField = new EmbedFieldBuilder() { Name = "DiscordId", Value = userId };
            var channelField = new EmbedFieldBuilder() { Name = "Channel", Value = channel };
            var wordField = new EmbedFieldBuilder() { Name = "Word", Value = word };
            logEmbed.AddField(userField);
            logEmbed.AddField(userIdField);
            logEmbed.AddField(channelField);
            logEmbed.AddField(wordField);
            await logChannel.SendMessageAsync("", false, logEmbed.Build());
        }

        public static async Task DMMessageAsync(DiscordSocketClient bot, SocketMessage pMsg, string word)
        {
            var message = pMsg as SocketUserMessage;
            var user = pMsg.Author;
            var server = bot.Guilds.FirstOrDefault(x => x.Id == BotConfig.Load().ServerID);
            var dmMessage = new EmbedBuilder() { Color = Colors.errorCol };
            dmMessage.Description = user + " | Do not use that profanity, your message has been deleted and you have been banned from the discord.";
            dmMessage.AddField(new EmbedFieldBuilder() { Name = "", Value = "Looks like you got caught using a bad word, now why did you do that?" });
            var iDMChannel = await user.GetOrCreateDMChannelAsync();
            await iDMChannel.SendMessageAsync("", false, dmMessage.Build());
        }
    }
}
