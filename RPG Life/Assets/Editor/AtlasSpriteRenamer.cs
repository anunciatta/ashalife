#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public class AtlasSpriteRenamer : EditorWindow
{
    private Texture2D atlasTexture;
    private bool isFourColumnAtlas = true;

    private static readonly string[] SetNames =
    {
        "Apprentice",
        "Student",
        "Initiate",
        "Adept",
        "Channeler",
        "Arcanist",
        "Spellbinder",
        "Magus",
        "Sorcerer",
        "Enchanter",
        "Battlemage",
        "Ritualist",
        "Archmage",
        "Spelllord",
        "Voidcaller",
        "Astral",
        "MythicSage",
        "Worldweaver",
        "EternalArcanist",
        "AscendedArchmage"
    };

    [MenuItem("Tools/Atlas/Sprite Renamer")]
    public static void Open()
    {
        GetWindow<AtlasSpriteRenamer>("Atlas Sprite Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Atlas Sprite Renamer", EditorStyles.boldLabel);
        GUILayout.Space(8);

        atlasTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Atlas Texture",
            atlasTexture,
            typeof(Texture2D),
            false
        );

        isFourColumnAtlas = EditorGUILayout.Toggle(
            "4-Column Atlas (Armor/Hat/Staff/Sword)",
            isFourColumnAtlas
        );

        GUILayout.Space(10);

        GUI.enabled = atlasTexture != null;

        if (GUILayout.Button("Rename Sprites"))
        {
            RenameSprites();
        }

        GUI.enabled = true;
    }

    // -------------------------------------------------

    private void RenameSprites()
    {
        string path = AssetDatabase.GetAssetPath(atlasTexture);

        var sprites = AssetDatabase
            .LoadAllAssetsAtPath(path)
            .OfType<Sprite>()
            .ToList();

        if (sprites.Count == 0)
        {
            Debug.LogError("No sprites found in atlas.");
            return;
        }

        int columns = isFourColumnAtlas ? 4 : 2;
        int expectedCount = SetNames.Length * columns;

        if (sprites.Count != expectedCount)
        {
            Debug.LogError(
                $"Sprite count mismatch. Found {sprites.Count}, expected {expectedCount}."
            );
            return;
        }

        SerializedObject textureImporter =
            new SerializedObject(AssetImporter.GetAtPath(path));
        SerializedProperty spriteSheet =
            textureImporter.FindProperty("m_SpriteSheet.m_Sprites");

        int index = 0;

        for (int row = 0; row < SetNames.Length; row++)
        {
            string setName = SetNames[row];

            for (int col = 0; col < columns; col++)
            {
                string newName = GetSpriteName(setName, col);
                spriteSheet.GetArrayElementAtIndex(index)
                    .FindPropertyRelative("m_Name")
                    .stringValue = newName;

                index++;
            }
        }

        textureImporter.ApplyModifiedProperties();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Debug.Log("Sprites renamed successfully.");
    }

    private string GetSpriteName(string setName, int column)
    {
        if (isFourColumnAtlas)
        {
            return column switch
            {
                0 => $"{setName}Armor",
                1 => $"{setName}Hat",
                2 => $"{setName}Staff",
                3 => $"{setName}LightSword",
                _ => setName
            };
        }
        else
        {
            return column switch
            {
                0 => $"{setName}Staff",
                1 => $"{setName}LightSword",
                _ => setName
            };
        }
    }
}
#endif
