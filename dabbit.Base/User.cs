using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace dabbit.Base
{
    public class User
    {
        public string Nick { get; internal set; }
        public string Ident { get; internal set; }
        public string Host { get; internal set; }
        public List<string> Modes { get; internal set; }
        public List<Channel> Channels { get; internal set; }
        public string Name { get; internal set; }
        public string Display { get { return this.Nick + "!" + this.Ident + "@" + this.Host; } }
        public bool IrcOp { get; internal set; }
        public bool Identified { get; internal set; }
        public string Server { get; internal set; }
        public int IdleTime { get; internal set; }
    }
}
