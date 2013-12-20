using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace dabbit.Base
{
    public class PrivmsgMessage : Message
    {
        public PrivmsgMessage()
        { }

        public PrivmsgMessage(Message old)
        {
            base.From = old.From;
            base.Command = old.Command;
            base.MessageLine = old.MessageLine;
            base.Parts = old.Parts;
            base.RawLine = old.RawLine;
            base.Timestamp = old.Timestamp;

            if (base.From.Type == SourceEntityType.Client)
            {
                this.UserFrom = new User();
                this.UserFrom.Nick = base.From.Parts[0];
                this.UserFrom.Ident = base.From.Parts[1];
                this.UserFrom.Host = base.From.Parts[2];
            }
        }
        public User UserFrom;
        public SourceEntity To;
        public string Wall; // + % @ & ~ before the channel ?
    }
    public class NoticeMessage : Message
    {
        public User UserFrom;
        public string To;
        public string Wall; // + % @ & ~ before the channel ?
    }
}
