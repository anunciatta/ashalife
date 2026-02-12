public class SaveSystem
{
    /*
    private static string SavePath => Application.persistentDataPath + "mbt_data_saver";

    // Current save data - accessible from anywhere
    public static SaveData CurrentSaveData;

    /// Initialize the save system (call this once at game start)
    public static void Initialize()
    {
        // Load existing save or create new one
        if (!LoadGame())
        {
            References.Instance.ResetStartConfigurations();
            Debug.Log("No save file found, starting with new data");
        }
        else
        {
            References.Instance.LoadGame(CurrentSaveData);
        }
    }

    /// Check if save file exists
    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    /// Delete save file
    public static bool DeleteSave()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Delete save failed: {e.Message}");
            return false;
        }
    }

    public static bool SaveGame()
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavePath, FileMode.Create);

            if (CurrentSaveData == null)
            {
                SaveData data = new SaveData(References.Instance.player);
                CurrentSaveData = data;
                formatter.Serialize(stream, data);
            }

            else
            {
                CurrentSaveData.UpdateData(References.Instance.player);
                formatter.Serialize(stream, CurrentSaveData);
            }

            stream.Close();

            //Debug.Log($"Game saved securely to: {SavePath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Save failed: {e.Message}");
            return false;
        }
    }

    public static bool LoadGame()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                return false;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavePath, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            CurrentSaveData = data;

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            return false;
        }
    }
    */
}
