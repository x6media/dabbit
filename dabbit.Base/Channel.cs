using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace dabbit.Base
{
    public class Channel
    {
        public string Name { get; internal set; }
        public Topic Topic { get; internal set; }
        public List<Mode> Modes { get; internal set; }
        public List<User> Users { get; internal set; }
        public string Display { get; internal set; }

        public Channel DeepClone()
        {
            Channel chan = (Channel)this.MemberwiseClone();
            chan.Topic = this.Topic.DeepClone();

            return chan;
        }
    }

    public class Topic
    {
        public string Display { get; set; }
        public string SetBy { get; set; }
        public DateTime DateSet { get; set; }

        public Topic DeepClone()
        {
            return (Topic)this.MemberwiseClone();
        }
    }

}
