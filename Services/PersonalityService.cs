using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AganBrowser.Models;

namespace AganBrowser.Services
{
    public class PersonalityService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        public List<PersonalityTrait> Traits { get; private set; } = new();

        public PersonalityService()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AganBrowser");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "personality_state.json");
            Load();
        }

        public void Load()
        {
            string defaultJsonPath = Path.Combine(AppContext.BaseDirectory, "Assets", "personality.json");
            
            if (File.Exists(defaultJsonPath))
            {
                try
                {
                    var json = File.ReadAllText(defaultJsonPath);
                    Traits = JsonSerializer.Deserialize<List<PersonalityTrait>>(json, _jsonOptions) ?? new List<PersonalityTrait>();
                }
                catch { }
            }

            if (Traits.Count == 0)
            {
                Traits = new List<PersonalityTrait>
                {
                    new() { Id = "esprili", Name = "😄 Esprili", Prompt = "Cevaplarında espri yap.", IsActive = false },
                    new() { Id = "resmi", Name = "🤵 Resmi", Prompt = "Ciddi ve resmi bir dil kullan.", IsActive = false }
                };
            }

            if (File.Exists(_filePath))
            {
                try
                {
                    var stateJson = File.ReadAllText(_filePath);
                    var savedStates = JsonSerializer.Deserialize<Dictionary<string, bool>>(stateJson, _jsonOptions);
                    if (savedStates != null)
                    {
                        foreach (var trait in Traits)
                        {
                            if (savedStates.TryGetValue(trait.Id, out bool isActive))
                            {
                                trait.IsActive = isActive;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        public void Save()
        {
            try
            {
                var states = Traits.ToDictionary(t => t.Id, t => t.IsActive);
                File.WriteAllText(_filePath, JsonSerializer.Serialize(states));
            }
            catch { }
        }

        public string GetActivePrompts()
        {
            var active = Traits.Where(t => t.IsActive).Select(t => t.Prompt).ToList();
            if (active.Count == 0) return "";
            return "Aktif kişilik özelliklerin şunlardır: " + string.Join(" ", active);
        }
    }
}