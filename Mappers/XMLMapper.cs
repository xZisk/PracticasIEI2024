using System;
using System.Globalization;
using System.Text.RegularExpressions;
using IEIPracticas.APIs_Scrapper;
using IEIPracticas.Models;
using MyProject.Models;

namespace IEIPracticas
{
    public static class XMLMapper
    {
        public static async Task<Monumento> XMLMonumentoToMonumento(XMLMonumento xm)
        {
            HereGeocodingService api = new HereGeocodingService();
            string latitudStr = xm.coordenadas.latitud.Replace(',', '.'); 
            string longitudStr = xm.coordenadas.longitud.Replace(',', '.');
            (string adress, string postalCode) respuestaApi;

            if (xm == null)
            {
                Console.WriteLine("Error: El objeto XMLMonumento no puede ser nulo.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(xm.nombre))  
            {
                Console.WriteLine("Error: Monumento sin nombre.");
                return null;
            }

            if (string.IsNullOrEmpty(xm.calle))
            {
                Console.WriteLine($"Error: Direccion de '{xm.nombre}' vacia o no definida, se intentara generar mediante las coordenadas.");
            }

            if (string.IsNullOrEmpty(latitudStr))
            {
                Console.WriteLine($"Error: Latitud de '{xm.nombre}' vacia o no definida, se intentara generar mediante la dirección.");
            }

            if (string.IsNullOrEmpty(longitudStr))
            {
                Console.WriteLine($"Error: Longitud de '{xm.nombre}' vacia o no definida, se intentara generar mediante la dirección");
            }

            if (string.IsNullOrEmpty(xm.calle) && (string.IsNullOrEmpty(longitudStr) || string.IsNullOrEmpty(latitudStr)))
            {
                Console.WriteLine($"Error: Imposible generar datos faltantes del monumento '{xm.nombre}', rechazado.");
                return null;
            }
            else
            {
                (double Latitude, double Longitude) coordinates = (0.0, 0.0);
                if (string.IsNullOrEmpty(latitudStr) || string.IsNullOrEmpty(longitudStr))
                {
                    coordinates = await api.GetCoordinatesFromAddress(xm.calle + ", " + xm.poblacion.localidad);
                }
                if (string.IsNullOrEmpty(latitudStr))
                { 
                    latitudStr = coordinates.Latitude.ToString();
                    Console.WriteLine($"Exito generando latitud de '{xm.nombre}', continua en filtros.");
                }
                if (string.IsNullOrEmpty(latitudStr))
                {
                    Console.WriteLine($"Error: Imposible generar latitud, monumento '{xm.nombre}' rechazado.");
                    return null;
                }
                if (string.IsNullOrEmpty(longitudStr))
                {
                    longitudStr = coordinates.Longitude.ToString();
                    Console.WriteLine($"Exito generando longitud de '{xm.nombre}', continua en filtros.");
                }
                if (string.IsNullOrEmpty(longitudStr))
                {
                    Console.WriteLine($"Error: Imposible generar longitud, monumento '{xm.nombre}' rechazado.");
                    return null;
                }
                if (string.IsNullOrEmpty(xm.calle))
                {
                    if (!double.TryParse(latitudStr.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double lat))
                    {
                        Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene una latitud inválida, imposible generar datos.");
                        return null;
                    }
                    if (!double.TryParse(longitudStr.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                    {
                        Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene una longitud inválida, imposible generar datos.");
                        return null;
                    }
                    respuestaApi = await api.GetAddressAndPostalCodeFromCoordinates(lat, lon);
                    xm.calle = respuestaApi.adress;
                    Console.WriteLine($"Exito generando direccion de '{xm.nombre}', continua en filtros.");
                }
            }

            if (string.IsNullOrEmpty(xm.codigoPostal?.ToString()))
            {
                Console.WriteLine($"Error: Codigo postal de '{xm.nombre}' vacio o no definido, se intentara generar mediante las coordenadas.");
                respuestaApi = await api.GetAddressAndPostalCodeFromCoordinates(double.Parse(latitudStr, CultureInfo.InvariantCulture),double.Parse(longitudStr, CultureInfo.InvariantCulture));
                xm.codigoPostal = respuestaApi.postalCode;
                if (string.IsNullOrEmpty(xm.codigoPostal))
                {
                    Console.WriteLine($"Error: No se ha podido generar un codigo postal, monumento '{xm.nombre}' rechazado.");
                    return null;
                }
                Console.WriteLine($"Exito generando codigo postal para '{xm.nombre}', continua en filtros.");
            }

            if (!double.TryParse(latitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double latitud) || latitud < -90 || latitud > 90)
            {
                Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene una latitud inválida.");
                return null;
            }

            if (!double.TryParse(longitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double longitud) || longitud < -180 || longitud > 180)
            {
                Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene una longitud inválida.");
                return null;
            }

            if (!int.TryParse(xm.codigoPostal, out int codigoPostal))
            {
                Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene un código postal inválido.");
                return null;  
            }

            if (codigoPostal != 0 && (codigoPostal < 1000 || codigoPostal > 52999))
            {
                Console.WriteLine($"Error: Monumento '{xm.nombre}' tiene un código postal fuera del rango válido: {codigoPostal}.");
                return null;
            }

            if (codigoPostal.ToString().Length < 5 && !string.IsNullOrEmpty(codigoPostal.ToString()) )
            {
                Console.WriteLine($"Error: El codigo postal de '{xm.nombre}' no presenta las 5 cifras, se añadiran 0 a la izquierda para arreglarlo.");
            }

            Monumento monumento = new Monumento
            {
                Nombre = xm.nombre,
                Localidad = xm.poblacion.localidad,
                Provincia = xm.poblacion.provincia,
                Direccion = xm.calle + ", " + xm.codigoPostal + " " + xm.poblacion.localidad,
                CodigoPostal = codigoPostal,
                Tipo = MapTipo(xm.tipoMonumento, xm.nombre),  
                Latitud = latitud,
                Longitud = longitud,
                Descripcion = Regex.Replace(xm.Descripcion?.CDataSection.Replace("'","''"),"<.*?>", string.Empty),
            };

            return monumento;
        }


        private static Tipo MapTipo(string tipoMonumento, string nombre)
        {
            if (string.IsNullOrWhiteSpace(tipoMonumento))
            {
                return Tipo.Otros;
            }

            if (tipoMonumento.Contains("Yacimientos arqueológicos", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Yacimiento; // 1
            }

            if (tipoMonumento.Contains("Monasterios", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Monasterio", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Convento", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Monasterio; // 3
            }

            if (tipoMonumento.Contains("Iglesias", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Iglesia", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Ermita", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Iglesia; // 2
            }

            if (tipoMonumento.Contains("Castillos", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Castillo", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Torre", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Muralla", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Castillo; // 4
            }

            if (tipoMonumento.Contains("Puentes", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Puente", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Pont", StringComparison.OrdinalIgnoreCase))
            {
                return Tipo.Puente; // 6
            }

            return Tipo.Edificio; // 5
        }

    }
}
