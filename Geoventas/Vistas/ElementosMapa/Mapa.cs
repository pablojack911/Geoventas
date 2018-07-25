using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public class Mapa : GMapControl
    {
        public Mapa()
            : base()
        {
            this.CenterCrossPen = null;
            this.ShowCenter = false;
            this.MapProvider = GoogleMapProvider.Instance;
            this.Manager.Mode = AccessMode.ServerAndCache;
            this.MaxZoom = 20;
            this.MinZoom = 1;
            this.Zoom = 15;
            //agregado por Pocho
            this.TouchEnabled = true;
            this.CanDragMap = true;
            //34.6000° S, 58.3833° W ARGENTINA
            //this.MapPoint = new Point(-34.6000, -58.3833);
            //this.Position = new PointLatLng(-34.6000, -58.3833);
            this.DragButton = System.Windows.Input.MouseButton.Left;
            this.IgnoreMarkerOnMouseWheel = true;
            //this.MouseDoubleClick += Map_MouseDoubleClick;
        }
    }
}
