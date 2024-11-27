using System;
using System.IO;

public static class Extractor_json
{
    public static string loadJsonAsString(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"El archivo JSON no existe en la ruta: {filePath}");
        }
        return File.ReadAllText(filePath);
    }
}
