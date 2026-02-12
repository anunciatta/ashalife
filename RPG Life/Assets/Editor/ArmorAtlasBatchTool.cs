#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ArmorAtlasBatchTool : EditorWindow
{
    private DefaultAsset armorFolder;
    private DefaultAsset helmetFolder;
    private List<HelmetSO> foundHelmets = new();
    private Texture2D atlasTexture;
    private Texture2D inventoryIconAtlas;

    static readonly int[] MandatoryIndices =
{
    0, // Body
    1, // LA
    2, // RA
    7, // LL
    8  // RL
};

    private List<ArmorSO> foundArmors = new();
    private Vector2 scroll;

    [MenuItem("Tools/Equipment/Armor Atlas Batch Tool")]
    public static void Open()
    {
        GetWindow<ArmorAtlasBatchTool>("Armor Atlas Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Armor Atlas Batch Processor", EditorStyles.boldLabel);
        GUILayout.Space(8);

        armorFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Armor Folder",
            armorFolder,
            typeof(DefaultAsset),
            false
        );

        atlasTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Sprite Atlas Texture",
            atlasTexture,
            typeof(Texture2D),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Scan Folder For ArmorSO"))
        {
            ScanForArmors();
        }

        GUILayout.Space(10);
        DrawArmorList();

        GUILayout.Space(10);

        GUI.enabled = foundArmors.Count > 0 && atlasTexture != null;

        if (GUILayout.Button("Auto-Fill Sprites For All Armors"))
        {
            ProcessAllArmors();
        }

        GUILayout.Space(20);
        GUILayout.Label("Helmet Processing", EditorStyles.boldLabel);

        helmetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Helmet Folder",
            helmetFolder,
            typeof(DefaultAsset),
            false
        );

        if (GUILayout.Button("Scan Folder For HelmetSO"))
        {
            ScanForHelmets();
        }

        DrawHelmetList();

        GUI.enabled = foundHelmets.Count > 0 && atlasTexture != null;

        if (GUILayout.Button("Auto-Fill Helmets From Atlas"))
        {
            ProcessAllHelmets();
        }

        GUILayout.Space(20);
        GUILayout.Label("Inventory Icons", EditorStyles.boldLabel);

        inventoryIconAtlas = (Texture2D)EditorGUILayout.ObjectField(
            "Inventory Icon Atlas",
            inventoryIconAtlas,
            typeof(Texture2D),
            false
        );

        GUI.enabled =
            inventoryIconAtlas != null &&
            (foundArmors.Count > 0 || foundHelmets.Count > 0);

        if (GUILayout.Button("Auto-Fill Inventory Icons (Armor + Helmet)"))
        {
            ProcessInventoryIcons();
        }

        GUI.enabled = true;

    }

    private void ProcessInventoryIcons()
    {
        string atlasPath = AssetDatabase.GetAssetPath(inventoryIconAtlas);

        var allSprites = AssetDatabase
            .LoadAllAssetsAtPath(atlasPath)
            .OfType<Sprite>()
            .ToList();

        if (allSprites.Count == 0)
        {
            Debug.LogError("No sprites found in Inventory Icon Atlas.");
            return;
        }

        // Build lookup table (case-insensitive)
        var spriteMap = allSprites.ToDictionary(
            s => s.name.ToLowerInvariant(),
            s => s
        );

        AssignArmorIcons(spriteMap);
        AssignHelmetIcons(spriteMap);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Inventory icons assigned.");
    }

    private void AssignArmorIcons(Dictionary<string, Sprite> spriteMap)
    {
        foreach (var armor in foundArmors)
        {
            string key = armor.name.ToLowerInvariant();

            if (spriteMap.TryGetValue(key, out var sprite))
            {
                armor.inventoryIcon = sprite;
                EditorUtility.SetDirty(armor);
            }
            else
            {
                Debug.LogWarning(
                    $"{armor.name}: Inventory icon not found ({armor.name})"
                );
            }
        }
    }

    private void AssignHelmetIcons(Dictionary<string, Sprite> spriteMap)
    {
        foreach (var helmet in foundHelmets)
        {
            string key = helmet.name.ToLowerInvariant();

            if (spriteMap.TryGetValue(key, out var sprite))
            {
                helmet.inventoryIcon = sprite;
                EditorUtility.SetDirty(helmet);
            }
            else
            {
                Debug.LogWarning(
                    $"{helmet.name}: Inventory icon not found ({helmet.name})"
                );
            }
        }
    }




    // ------------------------------------
    private void ScanForHelmets()
    {
        foundHelmets.Clear();

        if (helmetFolder == null)
        {
            Debug.LogError("Helmet folder not assigned.");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(helmetFolder);
        string[] guids = AssetDatabase.FindAssets("t:HelmetSO", new[] { folderPath });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var helmet = AssetDatabase.LoadAssetAtPath<HelmetSO>(path);
            if (helmet != null)
                foundHelmets.Add(helmet);
        }

        Debug.Log($"Found {foundHelmets.Count} HelmetSO assets.");
    }

    private void DrawHelmetList()
    {
        foreach (var helmet in foundHelmets)
        {
            EditorGUILayout.ObjectField(helmet, typeof(HelmetSO), false);
        }
    }

    private void AutoFillHelmetSprite(HelmetSO helmet, List<Sprite> allSprites)
    {
        if (helmet.helmetSpriteConfig == null)
        {
            Debug.LogError($"{helmet.name}: HelmetSpriteConfig is null.");
            return;
        }

        string setPrefix = helmet.name
            .Replace("Helmet", "")
            .Trim()
            .ToLowerInvariant();

        string expectedName = $"{setPrefix}_helmet";

        Sprite foundSprite = allSprites.FirstOrDefault(
            s => s.name.ToLowerInvariant() == expectedName
        );

        if (foundSprite == null)
        {
            Debug.LogError(
                $"{helmet.name}: ❌ Helmet sprite not found ({expectedName})"
            );
            return;
        }

        helmet.helmetSpriteConfig.sprite = foundSprite;
        EditorUtility.SetDirty(helmet);

        Debug.Log($"{helmet.name}: ✅ Helmet sprite assigned.");
    }


    private void ProcessAllHelmets()
    {
        string atlasPath = AssetDatabase.GetAssetPath(atlasTexture);

        var allSprites = AssetDatabase
            .LoadAllAssetsAtPath(atlasPath)
            .OfType<Sprite>()
            .ToList();

        if (allSprites.Count == 0)
        {
            Debug.LogError("No sprites found inside atlas texture.");
            return;
        }

        foreach (var helmet in foundHelmets)
        {
            AutoFillHelmetSprite(helmet, allSprites);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("All HelmetSO assets processed successfully.");
    }


    private void ScanForArmors()
    {
        foundArmors.Clear();

        if (armorFolder == null)
        {
            Debug.LogError("Armor folder not assigned.");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(armorFolder);

        string[] guids = AssetDatabase.FindAssets("t:ArmorSO", new[] { folderPath });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var armor = AssetDatabase.LoadAssetAtPath<ArmorSO>(path);
            if (armor != null)
                foundArmors.Add(armor);
        }

        Debug.Log($"Found {foundArmors.Count} ArmorSO assets.");
    }

    private void DrawArmorList()
    {
        GUILayout.Label("Found Armors", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(200));

        foreach (var armor in foundArmors)
        {
            EditorGUILayout.ObjectField(armor, typeof(ArmorSO), false);
        }

        EditorGUILayout.EndScrollView();
    }

    private void ProcessAllArmors()
    {
        string atlasPath = AssetDatabase.GetAssetPath(atlasTexture);

        var allSprites = AssetDatabase
            .LoadAllAssetsAtPath(atlasPath)
            .OfType<Sprite>()
            .ToList();

        if (allSprites.Count == 0)
        {
            Debug.LogError("No sprites found inside atlas texture.");
            return;
        }

        foreach (var armor in foundArmors)
        {
            AutoFillArmorSprites(armor, allSprites);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("All ArmorSO assets processed successfully.");
    }

    private void AutoFillArmorSprites(ArmorSO armor, List<Sprite> allSprites)
    {
        armor.armorSprites.sprites.Clear();

        string setPrefix = armor.name
    .Replace("Armor", "")
    .Trim()
    .ToLowerInvariant();

        var spriteMap = allSprites
         .Where(s => s.name.ToLowerInvariant().StartsWith(setPrefix + "_"))
         .ToDictionary(
             s => s.name.ToLowerInvariant(),
             s => s
         );

        string[] orderedNames =
        {
    $"{setPrefix}_body",
    $"{setPrefix}_la",
    $"{setPrefix}_ra",
    $"{setPrefix}_ls",
    $"{setPrefix}_rs",
    $"{setPrefix}_lh",
    $"{setPrefix}_rh",
    $"{setPrefix}_ll",
    $"{setPrefix}_rl"
};

        List<string> missing = new();

        foreach (var spriteName in orderedNames)
        {
            if (spriteMap.TryGetValue(spriteName, out var sprite))
            {
                armor.armorSprites.sprites.Add(sprite);
            }
            else
            {
                armor.armorSprites.sprites.Add(null);
                missing.Add(spriteName);
            }
        }

        if (armor.armorSprites.sprites.Count != 9)
        {
            Debug.LogError($"{armor.name}: Sprite list incomplete ({armor.armorSprites.sprites.Count}/9)");
        }

        List<string> criticalMissing = new();

        if (armor.armorSprites.sprites.Count != 9)
        {
            Debug.LogError($"{armor.name}: Sprite list size mismatch ({armor.armorSprites.sprites.Count}/9)");
            return;
        }

        if (armor.armorSprites.sprites[0] == null) criticalMissing.Add("Body");
        if (armor.armorSprites.sprites[1] == null) criticalMissing.Add("Left Arm (LA)");
        if (armor.armorSprites.sprites[2] == null) criticalMissing.Add("Right Arm (RA)");
        if (armor.armorSprites.sprites[7] == null) criticalMissing.Add("Left Leg (LL)");
        if (armor.armorSprites.sprites[8] == null) criticalMissing.Add("Right Leg (RL)");

        if (criticalMissing.Count > 0)
        {
            Debug.LogError(
                $"{armor.name}: ❌ Missing mandatory sprites:\n- " +
                string.Join("\n- ", criticalMissing)
            );
        }
        else
        {
            Debug.Log($"{armor.name}: ✅ Mandatory sprites validated.");
        }

        EditorUtility.SetDirty(armor);
    }
}
#endif
