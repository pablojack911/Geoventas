using GMap.NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GeoventasPocho.Controladores.Mapas
{
    public class GoogleMapsGeocoder
    {
        //api de Bing
        //http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}
        //http://dev.virtualearth.net/REST/v1/Locations?q=moreno%202254,mar%20del%20plata&key=bpBO0zDlRa970B6utXiJ~fX83ZmlkorNqopUtDFehGw~AoCRAJfGGthrRRq9qQ-NDmrbtYT5fP9ODX52kWa-bfj07spvy3UkDju_BoNlDaCe
        public static PointLatLng ObtenerCordenadasPorDireccion(string direccion, out GeoCoderStatusCode status)
        {
            var address = direccion;
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}", Uri.EscapeDataString(address));
            double lat = 0;
            double lng = 0;
            //34.6000° S, 58.3833° W ARGENTINA
            var request = WebRequest.Create(requestUri);
            try
            {
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());

                switch (xdoc.Element("GeocodeResponse").Element("status").Value)
                {
                    case "OK":
                        status = GeoCoderStatusCode.G_GEO_SUCCESS;
                        break;
                    case "ZERO_RESULTS":
                        status = GeoCoderStatusCode.G_GEO_UNAVAILABLE_ADDRESS;
                        break;
                    case "OVER_QUERY_LIMIT":
                        throw new OverQueryLimitException("Se excedió la cantidad de consultas a Google.");
                        break;
                    case "REQUEST_DENIED":
                        status = GeoCoderStatusCode.ExceptionInCode;
                        break;
                    case "INVALID_REQUEST":
                        status = GeoCoderStatusCode.G_GEO_BAD_REQUEST;
                        break;
                    default:
                        status = GeoCoderStatusCode.Unknow;
                        break;
                }



                if (status == GeoCoderStatusCode.G_GEO_SUCCESS)
                {
                    var result = xdoc.Element("GeocodeResponse").Element("result");
                    var locationElement = result.Element("geometry").Element("location");
                    lat = double.Parse(locationElement.Element("lat").Value, CultureInfo.InvariantCulture);
                    lng = double.Parse(locationElement.Element("lng").Value, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception exc)
            {
                throw exc;
                //status = GeoCoderStatusCode.ExceptionInCode;
            }
            var point = new PointLatLng(lat, lng);

            return point;

        }

        //public static String ObtenerDireccionPorCoordenadas(PointLatLng coordenada, out GeoCoderStatusCode status)
        public static String ObtenerDireccionPorCoordenadas(PointLatLng coordenada)
        {
            GeoCoderStatusCode status;
            var direccion = string.Empty;
            var lat = coordenada.Lat.ToString().Replace(',', '.');
            var lng = coordenada.Lng.ToString().Replace(',', '.');
            var requestUri = string.Format(@"https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}", lat, lng);
            var request = WebRequest.Create(requestUri);
            try
            {
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());

                switch (xdoc.Element("GeocodeResponse").Element("status").Value)
                {
                    case "OK":
                        status = GeoCoderStatusCode.G_GEO_SUCCESS;
                        break;
                    case "ZERO_RESULT":
                        status = GeoCoderStatusCode.G_GEO_UNAVAILABLE_ADDRESS;
                        break;
                    case "OVER_QUERY_LIMIT":
                        status = GeoCoderStatusCode.G_GEO_TOO_MANY_QUERIES;
                        break;
                    case "REQUEST_DENIED":
                        status = GeoCoderStatusCode.ExceptionInCode;
                        break;
                    case "INVALID_REQUEST":
                        status = GeoCoderStatusCode.G_GEO_BAD_REQUEST;
                        break;
                    default:
                        status = GeoCoderStatusCode.Unknow;
                        break;
                }



                if (status == GeoCoderStatusCode.G_GEO_SUCCESS)
                {
                    var nro = string.Empty;
                    var calle = string.Empty;
                    var resultados = xdoc.Element("GeocodeResponse").Element("result").Elements("address_component");
                    foreach (var item in resultados)
                    {
                        if (item.Element("type").Value == "street_number")
                            nro = item.Element("long_name").Value;
                        else
                            if (item.Element("type").Value == "route")
                            calle = item.Element("long_name").Value;
                    }
                    direccion = calle + " " + nro;
                }
                else
                {
                    direccion = "";
                    throw new Exception(xdoc.Element("error_message").Value);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return direccion;
        }
    }
}
