using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ClassicUO.Assets;
using ClassicUO.Utility.Logging;

namespace ClassicUO.Configuration;

public class ClientClilocs
{
    
    public static void LoadClientClilocs()
    {
        var toLoad = ConfigurationResolver.Load<DebugClilocs>("Clilocs.json");
        foreach (var cliloc in toLoad.clilocs)
        {
            ClilocLoader.Instance.SetString(cliloc.id, cliloc.data);
        }
    }
    
    private static class ConfigurationResolver
    {
        public static T Load<T>(string file) where T : class
        {
            if (!File.Exists(file))
            {
                Log.Warn(file + " not found.");

                return null;
            }

            var text = File.ReadAllText(file);

            text = Regex.Replace
            (
                text, @"(?<!\\)  # lookbehind: Check that previous character isn't a \
                                            \\         # match a \
                                            (?!\\)     # lookahead: Check that the following character isn't a \", @"\\", RegexOptions.IgnorePatternWhitespace
            );

            return JsonSerializer.Deserialize(text, typeof(T), DebugClilocsJsonContext.Default) as T;
        }
    }

    public class DebugClilocs
    {
        public List<ClilocEntries> clilocs { get; set; }
    }

    public class ClilocEntries
    {
        public int id { get; set; }
        public string data { get; set; }
    }
}


[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(ClientClilocs.DebugClilocs), GenerationMode = JsonSourceGenerationMode.Metadata)]
sealed partial class DebugClilocsJsonContext : JsonSerializerContext
{
    public static DebugClilocsJsonContext RealDefault { get; } = new DebugClilocsJsonContext
    (
        new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }
    );
}