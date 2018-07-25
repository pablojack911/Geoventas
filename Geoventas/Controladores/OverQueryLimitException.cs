using System;
using System.Runtime.Serialization;

namespace GeoventasPocho.Controladores
{
    [Serializable]
    internal class OverQueryLimitException : Exception
    {
        public OverQueryLimitException()
        {
        }

        public OverQueryLimitException(string message) : base(message)
        {
        }

        public OverQueryLimitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OverQueryLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}