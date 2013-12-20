using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dabbit.Base
{
    public class User
    {
        public string Nick { get; protected set; }
        public string Ident { get; set; }
        public string Host { get; set; }
        public List<string> Modes { get; protected set; }
        public List<Channel> Channels { get; protected set; }
        public string Name { get; set; }
        public string Display { get { return this.Nick + "!" + this.Ident + "@" + this.Host; } }
        public bool IrcOp { get; internal set; }
        public bool Identified { get; internal set; }
        public string Server { get; internal set; }
        public int IdleTime { get; internal set; }
    }
}
