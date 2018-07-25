using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public enum Estado : int
    {
        OK = 0,
        GPS_APAGADO = 1,
        DATOS_APAGADO = 2,
        FUERA_DE_ZONA = 3,
        DESLOGUEADO = 4,
        NO_REPORTA = 5,
        CHECKIN_CLIENTE = 6,
        CHECKOUT_CLIENTE = 7
    }
}
