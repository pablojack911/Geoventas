using GeoventasPocho.Factory.Pines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Factory
{
    public abstract class PinDecorator : Pin 
    {
        protected Pin pin;

        protected PinDecorator(Pin pin)
        {
            this.pin = pin;
        }
    }
}
