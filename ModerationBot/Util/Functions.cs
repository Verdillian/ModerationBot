using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModerationBot.Util
{
    class Functions : ModuleBase
    {

        public ulong serverId()
        {
            return Context.Guild.Id;
        }

    }
}
