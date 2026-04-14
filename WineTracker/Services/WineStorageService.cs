using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WineTracker.Models;

namespace WineTracker.Services
{
    public class WineStorageService : IWineStorageService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WineTracker",
            "wines.json");

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public IEnumerable<Wine> LoadAll()
        {
            if (!File.Exists(FilePath))
                return Array.Empty<Wine>();

            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<Wine>>(json, JsonOptions) ?? new List<Wine>();
            }
            catch
            {
                return Array.Empty<Wine>();
            }
        }

        public void Save(IEnumerable<Wine> wines)
        {
            string? dir = Path.GetDirectoryName(FilePath);
            if (dir != null)
                Directory.CreateDirectory(dir);

            string json = JsonSerializer.Serialize(wines, JsonOptions);
            File.WriteAllText(FilePath, json);
        }
    }
}
