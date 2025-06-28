using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;

namespace PROTHEUS.Models
{
    public static class ConfigManager
    {
        private const string Caminho = "config.json";

        public static List<CameraConfig> Carregar()
        {
            if (!File.Exists(Caminho))
                return new List<CameraConfig>();

            var json = File.ReadAllText(Caminho);
            return JsonSerializer.Deserialize<List<CameraConfig>>(json) ?? new();
        }

        public static void Salvar(List<CameraConfig> configuracoes)
        {
            var json = JsonSerializer.Serialize(configuracoes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Caminho, json);
        }
    }
}