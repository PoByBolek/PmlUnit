// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Runtime.Serialization;

namespace PmlUnit
{
    [Serializable]
    public class PmlException : Exception
    {
        public PmlError Error { get; }

        public PmlException()
        {
        }

        public PmlException(string message)
            : base(message)
        {
        }

        public PmlException(string message, PmlError error)
            : base(message)
        {
            Error = error;
        }

        public PmlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            Error = PmlError.FromString(info.GetString("PmlException.Error"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
            info.AddValue("PmlException.Error", Error == null ? "" : Error.ToString());
        }
    }
}
