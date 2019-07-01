using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.IO;
using ModerationBot.Config;
using ModerationBot.Database;
using ModerationBot.Util;
using ModerationBot.Modules.Filters;

namespace ModerationBot
{
    public class CommandHandler : ModuleBase
    {
        private CommandService commands;
        private static DiscordSocketClient bot;
        private IServiceProvider map;

        public static readonly string appdir = AppContext.BaseDirectory;

        public CommandHandler(IServiceProvider provider)
        {
            map = provider;
            bot = map.GetService<DiscordSocketClient>();
            bot.Ready += SetGame;
            //bot.UserJoined += AnnounceUserJoined;
            //bot.UserLeft += AnnounceLeftUser;
            //LOGS
            bot.UserJoined += UserJoinedAsync;
            bot.UserLeft += UserLeftAsync;
            bot.UserBanned += UserBannedAsync;
            bot.UserUnbanned += UserUnbannedAsync;
            bot.ChannelCreated += ChannelCreatedAsync;
            bot.ChannelDestroyed += ChannelDestroyedAsync;
            bot.RoleCreated += RoleCreatedAsync;
            bot.RoleDeleted += RoleDeletedAsync;
            // Create new user profile in Database
            bot.UserJoined += CreateUserProfile;
            // Profanity filter
            bot.MessageReceived += ProfanityCheckAsync;
            //Send user message to get handled
            bot.MessageReceived += HandleCommand;
            commands = map.GetService<CommandService>();
        }

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

        private async Task RoleDeletedAsync(SocketRole role)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{role.Name}** has been deleted!";
            embed.Description = $"**{role.Name}** has been deleted!";
            embed.WithFooter("Time: " + DateTime.Now + " | Role Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task RoleCreatedAsync(SocketRole role)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{role.Name}** has been created!";
            embed.Description = $"**{role.Name}** has been created!";
            embed.WithFooter("Time: " + DateTime.Now + " | Role Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task ChannelDestroyedAsync(SocketChannel chnl)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{chnl}** has been destroyed!";
            embed.Description = $"**Channel has been destroyed**";
            embed.WithFooter("Time: " + DateTime.Now + " | Channel Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task ChannelCreatedAsync(SocketChannel chnl)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{chnl}** has been created!";
            embed.Description = $"**Channel has been created**";
            embed.WithFooter("Time: " + DateTime.Now + " | Channel Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task UserUnbannedAsync(SocketUser user, SocketGuild arg2)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{user.Username}** has been unbanned!";
            embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Unbanned By: **{Context.User.Mention}!";
            embed.WithFooter("Time: " + DateTime.Now + " | Unban Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task UserBannedAsync(SocketUser user, SocketGuild arg2)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{user.Username}** has been banned!";
            embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Banned By: **{Context.User.Mention}!";
            embed.WithFooter("Time: " + DateTime.Now + " | Ban Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task UserLeftAsync(SocketGuildUser user)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{user.Username}** has left the server!";
            embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Current Member Count: **{user.Guild.MemberCount}";
            embed.WithFooter("Time: " + DateTime.Now + " | Leave Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().UserJoinLeave);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            var embed = new EmbedBuilder()
            {
                Color = Colors.adminCol
            };

            embed.Title = $"**{user.Username}** has joined the server!";
            embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Current Member Count: **{user.Guild.MemberCount}";
            embed.WithFooter("Time: " + DateTime.Now + " | Join Log");

            var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().UserJoinLeave);
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task CreateUserProfile(SocketGuildUser user)
        {
            DB.EnterUser(user);
            await Program.Logger(new LogMessage(LogSeverity.Info, "Moderation", "User entered into database."));
        }

        //public async Task AnnounceLeftUser(SocketGuildUser user) { }

        //public async Task AnnounceUserJoined(SocketGuildUser user) { }

        public async Task SetGame()
        {
            await bot.SetGameAsync("Moderation | Moderating so you don't have too.");
        }

        public async Task ConfigureAsync() { await commands.AddModulesAsync(Assembly.GetEntryAssembly(), map); }

        public async Task HandleCommand(SocketMessage pMsg)
        {
            //Don't handle the command if it is a system message
            var message = pMsg as SocketUserMessage;
            if (message == null)
                return;
            var context = new SocketCommandContext(bot, message);

            //Mark where the prefix ends and the command begins
            int argPos = 0;
            //Determine if the message has a valid prefix, adjust argPos

            if (message.HasStringPrefix(BotConfig.Load().Prefix, ref argPos))
            {
                if (message.Author.IsBot)
                    return;
                //Execute the command, store the result
                var result = await commands.ExecuteAsync(context, argPos, map);

                //If the command failed, notify the user
                if (!result.IsSuccess && result.ErrorReason != "Unknown command.")

                    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
            }
        }

        //Getters and Setters
        public static DiscordSocketClient GetBot() { return bot; }

    }
}
