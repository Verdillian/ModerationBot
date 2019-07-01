using System;
using System.Collections.Generic;
using System.Text;

namespace ModerationBot.Database
{
    public class Profile
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Quote { get; set; }
        public string AboutMe { get; set; }
        public string MUID { get; set; }
        public int Money { get; set; }
        public int Points { get; set; }
        public int Warnings { get; set; }
    }
}
