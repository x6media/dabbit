/**
 * Copyright David Barajas 2013
 * 
 * This file defines an Xml document  class. When XML files are parsed, they should
 * be converted to an XML class. This is the base of that class.
 * 
 * @Version v1.0
 * @Author dab
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dabbit.Base
{
    class IrcXml : IrcEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="profileId"></param>
        public IrcXml(string fullFileName, Guid profileId) : base(fullFileName)
        {
            this.fullFileName = fullFileName;
            this.profileId = profileId;
        }

        public Guid ProfileId
        {
            get
            {
                return this.profileId;
            }
        }

        private string fullFileName = String.Empty;
        private Guid profileId;
    }
}
