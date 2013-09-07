using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dabbit.Exceptions
{
    public class KeyAlreadyExistsException : Exception
    {
        public KeyAlreadyExistsException(string dictionaryName, string networkKey)
            : base(String.Format("The key {0} already exists in the {1} dictionary", networkKey, dictionaryName))
        {

        }
    }

    public class KeyNotExistsException : Exception
    {
        public KeyNotExistsException(string dictionaryName, string networkKey)
            : base(String.Format("The key {0} doesn't exist in the {1} dictionary", networkKey, dictionaryName))
        {

        }
    }
}
