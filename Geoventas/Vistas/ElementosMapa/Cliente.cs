using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public class Cliente
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Localidad { get; set; }
        public string Observacion { get; set; }
        public PointLatLng? Coordenada { get; set; }
        public int OrdenRecorrido { get; set; }
        public string Actividad { get; set; } //RAMO
        public bool Roja { get; set; }

        public override string ToString()
        {
            return this.Codigo + "\n" + this.Nombre + "\nActividad: " + this.Actividad + "\n" + this.Calle + " " + this.Numero;
        }
    }
}
