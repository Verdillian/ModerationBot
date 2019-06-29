﻿using Newtonsoft.Json;
using System;
using System.IO;

namespace ModerationBot.Config
{
    public class BotConfig
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public string Prefix { get; set; }
        public string Token { get; set; }
        public ulong BotLogs { get; set; }
        public string MutedRole { get; set; }
        public BotConfig()
        {
            Prefix = "--";
            Token = "";
            BotLogs = 0;
            MutedRole = "MUTED";
        }

        public void Save(string dir = "configuration/config.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }
        public static BotConfig Load(string dir = "configuration/config.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(file));
        }
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);




    }
}
