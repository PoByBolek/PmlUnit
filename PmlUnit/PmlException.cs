using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PmlUnit
{
    [Serializable]
    public class PmlException : Exception
    {
        public PmlException()
        {
        }

        public PmlException(Hashtable stackTrace)
            : base(BuildMessage(stackTrace))
        {
        }

        public PmlException(string message)
            : base(message)
        {
        }

        public PmlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string BuildMessage(Hashtable stackTrace)
        {
            if (stackTrace == null || stackTrace.Count == 0)
                return "An error occurred";

            var message = new StringBuilder();
            foreach (var key in stackTrace.Keys.OfType<double>().OrderBy(x => x))
                message.AppendLine(stackTrace[key].ToString());

            return message.ToString();
        }
    }
}
