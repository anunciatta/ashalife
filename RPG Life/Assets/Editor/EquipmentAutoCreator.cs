using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class EquipmentFromCsvImporter : EditorWindow
{
    private TextAsset csvFile;

    [MenuItem("Tools/Equipment/Import From CSV")]
    public static void Open()
    {
        GetWindow<EquipmentFromCsvImporter>("Equipment CSV Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Import Equipment From Localization CSV", EditorStyles.boldLabel);

        csvFile = (TextAsset)EditorGUILayout.ObjectField(
            "CSV File",
            csvFile,
            typeof(TextAsset),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Import"))
        {
            if (csvFile == null)
            {
                Debug.LogError("CSV file not assigned.");
                return;
            }

            ImportCsv(csvFile.text);
        }
    }

    void ImportCsv(string csvText)
    {
        var lines = csvText.Split('\n');

        // baseKey → labels
        Dictionary<string, EquipmentLabels> entries = new();

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split(',');
            if (parts.Length < 2)
                continue;

            string key = parts[0].Trim();

            if (key.EndsWith("Name"))
                Add(entries, key, LabelType.Name);
            else if (key.EndsWith("Description"))
                Add(entries, key, LabelType.Description);
            else if (key.EndsWith("Flavor"))
                Add(entries, key, LabelType.Flavor);
        }

        foreach (var kvp in entries)
        {
            CreateAsset(kvp.Key, kvp.Value);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✓ Imported {entries.Count} equipment items.");
    }

    void Add(Dictionary<string, EquipmentLabels> dict, string fullKey, LabelType type)
    {
        string baseKey = fullKey
            .Replace("Name", "")
            .Replace("Description", "")
            .Replace("Flavor", "");

        if (!dict.TryGetValue(baseKey, out var labels))
        {
            labels = new EquipmentLabels();
            dict[baseKey] = labels;
        }

        switch (type)
        {
            case LabelType.Name: labels.name = fullKey; break;
            case LabelType.Description: labels.description = fullKey; break;
            case LabelType.Flavor: labels.flavor = fullKey; break;
        }
    }

    void CreateAsset(string baseKey, EquipmentLabels labels)
    {
        EquipmentSO asset = DetectType(baseKey, out string folder);

        if (asset == null)
        {
            Debug.LogWarning($"⚠ Unknown equipment type: {baseKey}");
            return;
        }

        asset.nameLabel = labels.name;
        asset.descriptionLabel = labels.description;
        asset.flavorLabel = labels.flavor;

        string folderPath = $"Assets/GameData/Equipment/{folder}";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string assetPath = $"{folderPath}/{baseKey}.asset";

        if (File.Exists(assetPath))
        {
            Debug.Log($"↺ Skipped existing: {baseKey}");
            return;
        }

        AssetDatabase.CreateAsset(asset, assetPath);
    }

    EquipmentSO DetectType(string key, out string folder)
    {
        key = key.ToLower();

        if (key.Contains("staff") || key.Contains("sword") || key.Contains("bow") || key.Contains("dagger") || key.Contains("axe") || key.Contains("weapon"))
        {
            folder = "Weapons";
            return ScriptableObject.CreateInstance<WeaponSO>();
        }

        if (key.Contains("armor") || key.Contains("robe"))
        {
            folder = "Armors";
            return ScriptableObject.CreateInstance<ArmorSO>();
        }

        if (key.Contains("helmet") || key.Contains("hat") || key.Contains("hood"))
        {
            folder = "Helmets";
            return ScriptableObject.CreateInstance<HelmetSO>();
        }

        if (key.Contains("shield"))
        {
            folder = "Shields";
            return ScriptableObject.CreateInstance<ShieldSO>();
        }

        folder = null;
        return null;
    }

    enum LabelType { Name, Description, Flavor }

    class EquipmentLabels
    {
        public string name;
        public string description;
        public string flavor;
    }
}
