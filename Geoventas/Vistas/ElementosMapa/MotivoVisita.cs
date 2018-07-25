using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public enum TipoVisita : int
    {
        Pendiente = 0,
        [Description("Entrega Total")]
        EntregaTotal = 1,
        [Description("Entrega Parcial")]
        EntregaParcial = 2,
        Rechazado = 3,
        Cerrado = 4,
        [Description("Volver luego")]
        VolverLuego = 5,
        [Description("En viaje")]
        EnViaje = 6,
        [Description("Sin visitar")]
        SinVisitar = 7
    }
}
