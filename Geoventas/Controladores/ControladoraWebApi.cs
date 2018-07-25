using GeoventasPocho.Vistas.ElementosMapa;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores
{
    public class ControladoraWebApi
    {
        static Uri soloAddress = new Uri("http://192.168.1.95:5000");
        static Uri localhostAddress = new Uri("http://localhost:5000");

        public static async Task<IEnumerable<ClienteFletero>> ObtenerClientes(string codigo)
        {
            IEnumerable<ClienteFletero> clientes = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(soloAddress + "api/fleterotest/ObtenerClientes/" + codigo).Result;  // Blocking call!  
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");
                    // Get the response
                    var customerJsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Your response data is: " + customerJsonString);

                    // Deserialise the data (include the Newtonsoft JSON Nuget package if you don't already have it)
                    clientes= JsonConvert.DeserializeObject<IEnumerable<ClienteFletero>>(custome‌​rJsonString);
                    // Do something with it
                    response.Dispose();
                }
            }
            return clientes;
        }

        public static async Task<IEnumerable<ClienteFletero>> ObtenerClientes(string codigo, DateTime fecha)
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = soloAddress;
                client.BaseAddress = localhostAddress;

                client.DefaultRequestHeaders.Accept.Clear();
                // Agrega el header Accept: application/json para recibir la data como json  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.GetAsync(client.BaseAddress + "api/fleterotest/ObtenerClientes/" + codigo + "/" + fecha, HttpCompletionOption.ResponseHeadersRead).Result.Dispose();
                //var response = await client.GetAsync(client.BaseAddress + "api/fleterotest/ObtenerClientes/" + codigo + "/" + fecha);

                //// Si el servicio responde correctamente
                //if (response.IsSuccessStatusCode)
                //{
                //    // Lee el response y lo deserializa como un Product
                //    return await response.Content.ReadAsAsync<IEnumerable<ClienteFletero>>();
                //}
                // Sino devuelve null
                return await Task.FromResult<IEnumerable<ClienteFletero>>(null);
            }
        }
    }
}
