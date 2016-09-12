using System.Runtime.CompilerServices;

namespace tomtiv.myMediaManager.core.mediaManagerLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MediaItemException : Exception
    {
        public MediaItemException(String message) : base (message) { }

        public MediaItemException(String message, Exception inner) : base(message,inner) { }
    }
}
