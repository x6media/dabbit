using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dabbit.Exceptions
{
    public class UninitializedException : Exception
    {
        public UninitializedException(string className)
            : base(String.Format("The Singleton class {0}, was not initialized", className))
        {
        }
    }
}
