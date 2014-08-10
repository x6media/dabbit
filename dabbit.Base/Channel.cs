using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace dabbit.Base
{
    public class Channel
    {
        protected Channel()
        {

        }

        public Channel(Server svr)
        {
            this.ServerOf = svr;
        }

        public string Name { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual List<Mode> Modes { get; internal set; }
        public virtual List<User> Users { get; internal set; }
        public string Display { get; internal set; }

        public DateTime Created { get; internal set; }

        public Server ServerOf { get; internal set; }

        public Channel DeepClone()
        {
            Channel chan = (Channel)this.MemberwiseClone();
            chan.Topic = this.Topic.DeepClone();
            
            return chan;
        }

        public bool ChannelLoaded
        {
            get
            {
                return !String.IsNullOrEmpty(Name) && Modes != null && Users.Count() != 0 && !String.IsNullOrEmpty(Display);
            }
        }

        public override string ToString()
        {
            return this.Name + "2s";
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
