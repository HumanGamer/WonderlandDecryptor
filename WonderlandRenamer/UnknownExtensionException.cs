using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WonderlandRenamer
{
    public class UnknownExtensionException : Exception
    {
        public UnknownExtensionException(string message) : base(message)
        {

        }

        public UnknownExtensionException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public UnknownExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
