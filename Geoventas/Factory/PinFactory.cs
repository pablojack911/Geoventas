using GeoventasPocho.Controladores;
using GeoventasPocho.Factory.Pines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoventasPocho.Factory
{
    public static class PinFactory
    {
        public static Pin MakePin()
        {
            var pin = new Pin
            {
                Height = 40,
                Width = 42,
                ContextMenu = new ContextMenu()
            };
            return pin;
        }

        public static Pin MakePin (TipoPin tipo)
        {
            var pin = MakePin();
            pin.setTipo(tipo);
            return pin;
        }

        public static Pin MakePin(TipoPin tipo, string etiqueta)
        {
            var pin = MakePin();
            pin.setTipo(tipo);
            pin.setEtiqueta(etiqueta);
            return pin;
        }
    }
}
