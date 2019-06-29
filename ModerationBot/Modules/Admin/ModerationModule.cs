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

namespace ModerationBot.Modules.Admin
{
    class ModerationModule : ModuleBase
    {

        Errors error = new Errors();

        [Command("ban")]
        [Alias("blacklist", "bl")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(IUser user, [Remainder]string reason)
        {

            if (user != null)
            {
                if (reason != null)
                {
                    await Context.Guild.AddBanAsync(user, 0, reason);

                    var embed = new EmbedBuilder()
                    {
                        Color = Colors.adminCol
                    };

                    embed.Title = $"**{user.Username}** has been banned!";
                    embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Banned By: **{Context.User.Mention}!\n**Reason: **{reason}";
                    embed.WithFooter("Time: " + DateTime.Now + " | Ban Log");

                    var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
                    await channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    await error.sendError(Context.Channel, "Incorrect Command Usage: " + BotConfig.Load().Prefix + "ban @user#1234 Reason", Colors.errorCol);
                }
            }
            else
            {
                await error.sendError(Context.Channel, "Incorrect Command Usage: " + BotConfig.Load().Prefix + "ban @user#1234 Reason", Colors.errorCol);
            }
        }

        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUserAsync(SocketGuildUser user, [Remainder]string reason)
        {

            if (user != null)
            {
                if (reason != null)
                {
                    await user.KickAsync(reason);

                    var embeduser = new EmbedBuilder()
                    {
                        Color = Colors.adminCol
                    };

                    embeduser.Title = $"You have been kicked from " + Context.Guild.Name;
                    embeduser.Description = $"You have been kicked for the following reason: {reason}";
                    embeduser.WithFooter("Time: " + DateTime.Now + " | Kick Log");

                    await user.SendMessageAsync("", false, embeduser.Build());

                    var embed = new EmbedBuilder()
                    {
                        Color = Colors.adminCol
                    };

                    embed.Title = $"**{user.Username}** has been kicked!";
                    embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Kicked By: **{Context.User.Mention}!\n**Reason: **{reason}";
                    embed.WithFooter("Time: " + DateTime.Now + " | Kick Log");

                    var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
                    await channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    await error.sendError(Context.Channel, "Incorrect Command Usage: " + BotConfig.Load().Prefix + "kick @user#1234 Reason", Colors.errorCol);
                }
            }
            else
            {
                await error.sendError(Context.Channel, "Incorrect Command Usage: " + BotConfig.Load().Prefix + "kick @user#1234 Reason", Colors.errorCol);
            }
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.DeafenMembers)]
        public async Task MuteUserAsync(IGuildUser user, [Remainder]string reason)
        {
            if(user != null)
            {
                if(reason != null)
                {
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == BotConfig.Load().MutedRole);
                    await (user as IGuildUser).AddRoleAsync(role);

                    var embeduser = new EmbedBuilder()
                    {
                        Color = Colors.adminCol
                    };

                    embeduser.Title = $"You have been muted in " + Context.Guild.Name;
                    embeduser.Description = $"You have been muted for the following reason: {reason}";
                    embeduser.WithFooter("Time: " + DateTime.Now + " | Mute Log");

                    await user.SendMessageAsync("", false, embeduser.Build());

                    var embed = new EmbedBuilder()
                    {
                        Color = Colors.adminCol
                    };

                    embed.Title = $"**{user.Username}** has been muted!";
                    embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Muted By: **{Context.User.Mention}!\n**Reason: **{reason}";
                    embed.WithFooter("Time: " + DateTime.Now + " | Mute Log");

                    var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
                    await channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    await error.sendError(Context.Channel, "Incorrect Command Usage: " + BotConfig.Load().Prefix + "mute @user#1234 Reason", Colors.errorCol);
                }
            }
            else
            {
                await error.sendError(Context.Channel, "Incorrect Command Usage: " + BotConfig.Load().Prefix + "mute @user#1234 Reason", Colors.errorCol);
            }
        }

        [Command("unmute")]
        [RequireUserPermission(GuildPermission.DeafenMembers)]
        public async Task UnmuteUserAsync(IGuildUser user)
        {
            if (user != null)
            {
                var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == BotConfig.Load().MutedRole);
                await (user as IGuildUser).RemoveRoleAsync(role);

                var embeduser = new EmbedBuilder()
                {
                    Color = Colors.adminCol
                };

                embeduser.Title = $"You have been unmuted in " + Context.Guild.Name;
                embeduser.Description = $"You have been unmuted.";
                embeduser.WithFooter("Time: " + DateTime.Now + " | Mute Log");

                await user.SendMessageAsync("", false, embeduser.Build());

                var embed = new EmbedBuilder()
                {
                    Color = Colors.adminCol
                };

                embed.Title = $"**{user.Username}** has been unmuted!";
                embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{Context.Guild.Name}\n**Unmuted By: **{Context.User.Mention}!";
                embed.WithFooter("Time: " + DateTime.Now + " | Mute Log");

                var channel = await Context.Guild.GetTextChannelAsync(BotConfig.Load().BotLogs);
                await channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                await error.sendError(Context.Channel, "Incorrect Command Usage: " + BotConfig.Load().Prefix + "unmute @user#1234 Reason", Colors.errorCol);
            }
        }

        // Warn Command Later on - Connect to a DB - Takes less storage.

    }
}
