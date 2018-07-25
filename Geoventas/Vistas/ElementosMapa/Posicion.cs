using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public class Posicion
    {
        public DateTime Fecha { get; set; }
        //public PointLatLng Coordenada { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public Estado Estado { get; set; }
        public string Cliente { get; set; }
        public MotivoNoCompra MotivoNoCompra { get; set; }
        public TipoVisita TipoVisita { get; set; }
        public decimal PesosCompra { get; set; }
        public int BultosCompra { get; set; }
    }
}
