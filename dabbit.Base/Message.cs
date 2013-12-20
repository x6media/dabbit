using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dabbit.Base
{
    public class Message
    {
        public string[] Parts { get; internal set; }
        public string MessageLine { get; internal set; }
        public string Command { get; internal set; }
        public string RawLine { get; internal set; }

        public From From { get; internal set; }
        public DateTime Timestamp { get; internal set; }

    }
    public class From
    {
        public FromType Type { get { return this.fromType; } }
        public string[] Parts { get { return this.parts; } }

        public From(string[] parts, FromType from)
        {
            this.fromType = from;
            this.parts = parts;
        }

        private FromType fromType;
        private string[] parts;
    }
    public enum FromType
    {
        Server,
        Client,
        BNC,
        Dabbit
    }
}
