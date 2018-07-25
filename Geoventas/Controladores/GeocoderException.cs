using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores
{
    public class GeocoderException : Exception
    {
        public string codigoCliente { get; set; }

        public GeocoderException()
        {
        }

        public GeocoderException(string message) : base(message)
        {
        }

        public GeocoderException(Exception ex, string codigo) : base(ex.Message, ex)
        {
            this.codigoCliente = codigo;
        }
        public GeocoderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GeocoderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
