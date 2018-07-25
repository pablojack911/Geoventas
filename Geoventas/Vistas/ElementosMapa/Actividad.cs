using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public class Actividad
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public List<Cliente> Clientes { get; set; }
    }
}
