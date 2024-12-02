using IEIPracticas.APIs_Scrapper;
using IEIPracticas.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IEIPracticas.Mappers
{
    public static class CSVMapper
    {
        public static async Task<Monumento> CSVMonumentoToMonumento(CSVMonumento cm)
        {
            HereGeocodingService api = new HereGeocodingService();
            string latitudStr = cm.UTMESTE.Replace(',', '.');
            string longitudStr = cm.UTMNORTE.Replace(',', '.');
            string direccion = "";
            string codigoPostal = "";

            if (cm == null)
            {
                Console.WriteLine("Error: El objeto XMLMonumento no puede ser nulo.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(cm.DENOMINACION))
            {
                Console.WriteLine("Error: Monumento sin nombre.");
                return null;
            }

            if (!double.TryParse(latitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double latitud) || latitud < -90 || latitud > 90)
            {
                Console.WriteLine($"Error: Monumento '{cm.DENOMINACION}' tiene una latitud inválida.");
                return null;
            }

            if (!double.TryParse(longitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double longitud) || longitud < -180 || longitud > 180)
            {
                Console.WriteLine($"Error: Monumento '{cm.DENOMINACION}' tiene una longitud inválida.");
                return null;
            }

            Console.WriteLine($"Se procederá al intento de generación de dirección y codigo postal para el monumento '{cm.DENOMINACION}'");
            direccion = await api.GetAddressFromCoordinates(latitud, longitud);
            codigoPostal = await api.GetPostalCodeFromCoordinates(latitud, longitud);

            if (string.IsNullOrEmpty(direccion))
            {
                Console.WriteLine($"Error: No se pudo generar dirección para '{cm.DENOMINACION}'.");
                return null;
            }
            else Console.WriteLine("Exito: Direccion generada.");

            if (string.IsNullOrEmpty(codigoPostal))
            {
                Console.WriteLine($"Error: No se pudo generar codigo postal para '{cm.DENOMINACION}'.");
                return null;
            }
            else Console.WriteLine("Exito: Codigo postal generado.");

            if (codigoPostal.ToString().Length < 5 && !string.IsNullOrEmpty(codigoPostal.ToString()))
            {
                Console.WriteLine($"Error: El codigo postal de '{cm.DENOMINACION}' no presenta las 5 cifras, se añadiran 0 a la izquierda para arreglarlo.");
            }

            Monumento monumento = new Monumento
            {
                Nombre = cm.DENOMINACION.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim(),
                Localidad = cm.MUNICIPIO,
                Provincia = cm.PROVINCIA,
                Direccion = direccion,
                CodigoPostal = int.Parse(codigoPostal),
                Tipo = MapTipo(cm.CATEGORIA, cm.DENOMINACION),
                Latitud = latitud,
                Longitud = longitud,
                Descripcion = $"Monumento '{cm.DENOMINACION}' situado en '{cm.MUNICIPIO}' de tipo '{MapTipo(cm.CATEGORIA, cm.DENOMINACION)}'."
            };

            return monumento;
        }

        private static Tipo MapTipo(string categoria, string denominacion)
        {
            if (categoria == "Zona arqueológica")
                return Tipo.Yacimiento; // 1

            if (categoria == "Individual (mueble)" || categoria == "Zona paleontológica" ||
                categoria == "Fondo de museo (primera)" || string.IsNullOrEmpty(categoria))
                return Tipo.Otros; // 7

            if (categoria == "Monumento" && (denominacion.Contains("Iglesia") || denominacion.Contains("Ermita")))
                return Tipo.Iglesia; // 2

            if (categoria == "Monumento" && (denominacion.Contains("Monasterio") || denominacion.Contains("Convento")))
                return Tipo.Monasterio; // 3

            if (categoria == "Monumento" &&
                (denominacion.Contains("Castillo") || denominacion.Contains("Castell") || denominacion.Contains("Castellet") ||
                 denominacion.Contains("Torre") || denominacion.Contains("Torreta") || denominacion.Contains("Muralla") ||
                 denominacion.Contains("Amurallado") || denominacion.Contains("Amurallada")))
                return Tipo.Castillo; // 4

            if (categoria == "Monumento" && (denominacion.Contains("Puente") || denominacion.Contains("Pont")))
                return Tipo.Puente; // 6


            return Tipo.Edificio; // 5
        }

        private static double ConvertToDouble(string value)
        {
            if (double.TryParse(value, out var result))
                return result;

            return 0.0; // Valor por defecto
        }
    }
}
