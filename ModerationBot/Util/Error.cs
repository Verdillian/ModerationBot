using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModerationBot.Util
{
    class Errors : ModuleBase
    {

        public async Task sendError(ISocketMessageChannel channel, string error, Color color)
        {
            //Console.WriteLine("ERROR: " + error);

            var embed = new EmbedBuilder() { Color = color };
            embed.Title = ("ERROR");
            embed.Description = (error);
            await channel.SendMessageAsync("", false, embed.Build());

            Console.WriteLine("Error message sent to user: " + error);

        }

        public async Task sendError(IMessageChannel channel, string error, Color color)
        {
            //Console.WriteLine("ERROR: " + error);

            var embed = new EmbedBuilder() { Color = color };
            embed.Title = ("ERROR");
            embed.Description = (error);
            await channel.SendMessageAsync("", false, embed.Build());

            Console.WriteLine("Error message sent to user: " + error);

        }

    }
}
