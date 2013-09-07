using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DabbitClient.Base;

namespace DabbitClient.Irc
{
    class IrcUserEntity : IrcEntity
    {
        /// <summary>
        /// Constructor for IrcUserEntity when a host is not known.
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="ident"></param>
        /// <param name="real"></param>
        public IrcUserEntity(string nickname, string ident, string real) : base(nickname)
        {

            if (String.IsNullOrEmpty(nickname))
            {
                throw new ArgumentNullException("nickname");
            }

            if (String.IsNullOrEmpty(ident))
            {
                throw new ArgumentNullException("ident");
            }

            if (String.IsNullOrEmpty(real))
            {
                throw new ArgumentNullException("real");
            }

            this.nickname = nickname;
            this.ident = ident;
            this.real = real;
            this.modes = new List<IrcModeChannelEntity>();
        }

        /// <summary>
        /// Create a new instance of a user when the host is known. Stores the Nickname, the ident, host, and the real name
        /// of the user.
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="ident"></param>
        /// <param name="real"></param>
        /// <param name="host"></param>
        public IrcUserEntity(string nickname, string ident, string real, string host) : this(nickname, ident, real)
        {
            if (String.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException("host");
            }

            this.host = host;
        }

        /// <summary>
        /// Gets the Nickname of the user
        /// </summary>
        public string Nickname
        {
            get { return this.nickname; }
        }

        /// <summary>
        /// Gets the Ident of the user
        /// </summary>
        public string Ident
        {
            get { return this.ident; }
        }

        /// <summary>
        /// Gets the hostname for the user (if one is set)
        /// </summary>
        public string Host
        {
            get { return this.host; }
        }

        /// <summary>
        /// Gets the Real name of the user
        /// </summary>
        public string RealName
        {
            get { return this.real; }
        }

        /// <summary>
        /// Returns the full (raw) form of the user: nick!ident@host
        /// </summary>
        public string DisplayFull
        {
            get
            {
                return this.Nickname + "@" + this.Ident + "@" + this.host;
            }
        }

        /// <summary>
        /// Add a user mode to this user
        /// </summary>
        /// <param name="modeEntity">The Mode for this user</param>
        public void AddUserChannelMode(IrcModeChannelEntity modeEntity)
        {
            if (modeEntity == null)
            {
                throw new ArgumentNullException("modeEntity");
            }

            this.modes.Add(modeEntity);
        }

        /// <summary>
        /// Remove a mode based on the IRC protocol value (qaohv)
        /// </summary>
        /// <param name="protocolDisplay"></param>
        public void RemoveUserChannelMode(string protocolDisplay)
        {
            var modesToRemove = this.modes
                .Select((item, index) => new { Item = item, Position = index })
                .Where(x => x.Item.IrcDisplay == protocolDisplay);
            
            foreach (var mode in modesToRemove)
            {
                this.RemoveUserChannelModeByIndex(mode.Position);
            }
        }
        
        /// <summary>
        /// Remove a mode based on the index of the user's mode list
        /// </summary>
        /// <param name="indexId"></param>
        public void RemoveUserChannelModeByIndex(int indexId)
        {
            if (indexId < 0 || indexId >= this.modes.Count)
            {
                throw new ArgumentOutOfRangeException("indexId");
            }

            this.modes.RemoveAt(indexId);
        }

        private string nickname = String.Empty;
        private string ident = String.Empty;
        private string host = String.Empty;
        private string real = String.Empty;

        private List<IrcModeChannelEntity> modes; // Change!!
    }
}
