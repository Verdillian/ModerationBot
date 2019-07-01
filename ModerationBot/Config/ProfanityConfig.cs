using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModerationBot.Config
{
    public class ProfanityConfig
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public int Filters { get; set; }
        public string[] FilteredWords { get; set; }
        public ProfanityConfig()
        {
            Filters = 4;
            FilteredWords = new string[20];
        }

        public void Save(string dir = "configuration/config.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }
        public static ProfanityConfig Load(string dir = "configuration/ProfanityConfig.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<ProfanityConfig>(File.ReadAllText(file));
        }
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);




    }
}
