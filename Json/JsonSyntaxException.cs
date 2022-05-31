using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Json
{
    [Serializable]
    public class JsonSyntaxException : Exception
    {
        public JsonSyntaxException() { }
        public JsonSyntaxException(string message) : base(message) { }
        public JsonSyntaxException(string message, Exception inner) : base(message, inner) { }
        protected JsonSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
