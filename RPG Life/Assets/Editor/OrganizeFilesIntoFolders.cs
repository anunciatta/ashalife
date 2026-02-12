using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class OrganizeFilesIntoFolders : EditorWindow
{
    [MenuItem("Tools/Organize Files Into Folders")]
    public static void ShowWindow()
    {
        GetWindow<OrganizeFilesIntoFolders>("Organize Files");
    }

    private void OnGUI()
    {
        GUILayout.Label("Organize Files Into Individual Folders", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        string currentFolder = GetSelectedFolderPath();
        
        EditorGUILayout.LabelField("Current Selected Folder:", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(currentFolder, GUILayout.Height(20));
        
        EditorGUILayout.HelpBox(
            "Select a folder in the Project window, then click the button below to organize its files.\n\n" +
            "If a file with the same name already exists in the destination folder, it will be renamed to 'InventoryIcon'.",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        GUI.enabled = !string.IsNullOrEmpty(currentFolder) && AssetDatabase.IsValidFolder(currentFolder);
        
        if (GUILayout.Button("Organize Files in Selected Folder", GUILayout.Height(40)))
        {
            OrganizeFiles(currentFolder);
        }
        
        GUI.enabled = true;
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    private string GetSelectedFolderPath()
    {
        if (Selection.activeObject != null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            
            if (AssetDatabase.IsValidFolder(path))
            {
                return path;
            }
            else if (!string.IsNullOrEmpty(path))
            {
                return Path.GetDirectoryName(path).Replace("\\", "/");
            }
        }
        
        return "";
    }

    private void OrganizeFiles(string folderPath)
    {
        string[] allGuids = AssetDatabase.FindAssets("", new[] { folderPath });
        
        var filesToMove = allGuids
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => !AssetDatabase.IsValidFolder(path))
            .Where(path => Path.GetDirectoryName(path).Replace("\\", "/") == folderPath)
            .ToList();

        if (filesToMove.Count == 0)
        {
            EditorUtility.DisplayDialog("No Files Found", 
                "No files found directly in the selected folder.", 
                "OK");
            return;
        }

        bool proceed = EditorUtility.DisplayDialog(
            "Organize Files",
            $"Found {filesToMove.Count} file(s) in:\n{folderPath}\n\nEach file will be moved into its own subfolder.\n\nContinue?",
            "Yes",
            "Cancel"
        );

        if (!proceed) return;

        int movedCount = 0;
        int renamedCount = 0;
        int errorCount = 0;

        // STEP 1: Create all folders first
        Dictionary<string, string> fileToFolderMap = new Dictionary<string, string>();
        
        foreach (string assetPath in filesToMove)
        {
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string newFolderPath = $"{folderPath}/{fileName}";
            
            fileToFolderMap[assetPath] = newFolderPath;
            
            if (!AssetDatabase.IsValidFolder(newFolderPath))
            {
                string guid = AssetDatabase.CreateFolder(folderPath, fileName);
                
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogError($"Failed to create folder: {newFolderPath}");
                }
                else
                {
                    Debug.Log($"Created folder: {newFolderPath}");
                }
            }
        }

        // Refresh to register new folders
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // STEP 2: Move the files
        AssetDatabase.StartAssetEditing();
        
        try
        {
            foreach (string assetPath in filesToMove)
            {
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                string fileExtension = Path.GetExtension(assetPath);
                string newFolderPath = fileToFolderMap[assetPath];
                string newAssetPath = $"{newFolderPath}/{fileName}{fileExtension}";
                
                // Check if destination already exists
                if (File.Exists(newAssetPath))
                {
                    // File already exists, rename to "InventoryIcon"
                    newAssetPath = $"{newFolderPath}/InventoryIcon{fileExtension}";
                    
                    // If InventoryIcon also exists, add a number
                    int counter = 1;
                    while (File.Exists(newAssetPath))
                    {
                        newAssetPath = $"{newFolderPath}/InventoryIcon{counter}{fileExtension}";
                        counter++;
                    }
                    
                    Debug.Log($"Destination exists. Renaming to: {Path.GetFileName(newAssetPath)}");
                    renamedCount++;
                }
                
                Debug.Log($"Moving: {assetPath} -> {newAssetPath}");
                
                string error = AssetDatabase.MoveAsset(assetPath, newAssetPath);
                
                if (string.IsNullOrEmpty(error))
                {
                    movedCount++;
                    Debug.Log($"✓ Successfully moved: {fileName}");
                }
                else
                {
                    Debug.LogError($"✗ Error moving {fileName}: {error}");
                    errorCount++;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        string message = $"Successfully moved {movedCount} file(s)!";
        if (renamedCount > 0)
        {
            message += $"\n{renamedCount} file(s) renamed to 'InventoryIcon' to avoid conflicts.";
        }
        if (errorCount > 0)
        {
            message += $"\n\n{errorCount} error(s) occurred. Check the Console for details.";
        }

        EditorUtility.DisplayDialog("Complete", message, "OK");
    }
}
