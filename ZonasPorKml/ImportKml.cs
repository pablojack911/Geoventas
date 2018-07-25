using MatrizDeAhorroWPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using ZonasPorKml;

namespace Inteldev.Fixius.Mapas
{
    public class ImportKml : DependencyObject
    {

        //public Dictionary<string, List<Coordenada>> Zonas { get; set; }

        //public List<Cliente> Clientes
        //{
        //    get { return (List<Cliente>)GetValue(ClientesProperty); }
        //    set { SetValue(ClientesProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Clientes.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ClientesProperty =
        //    DependencyProperty.Register("Clientes", typeof(List<Cliente>), typeof(ImportKml));

        public static List<Zona> ImportarZonasKml(string pathoffile)
        {
            var Zonas = new List<Zona>();

            XNamespace ns = "http://www.opengis.net/kml/2.2";
            //XNamespace ns = "http://www.google.com/kml/ext/2.2";

            var doc = XDocument.Load(pathoffile);
            //var d = doc.Root.Elements().Elements(ns + "Folder").Elements(ns + "Folder").Elements(ns + "Folder").Elements(ns + "Folder").Elements(ns + "Placemark");

            var query = doc.Root
               .Element(ns + "Document")
               .Elements(ns + "Folder")
               //.Elements(ns + "Folder")
               //.Elements(ns + "Folder")
               //.Elements(ns + "Folder")
               .Elements(ns + "Placemark")
               .Select(x => new
               {
                   Name = x.Element(ns + "name").Value,
                   //Description = x.Element(ns + "description")?.Value,
                   //Coordinates = x.Element(ns + "Point")?.Value,
                   Polygono = x.Element(ns + "Polygon")?.Value.Substring(1).Trim()

                   // etc
               });


            //this.Clientes = new List<Cliente>();


            foreach (var item in query)
            {
                if (item.Polygono != null)
                    if (item.Polygono.Length != 0)
                        cargaZona(Zonas, item);
            }

            return Zonas;
        }

        private static void cargaZona(List<Zona> zonas, dynamic item)
        {
            var verti = item.Polygono.Split(' ');
            var codigo = ((string)item.Name).Split('-').LastOrDefault().Trim().Replace(" ", string.Empty).PadLeft(3, '0');

            double lat;
            double lng;

            foreach (var vx in verti)
            {
                var coord = vx.Split(',');
                lng = double.Parse(coord[0], CultureInfo.InvariantCulture);
                lat = double.Parse(coord[1], CultureInfo.InvariantCulture);
                zonas.Add(new Zona()
                {
                    Codigo = codigo,
                    Lat = lat,
                    Lng = lng
                });
            }
        }

        //void CargarClientes(dynamic item)
        //{
        //    string codigo = ((string)item.Name).Split('(', ')').ElementAtOrDefault(1); ;

        //    if (codigo == null)
        //        codigo = ((string)item.Description).Split('(', ')').ElementAtOrDefault(1);


        //    var coor = item.Coordinates.Split(',');
        //    double lat = 0;
        //    double lng = 0;
        //    try
        //    {
        //        if (coor.Length == 3)
        //        {
        //            lng = double.Parse(coor[0], CultureInfo.InvariantCulture);
        //            lat = double.Parse(coor[1], CultureInfo.InvariantCulture);
        //        }
        //    }
        //    catch (Exception exc)
        //    {

        //    }


        //    var cliente = new CoordenadaCliente()
        //    {
        //        Codigo = codigo ?? "",
        //        Nombre = item.Description,
        //        //Domicilio = item.Name,
        //        Latitud = lat,
        //        Longitud = lng

        //    };


        //    this.Clientes.Add(cliente);
        //    if (cliente.Codigo.Trim() != string.Empty)
        //    {
        //        cliente.Codigo = cliente.Codigo.PadLeft(5, '0');
        //        this.Clientes.Add(cliente);
        //    }
        //}

    }
}
