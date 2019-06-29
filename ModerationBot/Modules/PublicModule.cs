using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using ModerationBot.Config;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using ModerationBot.Util;

namespace ModerationBot.Modules
{
    public class PublicModule : ModuleBase
    {

        // Command to show how embeds work and just how a command works overall.
        [Command("ping")]
        [Remarks("Ping Pong")]
        public async Task PingPong()
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.generalCol
            };

            embed.Description = $"Pong";
            embed.WithFooter(new EmbedFooterBuilder().WithText($"Message From: {Context.User.Username} | Guild: {Context.Guild.Name}"));

            await Context.Channel.SendMessageAsync("", false, embed.Build());

            await Context.Message.DeleteAsync();
        }
    }

}
