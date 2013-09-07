using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DabbitClient.Network;
using DabbitClient.Base;

namespace DabbitClient.Irc
{
    public interface IContext
    {
        Dictionary<string, IrcNetwork> GetNetworks();
        void AddNetwork(string key, IrcNetwork value);
        Dictionary<string, string> Settings { get; set; }
        T GetProvider<T>();
    }
    class IrcClientContext : IContext
    {
        public Dictionary<string, IrcNetwork> GetNetworks()
        {
            return new Dictionary<string, IrcNetwork>();
        }

        public void AddNetwork(string key, IrcNetwork value)
        {

        }

        public Dictionary<string, string> Settings
        {
            get { return new Dictionary<string, string>(); }
            set { }
        }

        public T GetProvider<T>()
        {
            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.GetInterfaces().Contains(typeof(T))
                                     && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as T;

            foreach (var instance in instances)
            {
                instance.Foo(); // where Foo is a method of ISomething
            }
        }
    }
}
