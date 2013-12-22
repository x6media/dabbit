using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace dabbit.Base
{
    public class User
    {
        public string Nick { get; set; }
        public string Ident { get; set; }
        public string Host { get; set; }
        public List<string> Modes { get; set; }
        public List<Channel> Channels { get; set; }
        public string Name { get; set; }
        public string Display { get { return this.Nick + "!" + this.Ident + "@" + this.Host; } }
        public bool IrcOp { get; set; }
        public bool Identified { get; set; }
        public string Server { get; set; }
        public int IdleTime { get; set; }
    }
}
