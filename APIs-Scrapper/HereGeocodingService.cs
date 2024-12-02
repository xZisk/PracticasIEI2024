using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IEIPracticas.APIs_Scrapper
{
    public class HereGeocodingService
    {
        private const string ApiKey = "VQRKZ8JdzsXSteMI-6Ijefe3MRd4zbyPIH63BN3P7M8";
        private const string GeocodeUrl = "https://geocode.search.hereapi.com/v1/geocode";
        private const string ReverseGeocodeUrl = "https://revgeocode.search.hereapi.com/v1/revgeocode";

        private readonly HttpClient _httpClient;

        public HereGeocodingService()
        {
            _httpClient = new HttpClient();
        }
        /*
         * Obtiene las coordenadas de una dirección usando la API de HERE.
         * address == Dirección
         * return Una dupla double de (latitud, longitud)
         */
        public async Task<(double Latitude, double Longitude)> GetCoordinatesFromAddress(string address)
        {
            string apiAddress = address.Replace(",", "").Replace(" ", "+");
            string url = $"{GeocodeUrl}?q={apiAddress}&apiKey={ApiKey}&lang=es_ES&limit=1";
            Console.WriteLine(url);
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return ParseCoordinates(jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error al conectar con la API de HERE: {e.Message}");
                return (0, 0); // Valores por defecto en caso de error
            }
        }
        /* Obtiene la dirección a partir de coordenadas geográficas usando la API de HERE.
         * latitude == Latitud
         * longitude == Longitud
         * returns Una dirección como string
         */
        public async Task<string> GetAddressFromCoordinates(double latitude, double longitude)
        {
            string latlon = latitude.ToString().Replace(",", ".") + "," + longitude.ToString().Replace(",", ".");
            string url = $"{ReverseGeocodeUrl}?at={latlon}&apiKey={ApiKey}&lang=es_ES&limit=1";
            Console.WriteLine(url);
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return ParseAddress(jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error al conectar con la API de HERE: {e.Message}");
                return null; // Null en caso de error
            }
        }
        /* Obtiene el codigo postal a partir de coordenadas geográficas usando la API de HERE.
         * latitude = Latitud
         * longitude = Longitud
         * returns Un codigo postal como string
         */
        public async Task<string> GetPostalCodeFromCoordinates(double latitude, double longitude)
        {
            string latlon = latitude.ToString().Replace(",", ".") + "," + longitude.ToString().Replace(",", ".");
            string url = $"{ReverseGeocodeUrl}?at={latlon}&apiKey={ApiKey}&lang=es_ES&limit=1";
            Console.WriteLine(url);
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return ParsePostalCode(jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error al conectar con la API de HERE: {e.Message}");
                return null; // Valor por defecto en caso de error
            }
        }
        // Extrae el codigo postal de la respuesta JSON.
        private string ParsePostalCode(string jsonResponse)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(jsonResponse);
                var address = jsonDoc.RootElement.GetProperty("items")[0].GetProperty("address");
                return address.GetProperty("postalCode").GetString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al procesar la respuesta de la API: {e.Message}");
                return null; // En caso de error, devolver null
            }
        }

        // Extrae coordenadas (latitud y longitud) de la respuesta JSON.
        private (double Latitude, double Longitude) ParseCoordinates(string jsonResponse)
        {
            var json = JObject.Parse(jsonResponse);
            var position = json["items"]?[0]?["position"];
            if (position != null)
            {
                double latitude = position["lat"].ToObject<double>();
                double longitude = position["lng"].ToObject<double>();
                return (latitude, longitude);
            }

            Console.WriteLine("No se encontraron coordenadas para la dirección proporcionada.");
            return (0, 0); // Valores por defecto en caso de error
        }
        // Extrae la dirección completa de la respuesta JSON.
        private string ParseAddress(string jsonResponse)
        {
            var json = JObject.Parse(jsonResponse);
            var address = json["items"]?[0]?["address"]?["label"]?.ToString();

            if (!string.IsNullOrEmpty(address))
            {
                return address;
            }

            Console.WriteLine("No se encontró una dirección para las coordenadas proporcionadas.");
            return null;
        }
    }
}
