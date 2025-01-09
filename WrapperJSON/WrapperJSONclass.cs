using System;
using System.IO;

namespace WrapperJSON
{
    class WrapperJSONclass
    {
        public static string LoadJSONAsString()
        {
            string filePath = "./edificios.json";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo JSON no existe en la ruta: {filePath}");
            }
            return File.ReadAllText(filePath);
        }
    }
}
