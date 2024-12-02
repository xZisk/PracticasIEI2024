using IEIPracticas.APIs_Scrapper;
using IEIPracticas.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Mappers
{
    public static class JSONMapper
    {

        public static async Task<Monumento> JSONMonumentoToMonumento(JSONMonumento jsonMonumento)
        {
            HereGeocodingService api = new HereGeocodingService();
            string latitudStr = jsonMonumento.latwgs84.Replace(',', '.');
            string longitudStr = jsonMonumento.lonwgs84.Replace(',', '.');
            if (jsonMonumento == null)
            {
                Console.WriteLine("Error: El objeto JSONMonumento no puede ser nulo.");
                return null;
            }

            // Validar errores específicos
            if (string.IsNullOrWhiteSpace(jsonMonumento.documentName))
            {
                Console.WriteLine("Error: Monumento sin nombre.");
                return null;
            }
            if (string.IsNullOrEmpty(jsonMonumento.address))
            {
                Console.WriteLine($"Error: Direccion de '{jsonMonumento.documentName}' vacia o no definida, se intentara generar mediante las coordenadas.");
            }

            if (string.IsNullOrEmpty(latitudStr))
            {
                Console.WriteLine($"Error: Latitud de '{jsonMonumento.documentName}' vacia o no definida, se intentara generar mediante la dirección.");
            }

            if (string.IsNullOrEmpty(longitudStr))
            {
                Console.WriteLine($"Error: Longitud de '{jsonMonumento.documentName}' vacia o no definida, se intentara generar mediante la dirección");
            }

            if (string.IsNullOrEmpty(jsonMonumento.address) && (string.IsNullOrEmpty(longitudStr) || string.IsNullOrEmpty(latitudStr)))
            {
                Console.WriteLine($"Error: Imposible generar datos faltantes del monumento '{jsonMonumento.documentName}', rechazado.");
                return null;
            }
            else
            {
                (double Latitude, double Longitude) coordinates = (0.0, 0.0);
                if (string.IsNullOrEmpty(latitudStr) || string.IsNullOrEmpty(longitudStr))
                {
                    coordinates = await api.GetCoordinatesFromAddress(jsonMonumento.address + ", " + jsonMonumento.municipality);
                    Console.WriteLine($"Exito generando direccion de '{jsonMonumento.documentName}', continua en filtros.");
                }
                if (string.IsNullOrEmpty(latitudStr))
                {
                    latitudStr = coordinates.Latitude.ToString();
                    Console.WriteLine($"Exito generando latitud de '{jsonMonumento.documentName}', continua en filtros.");
                }
                if (string.IsNullOrEmpty(latitudStr))
                {
                    Console.WriteLine($"Error: Imposible generar latitud, monumento '{jsonMonumento.documentName}' rechazado.");
                    return null;
                }
                if (string.IsNullOrEmpty(longitudStr))
                {
                    longitudStr = coordinates.Longitude.ToString();
                    Console.WriteLine($"Exito generando longitud de '{jsonMonumento.documentName}', continua en filtros.");
                }
                if (string.IsNullOrEmpty(longitudStr))
                {
                    Console.WriteLine($"Error: Imposible generar longitud, monumento '{jsonMonumento.documentName}' rechazado.");
                    return null;
                }
                if (string.IsNullOrEmpty(jsonMonumento.address))
                {
                    if (!double.TryParse(latitudStr.Replace(",","."), NumberStyles.Any, CultureInfo.InvariantCulture, out double lat))
                    {
                        Console.WriteLine($"Error: Monumento '{jsonMonumento.documentName}' tiene una latitud inválida, imposible generar datos.");
                        return null;
                    }
                    if (!double.TryParse(longitudStr.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                    {
                        Console.WriteLine($"Error: Monumento '{jsonMonumento.documentName}' tiene una longitud inválida, imposible generar datos.");
                        return null;
                    }
                    jsonMonumento.address = await api.GetAddressFromCoordinates(lat, lon);
                }
                if (string.IsNullOrEmpty(jsonMonumento.address) || jsonMonumento.address.Contains("api"))
                {
                    Console.WriteLine($"Error: Imposible generar direccion, monumento '{jsonMonumento.documentName}' rechazado.");
                    return null;
                }
                else Console.WriteLine($"Exito generando direccion para '{jsonMonumento.documentName}', continua en filtros.");
            }

            if (string.IsNullOrEmpty(jsonMonumento.postalCode?.ToString()))
            {
                Console.WriteLine($"Error: Codigo postal de '{jsonMonumento.documentName}' vacio o no definido, se intentara generar mediante las coordenadas.");
                jsonMonumento.postalCode = await api.GetPostalCodeFromCoordinates(double.Parse(latitudStr, CultureInfo.InvariantCulture), double.Parse(longitudStr, CultureInfo.InvariantCulture));
                if (string.IsNullOrEmpty(jsonMonumento.postalCode))
                {
                    Console.WriteLine($"Error: No se ha podido generar un codigo postal, monumento '{jsonMonumento.documentName}' rechazado.");
                    return null;
                }
                Console.WriteLine($"Exito generando codigo postal para '{jsonMonumento.documentName}', continua en filtros.");
            }

            if (!double.TryParse(latitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double latitud) || latitud < -90 || latitud > 90)
            {
                Console.WriteLine($"Error: Monumento '{jsonMonumento.documentName}' tiene una latitud inválida.");
                return null;
            }

            if (!double.TryParse(longitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double longitud) || longitud < -180 || longitud > 180)
            {
                Console.WriteLine($"Error: Monumento '{jsonMonumento.documentName}' tiene una longitud inválida.");
                return null;
            }

            if (!int.TryParse(jsonMonumento.postalCode, out int codigoPostal))
            {
                Console.WriteLine($"Error: Monumento '{jsonMonumento.documentName}' tiene un código postal inválido.");
                return null;
            }

            if (codigoPostal != 0 && (codigoPostal < 1000 || codigoPostal > 52999))
            {
                Console.WriteLine($"Error: Monumento '{jsonMonumento.documentName}' tiene un código postal fuera del rango válido: {codigoPostal}.");
                return null;
            }

            if (codigoPostal.ToString().Length < 5 && !string.IsNullOrEmpty(codigoPostal.ToString()))
            {
                Console.WriteLine($"Error: El codigo postal de '{jsonMonumento.documentName}' no presenta las 5 cifras, se añadiran 0 a la izquierda para arreglarlo.");
            }
            // Crear el objeto Monumento si pasa todas las validaciones
            var monumento = new Monumento
            {
                Nombre = jsonMonumento.documentName,
                Localidad = jsonMonumento.municipality,
                Provincia = jsonMonumento.territory,
                Tipo = MapTipo(jsonMonumento.documentName),
                Descripcion = jsonMonumento.documentDescription,
                Direccion = jsonMonumento.address + ", " + jsonMonumento.postalCode + " " + jsonMonumento.municipality,
                CodigoPostal = codigoPostal,
                Longitud = longitud,
                Latitud = latitud,
            };
            return monumento;
        }
        private static Tipo MapTipo(string documentName)
        {
            if (documentName.Contains("Yacimiento arqueológico", StringComparison.OrdinalIgnoreCase))
                return Tipo.Yacimiento; // Yacimiento arqueológico

            if (documentName.Contains("Ermita", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Iglesia", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Catedral", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Basílica", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Santuario", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Parroquia", StringComparison.OrdinalIgnoreCase))
                return Tipo.Iglesia; // Iglesia-Ermita

            if (documentName.Contains("Monasterio", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Convento", StringComparison.OrdinalIgnoreCase))
                return Tipo.Monasterio; // Monasterio-Convento

            if (documentName.Contains("Palacio", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Castillo", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Torre", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Fortaleza", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Fuerte", StringComparison.OrdinalIgnoreCase))
                return Tipo.Castillo; // Castillo-Fortaleza-Torre

            if (documentName.Contains("Edificio", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Molino", StringComparison.OrdinalIgnoreCase) ||
                documentName.Contains("Teatro", StringComparison.OrdinalIgnoreCase))
                return Tipo.Edificio; // Edificio singular

            if (documentName.Contains("Puente", StringComparison.OrdinalIgnoreCase))
                return Tipo.Puente; // Puente

            return Tipo.Otros; // Otro
        }

    }
}
