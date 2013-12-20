using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dabbit.Base
{
    public class PrivmsgMessage : Message
    {
        public User From;
        public string To;
        public string Wall; // + % @ & ~ before the channel ?
    }
    public class NoticeMessage : Message
    {
        public User From;
        public string To;
        public string Wall; // + % @ & ~ before the channel ?
    }
}
