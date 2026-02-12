
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using Firebase.Database;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;
using NUnit.Framework;


public class FirebaseSaveManager
{
    public static SaveData currentSaveData { get; private set; }

    // Save game to cloud
    public static async Task<bool> SaveGame(SaveData saveData)
    {
        if (References.Instance.firebaseManager.user == null)
        {
            Debug.LogError("No user logged in");
            return false;
        }

        try
        {
            string json = JsonConvert.SerializeObject(saveData);
            await References.Instance.firebaseManager.database
            .Child(USERS_PATH)
            .Child(References.Instance.firebaseManager.user.UserId)
            .Child(SAVE_DATA_PATH).SetRawJsonValueAsync(json);

            Debug.Log("Game saved to cloud");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            return false;
        }
    }

    // Load game from cloud
    public static async Task<bool> LoadGame()
    {
        if (References.Instance.firebaseManager.user == null)
        {
            Debug.LogError("No user logged in");
            return false;
        }

        try
        {
            var snapshot = await References.Instance.firebaseManager.database
                .Child(USERS_PATH)
                .Child(References.Instance.firebaseManager.user.UserId)
                .GetValueAsync();

            if (snapshot.Exists)
            {
                string json = snapshot.GetRawJsonValue();

                SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
                currentSaveData = data;

                Debug.Log("Game loaded from cloud");
                return true;
            }
            else
            {
                Debug.Log("No save data found");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            return false;
        }
    }

    public static void DeleteLocalSave()
    {
        if (File.Exists(SAVE_PATH))
        {
            File.Delete(SAVE_PATH);
        }
    }

    public static async Task<string> GetLastSavedData()
    {
        try
        {
            var snapshot = await GetUserRef().Child(SAVE_DATA_PATH).Child("lastSaved").GetValueAsync();
            string date = string.Empty;
            if (snapshot.Exists)
            {
                date = snapshot.GetRawJsonValue();
                //date = JsonUtility.FromJson<string>(json);

                Debug.Log($"✓ Loaded last saved data ({date})");
            }

            return date;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Get last saved data failed: {e.Message}");
            CallConnectionFailureEvent(e.Message);
            return string.Empty;
        }
    }

    #region User 

    #region OK

    public static async Task<bool> UpdateLastSavedData()
    {
        try
        {
            await GetUserRef()
                .Child(SAVE_DATA_PATH)
                .Child("lastSaved")
                .SetValueAsync(DateTime.UtcNow.ToString("o"));

            return true;

        }
        catch (Exception e)
        {
            Debug.LogWarning($"Update last saved failed: {e.Message}");
            return false;
        }
    }

    #endregion

    private static DatabaseReference GetUserRef()
    {
        string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;

        return References.Instance.firebaseManager.database
            .Child(USERS_PATH)
            .Child(userId);
    }

    public static async Task<string> GetUsername()
    {
        try
        {
            var snapshot = await GetUserRef().Child(SAVE_DATA_PATH).Child("username").GetValueAsync();
            return snapshot.Exists ? (string)snapshot.Value : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion

    #region Characters

    public static async Task<bool> UpdateEquipment(Character character, ItemSO itemSO)
    {
        try
        {
            string equipmentPath;

            switch (itemSO)
            {
                case ArmorSO:
                    equipmentPath = "equippedItems/armor";
                    break;
                case WeaponSO:
                    if (itemSO.itemType == ItemType.LightSword || itemSO.itemType == ItemType.Dagger)
                        equipmentPath = "equippedItems/shield";
                    else
                        equipmentPath = "equippedItems/weapon";
                    break;
                case HelmetSO:
                    equipmentPath = "equippedItems/helmet";
                    break;
                case ShieldSO:
                    equipmentPath = "equippedItems/shield";
                    break;
                case JewelrySO:
                    equipmentPath = "equippedItems/jewelry";
                    break;
                case WingSO:
                    equipmentPath = "equippedItems/wings";
                    break;

                default:
                    equipmentPath = string.Empty;
                    return false;
            }

            await GetUserRef()
                .Child(CHARACTERS_PATH)
                .Child(character.characterId)
                .Child(equipmentPath)
                .SetValueAsync(itemSO.saveableEntityId);

            return true;
        }

        catch (Exception e)
        {
            Debug.LogWarning($"Update habit failed: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> RemoveCharacter(string characterId)
    {
        try
        {
            await GetUserRef()
                .Child(CHARACTERS_PATH)
                .Child(characterId)
                .RemoveValueAsync();

            Debug.Log($"✓ Character {characterId} deleted");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Delete character failed: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> AddNewCharacter(Character character)
    {
        bool success = await UpdateLastSavedData();

        if (success)
        {
            try
            {
                string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;

                // Push creates a new child with auto-generated ID
                var characterRef = GetUserRef()
                    .Child(CHARACTERS_PATH)
                    .Push(); // This generates unique ID

                // Get the auto-generated ID
                string characterId = characterRef.Key;
                character.characterId = characterId; // Store ID in the character

                // Save the character
                string json = JsonConvert.SerializeObject(character);
                await characterRef.SetRawJsonValueAsync(json);

                Debug.Log($"✓ [FIREBASE] Character saved successfully with ID: {characterId}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Add character failed: {e.Message}");
                return false;
            }
        }

        else
        {
            return false;
        }
    }

    public static async Task<int> GetCharacterCount()
    {
        try
        {
            var snapshot = await GetUserRef().Child(CHARACTERS_PATH).GetValueAsync();
            return snapshot.Exists ? (int)snapshot.ChildrenCount : 0;
        }
        catch
        {
            return 0;
        }
    }

    public static async Task<List<Character>> GetAllCharacters()
    {
        try
        {
            var snapshot = await GetUserRef().Child(CHARACTERS_PATH).GetValueAsync();

            List<Character> characters = new List<Character>();

            if (snapshot.Exists)
            {
                // Iterate through each character
                foreach (var childSnapshot in snapshot.Children)
                {
                    string json = childSnapshot.GetRawJsonValue();
                    Character character = JsonConvert.DeserializeObject<Character>(json);
                    characters.Add(character);
                }
            }

            Debug.Log($"✓ Loaded {characters.Count} character(s)");
            return characters;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Get characters failed: {e.Message}");
            CallConnectionFailureEvent(e.Message);
            return new List<Character>();
        }
    }

    public async Task<Character> GetCharacter(string characterId)
    {
        try
        {
            var snapshot = await GetUserRef()
                .Child(CHARACTERS_PATH)
                .Child(characterId)
                .GetValueAsync();

            if (snapshot.Exists)
            {
                string json = snapshot.GetRawJsonValue();
                return JsonConvert.DeserializeObject<Character>(json);
            }

            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Get character failed: {e.Message}");
            return null;
        }
    }

    public static async Task<bool> UpdateCharacterField(string characterId, string fieldPath, object value)
    {
        bool success = await UpdateLastSavedData();

        if (!success)
            return false;

        try
        {
            await GetUserRef()
                .Child(CHARACTERS_PATH)
                .Child(characterId)
                .Child(fieldPath)
                .SetValueAsync(value);
            return true;

        }
        catch (Exception e)
        {
            Debug.LogWarning($"Update field failed: {e.Message}");
            return false;
        }
    }

    #endregion

    #region Dailies

    public static async Task<bool> AddNewDaily(DailyContent daily)
    {
        try
        {
            var success = await UpdateLastSavedData();

            if (success)
            {
                // Push creates a new child with auto-generated ID
                var dailyRef = GetUserRef()
                    .Child(DAILIES_PATH)
                    .Push(); // This generates unique ID
                             // Get the auto-generated ID
                string dailyId = dailyRef.Key;
                daily.id = dailyId; // Store ID in the character

                // Save the character
                string json = JsonConvert.SerializeObject(daily);
                await dailyRef.SetRawJsonValueAsync(json);

                Debug.Log($"✓ Daily {dailyId} added atomically");
                return true;
            }

            else
            {
                Debug.LogWarning($"Add daily failed!");
                return false;
            }

        }
        catch (Exception e)
        {
            Debug.LogWarning($"Add daily failed: {e.Message}");
            return false;
        }
    }

    #region OK

    public static async Task<bool> RemoveDaily(string dailyId)
    {
        try
        {
            // Validation
            if (string.IsNullOrEmpty(dailyId))
            {
                Debug.LogWarning("Daily ID is null or empty");
                return false;
            }

            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";

            string dailyPath = $"{basePath}/{DAILIES_PATH}/{dailyId}";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";

            var updates = new Dictionary<string, object>
            {
                [dailyPath] = null,  // ✅ Setting to null removes the entry
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);
            Debug.Log($"✓ Daily {dailyId} deleted");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Delete daily failed: {e.Message}");
            return (false);
        }
    }

    #endregion


    public static async Task<List<DailyContent>> GetAllDailies()
    {
        try
        {
            var snapshot = await GetUserRef().Child(DAILIES_PATH).GetValueAsync();

            List<DailyContent> dailies = new List<DailyContent>();

            if (snapshot.Exists)
            {
                // Iterate through each character
                foreach (var childSnapshot in snapshot.Children)
                {
                    string json = childSnapshot.GetRawJsonValue();
                    DailyContent daily = JsonConvert.DeserializeObject<DailyContent>(json);
                    dailies.Add(daily);
                }
            }

            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";

            var updates = new Dictionary<string, object>
            {
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            if (References.Instance.lastSavedDate.Date < DateTime.Now.Date)
            {
                // Add all completed dailies to the update
                foreach (var daily in dailies.Where(d => d.isCompleted))
                {
                    string dailyPath = $"{basePath}/{DAILIES_PATH}/{daily.id}/isCompleted";
                    updates[dailyPath] = false;

                    // Update local state immediately
                    daily.isCompleted = false;
                }

                // Only make the Firebase call if there are dailies to reset
                if (updates.Count > 1)  // More than just the timestamp
                {
                    try
                    {
                        await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);
                        Debug.Log($"✓ Reset {updates.Count - 1} completed dailies");
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Error reseting dailies: {e.Message}");
                    }

                }
            }

            var expiredUpdates = new Dictionary<string, object>();
            DateTime today = DateTime.Now.Date;

            foreach (var daily in dailies)
            {
                // Check if daily is one-time and already passed
                if (daily.repetition == (int)Repetition.Daily && daily.repeatEveryDays == 0)
                {
                    DateTime utcTime = DateTime.Parse(daily.startDate);
                    DateTime taskStart = utcTime.ToLocalTime().Date;

                    if (taskStart < today)
                    {
                        // Mark for deletion
                        string dailyPath = $"{basePath}/{DAILIES_PATH}/{daily.id}";
                        expiredUpdates[dailyPath + "/isActive"] = false;  // archived
                    }
                }
            }

            // Only make Firebase call if there are dailies to remove
            if (expiredUpdates.Count > 0)
            {
                try
                {
                    // Add timestamp
                    expiredUpdates[$"{basePath}/{SAVE_DATA_PATH}/lastSaved"] = DateTime.UtcNow.ToString("o");

                    await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(expiredUpdates);

                    Debug.Log($"✓ Updated {expiredUpdates.Count} expired dailies to archived status");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error reseting dailies: {e.Message}");
                }

            }
            return dailies;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Get dailies failed: {e.Message}");
            return new List<DailyContent>();
        }
    }


    public static async Task<bool> UpdateDailyOnMark(string dailyId, string fieldPath, object value, Character character, int xpReward, int coinReward, int gemReward, int overflowXp, int nextLevelXp = 0)
    {
        try
        {
            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";

            string coinId = References.Instance.inventory.coin.saveableEntityId;
            string gemId = References.Instance.inventory.gem.saveableEntityId;

            int currentCoins = character.inventory.ContainsKey(coinId) ? character.inventory[coinId] : 0;
            int currentGems = character.inventory.ContainsKey(gemId) ? character.inventory[gemId] : 0;

            // Build path strings manually
            string dailyPath = $"{basePath}/{DAILIES_PATH}/{dailyId}/";
            string inventoryPath = $"{basePath}/{CHARACTERS_PATH}/{character.characterId}/inventory/";
            string characterPath = $"{basePath}/{CHARACTERS_PATH}/{character.characterId}/";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";
            string experiencePath = $"{basePath}/{CHARACTERS_PATH}/{character.characterId}/statuses/6/";

            // Common updates
            var updates = new Dictionary<string, object>
            {
                [dailyPath + fieldPath] = value,
                [inventoryPath + coinId] = currentCoins + coinReward,
                [inventoryPath + gemId] = currentGems + gemReward,
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            bool didLevelUp = overflowXp >= 0 && nextLevelXp > 0 && xpReward > 0;
            bool didLevelDown = overflowXp >= 0 && nextLevelXp > 0 && xpReward < 0;

            if (didLevelUp)
            {
                // Level up updates
                updates[experiencePath + "currentValue"] = overflowXp;
                updates[experiencePath + "maxValue"] = nextLevelXp;
                updates[characterPath + "level"] = character.level + 1;

                // Calculate and update all stats
                var classStats = References.Instance.experienceConfigurations
                    .classDefinitions[character.avatarConfig.classesIndex].stats;

                for (int i = 0; i < 6; i++)
                {
                    int newStatValue = References.Instance.experienceConfigurations.StatAtLevel(
                        classStats[i].baseValue,
                        classStats[i].growth,
                        character.level + 1
                    );

                    updates[characterPath + $"statuses/{i}/currentValue"] = newStatValue;
                    updates[characterPath + $"statuses/{i}/maxValue"] = newStatValue;
                }

                updates[characterPath + $"statuses/{(int)Status.Energy}/maxValue"] = References.Instance.experienceConfigurations.GetEnergyForNextLevel(character.level + 1);
            }

            else if (didLevelDown)
            {
                // Level up updates
                updates[experiencePath + "currentValue"] = overflowXp;
                updates[experiencePath + "maxValue"] = nextLevelXp;
                updates[characterPath + "level"] = character.level - 1;

                // Calculate and update all stats
                var classStats = References.Instance.experienceConfigurations
                    .classDefinitions[character.avatarConfig.classesIndex].stats;

                for (int i = 0; i < 6; i++)
                {
                    int newStatValue = References.Instance.experienceConfigurations.StatAtLevel(
                        classStats[i].baseValue,
                        classStats[i].growth,
                        character.level - 1
                    );

                    updates[characterPath + $"statuses/{i}/currentValue"] = newStatValue;
                    updates[characterPath + $"statuses/{i}/maxValue"] = newStatValue;
                }

                updates[characterPath + $"statuses/{(int)Status.Energy}/maxValue"] = References.Instance.experienceConfigurations.GetEnergyForNextLevel(character.level - 1);
            }

            else
            {
                // Normal XP gain
                updates[experiencePath + "currentValue"] = character.statuses[6].currentValue + xpReward;
            }

            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);

            Debug.Log($"✓ Updated {fieldPath} for daily {dailyId}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Update daily field failed: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> EditDaily(string dailyId, DailyContent editedDaily)
    {
        try
        {
            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";
            string dailyPath = $"{basePath}/{DAILIES_PATH}/{dailyId}/";

            // Common updates
            var updates = new Dictionary<string, object>
            {
                [dailyPath + "difficulty"] = editedDaily.difficulty,
                [dailyPath + "notes"] = editedDaily.notes,
                [dailyPath + "title"] = editedDaily.title,
                [dailyPath + "startDate"] = editedDaily.startDate,
                [dailyPath + "repetition"] = editedDaily.repetition,
                [dailyPath + "repeatEveryDays"] = editedDaily.repeatEveryDays,
                [dailyPath + "isCompleted"] = editedDaily.isCompleted,
                [dailyPath + "daysOfWeek"] = editedDaily.daysOfWeek,
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);
            Debug.Log($"✓ Daily {dailyId} updated with sucess!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Update daily failed: {e.Message}");
            return false;
        }
    }

    #endregion

    #region Habits

    public static async Task<bool> AddNewHabit(HabitContent habit)
    {
        await UpdateLastSavedData();

        try
        {
            // Push creates a new child with auto-generated ID
            var habitRef = GetUserRef()
                .Child(HABITS_PATH)
                .Push(); // This generates unique ID

            // Get the auto-generated ID
            string habitId = habitRef.Key;
            habit.id = habitId; // Store ID in the character

            // Save the character
            string json = JsonConvert.SerializeObject(habit);
            await habitRef.SetRawJsonValueAsync(json);

            Debug.Log($"✓ [FIREBASE] Habit saved successfully with ID: {habitId}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Add Habit failed: {e.Message}");
            return false;
        }
    }

    public static async Task UpdateHabitField(string habitId, string fieldPath, object value)
    {
        await UpdateLastSavedData();

        try
        {
            await GetUserRef()
                .Child(HABITS_PATH)
                .Child(habitId)
                .Child(fieldPath)
                .SetValueAsync(value);

            Debug.Log($"✓ Updated {fieldPath} for habit {habitId}");
        }
        catch (Exception e)
        {
            CallConnectionFailureEvent(e.Message);
            Debug.LogWarning($"Update habit field failed: {e.Message}");
        }
    }

    public static async Task<List<HabitContent>> GetAllHabits()
    {
        await UpdateLastSavedData();

        try
        {
            var snapshot = await GetUserRef().Child(HABITS_PATH).GetValueAsync();

            List<HabitContent> habits = new List<HabitContent>();
            List<HabitContent> lostHabits = new List<HabitContent>();

            if (snapshot.Exists)
            {
                // Iterate through each character
                foreach (var childSnapshot in snapshot.Children)
                {
                    string json = childSnapshot.GetRawJsonValue();
                    HabitContent habit = JsonConvert.DeserializeObject<HabitContent>(json);
                    habits.Add(habit);
                }
            }

            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";

            DateTime today = DateTime.Now.Date;

            var updates = new Dictionary<string, object>
            {
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            //Pass one day check
            if (References.Instance.lastSavedDate.Date.Day < DateTime.Now.Date.Day)
            {
                foreach (var habit in habits)
                {
                    string isCompletedPath = $"{basePath}/{HABITS_PATH}/{habit.id}/isCompleted";
                    string lastMarkPath = $"{basePath}/{HABITS_PATH}/{habit.id}/lastMark";
                    string isActivePath = $"{basePath}/{HABITS_PATH}/{habit.id}/isActive";
                    string countPath = $"{basePath}/{HABITS_PATH}/{habit.id}/count";
                    int daysPassed = (today - DateTime.Parse(habit.lastPositiveCheck).ToLocalTime().Date).Days;

                    if ((Repetition)habit.repetition == Repetition.Daily && !habit.isCompleted)
                    {
                        updates[isCompletedPath] = false;
                        updates[lastMarkPath] = (int)MarkStatus.Neutral;
                        updates[isActivePath] = true;
                        habit.isCompleted = false;
                        habit.lastMark = (int)MarkStatus.Neutral;
                        habit.isActive = true;

                        if (daysPassed > 1)
                        {
                            updates[countPath] = 0;
                            habit.count = 0;
                            lostHabits.Add(habit);
                        }
                    }

                    else if ((Repetition)habit.repetition == Repetition.Weekly && !habit.isCompleted && DateTime.Now.Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        updates[isCompletedPath] = false;
                        updates[isActivePath] = true;
                        updates[lastMarkPath] = (int)MarkStatus.Neutral;
                        habit.isCompleted = false;
                        habit.lastMark = (int)MarkStatus.Neutral;
                        habit.isActive = true;

                        if (daysPassed > 8)
                        {
                            updates[countPath] = 0;
                            habit.count = 0;
                            lostHabits.Add(habit);
                        }
                    }

                    else if ((Repetition)habit.repetition == Repetition.Monthly && !habit.isCompleted && DateTime.Now.Date.Day == DateTime.DaysInMonth(DateTime.Now.Date.Year, DateTime.Now.Date.Month))
                    {
                        updates[isCompletedPath] = false;
                        updates[isActivePath] = true;
                        updates[lastMarkPath] = (int)MarkStatus.Neutral;
                        habit.isCompleted = false;
                        habit.lastMark = (int)MarkStatus.Neutral;
                        habit.isActive = true;

                        if (daysPassed > 32)
                        {
                            updates[countPath] = 0;
                            habit.count = 0;
                            lostHabits.Add(habit);
                        }
                    }

                    else if ((Repetition)habit.repetition == Repetition.Yearly && !habit.isCompleted && DateTime.Now.Date.Day == 31 && DateTime.Now.Date.Month == 12)
                    {
                        updates[isCompletedPath] = false;
                        updates[isActivePath] = true;
                        updates[lastMarkPath] = (int)MarkStatus.Neutral;
                        habit.isCompleted = false;
                        habit.lastMark = (int)MarkStatus.Neutral;
                        habit.isActive = true;

                        if (daysPassed > 366)
                        {
                            updates[countPath] = 0;
                            habit.count = 0;
                            lostHabits.Add(habit);
                        }
                    }

                    else if (!habit.isCompleted)
                    {
                        updates[isCompletedPath] = false;
                        updates[isActivePath] = false;
                        updates[lastMarkPath] = (int)MarkStatus.Neutral;
                        habit.isCompleted = false;
                        habit.lastMark = (int)MarkStatus.Neutral;
                        habit.isActive = false;
                    }
                }
            }


            else
            {
                foreach (var habit in habits)
                {
                    string isActivePath = $"{basePath}/{HABITS_PATH}/{habit.id}/isActive";

                    if ((Repetition)habit.repetition == Repetition.Daily && !habit.isCompleted)
                    {
                        updates[isActivePath] = true;
                        habit.isActive = true;
                    }

                    else if ((Repetition)habit.repetition == Repetition.Weekly && !habit.isCompleted && DateTime.Now.Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        updates[isActivePath] = true;
                        habit.isActive = true;
                    }

                    else if ((Repetition)habit.repetition == Repetition.Monthly && !habit.isCompleted && DateTime.Now.Date.Day == DateTime.DaysInMonth(DateTime.Now.Date.Year, DateTime.Now.Date.Month))
                    {
                        updates[isActivePath] = true;
                        habit.isActive = true;
                    }

                    else if ((Repetition)habit.repetition == Repetition.Yearly && !habit.isCompleted && DateTime.Now.Date.Day == 31 && DateTime.Now.Date.Month == 12)
                    {
                        updates[isActivePath] = true;
                        habit.isActive = true;
                    }

                    else if (!habit.isCompleted)
                    {
                        updates[isActivePath] = false;
                        habit.isActive = false;
                    }
                }
            }

            // Only make the Firebase call if there are dailies to reset
            if (updates.Count > 1)  // More than just the timestamp
            {
                try
                {

                    // Add timestamp
                    updates[$"{basePath}/{SAVE_DATA_PATH}/lastSaved"] = DateTime.UtcNow.ToString("o");
                    await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);
                    Debug.Log($"✓ Reset {updates.Count - 1} habits");
                }

                catch (Exception e)
                {
                    Debug.LogWarning($"Error reseting habits: {e.Message}");
                }
            }

            Debug.Log($"✓ Loaded {habits.Count} habits(s)");
            return habits;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Get habits failed: {e.Message}");
            return new List<HabitContent>();
        }
    }

    public static async Task<string> GetLastSavedHabitDate(HabitContent habitContent)
    {
        try
        {
            var snapshot = await GetUserRef().Child(HABITS_PATH).Child(habitContent.id).Child("lastPositiveCheck").GetValueAsync();
            string date = string.Empty;
            if (snapshot.Exists)
            {
                date = snapshot.GetRawJsonValue();
                //date = JsonUtility.FromJson<string>(json);

                Debug.Log($"✓ Loaded last saved data ({date})");
            }

            return date;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Get last saved data failed: {e.Message}");
            CallConnectionFailureEvent(e.Message);
            return string.Empty;
        }
    }

    public static async Task<(bool success, int value)> GetLastSavedHabitMark(HabitContent habitContent)
    {
        try
        {
            // ✅ Add validation
            if (habitContent == null)
            {
                Debug.LogError("HabitContent is null!");
                return (false, 0);
            }

            if (string.IsNullOrEmpty(habitContent.id))
            {
                Debug.LogError("Habit ID is null or empty!");
                return (false, 0);
            }

            // ✅ Log the full path to debug
            string fullPath = $"{GetUserRef().Child(HABITS_PATH).Child(habitContent.id).Child("lastMark").ToString()}";
            Debug.Log($"Attempting to read from path: {fullPath}");

            var snapshot = await GetUserRef().Child(HABITS_PATH).Child(habitContent.id).Child("lastMark").GetValueAsync();

            if (!snapshot.Exists || snapshot.Value == null)
                return (false, 0);

            int value = Convert.ToInt32(snapshot.Value);
            Debug.Log($"✓ Loaded last saved habit mark ({value})");
            return (true, value);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Get last saved data failed: {e.Message}");
            return (false, 0);
        }
    }

    public static async Task<bool> UpdateHabitOnMark(string habitId, HabitContent updatedHabit, Character character, int xpReward, int coinReward, int gemReward, int overflowXp, int nextLevelXp = 0)
    {
        try
        {
            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";

            string coinId = References.Instance.inventory.coin.saveableEntityId;
            string gemId = References.Instance.inventory.gem.saveableEntityId;

            int currentCoins = character.inventory.ContainsKey(coinId) ? character.inventory[coinId] : 0;
            int currentGems = character.inventory.ContainsKey(gemId) ? character.inventory[gemId] : 0;

            // Build path strings manually
            string habitPath = $"{basePath}/{HABITS_PATH}/{habitId}/";
            string inventoryPath = $"{basePath}/{CHARACTERS_PATH}/{character.characterId}/inventory/";
            string characterPath = $"{basePath}/{CHARACTERS_PATH}/{character.characterId}/";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";
            string experiencePath = $"{basePath}/{CHARACTERS_PATH}/{character.characterId}/statuses/6/";

            // Common updates
            var updates = new Dictionary<string, object>
            {
                [habitPath + "lastMark"] = updatedHabit.lastMark,
                [habitPath + "lastPositiveCheck"] = updatedHabit.lastPositiveCheck,
                [habitPath + "count"] = updatedHabit.count,
                [inventoryPath + coinId] = currentCoins + coinReward,
                [inventoryPath + gemId] = currentGems + gemReward,
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            bool didLevelUp = overflowXp >= 0 && nextLevelXp > 0 && xpReward > 0;
            bool didLevelDown = overflowXp >= 0 && nextLevelXp > 0 && xpReward < 0;

            if (didLevelUp)
            {
                // Level up updates
                updates[experiencePath + "currentValue"] = overflowXp;
                updates[experiencePath + "maxValue"] = nextLevelXp;
                updates[characterPath + "level"] = character.level + 1;

                // Calculate and update all stats
                var classStats = References.Instance.experienceConfigurations
                    .classDefinitions[character.avatarConfig.classesIndex].stats;

                for (int i = 0; i < 6; i++)
                {
                    int newStatValue = References.Instance.experienceConfigurations.StatAtLevel(
                        classStats[i].baseValue,
                        classStats[i].growth,
                        character.level + 1
                    );

                    updates[characterPath + $"statuses/{i}/currentValue"] = newStatValue;
                    updates[characterPath + $"statuses/{i}/maxValue"] = newStatValue;
                }

                updates[characterPath + $"statuses/{(int)Status.Energy}/maxValue"] = References.Instance.experienceConfigurations.GetEnergyForNextLevel(character.level + 1);
            }

            else if (didLevelDown)
            {
                // Level up updates
                updates[experiencePath + "currentValue"] = overflowXp;
                updates[experiencePath + "maxValue"] = nextLevelXp;
                updates[characterPath + "level"] = character.level - 1;

                // Calculate and update all stats
                var classStats = References.Instance.experienceConfigurations
                    .classDefinitions[character.avatarConfig.classesIndex].stats;

                for (int i = 0; i < 6; i++)
                {
                    int newStatValue = References.Instance.experienceConfigurations.StatAtLevel(
                        classStats[i].baseValue,
                        classStats[i].growth,
                        character.level - 1
                    );

                    updates[characterPath + $"statuses/{i}/currentValue"] = newStatValue;
                    updates[characterPath + $"statuses/{i}/maxValue"] = newStatValue;
                }

                updates[characterPath + $"statuses/{(int)Status.Energy}/maxValue"] = References.Instance.experienceConfigurations.GetEnergyForNextLevel(character.level - 1);
            }

            else
            {
                // Normal XP gain
                updates[experiencePath + "currentValue"] = character.statuses[6].currentValue + xpReward;
            }

            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);

            Debug.Log($"✓ Habit {habitId} updated");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Update habit failed: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> EditHabit(string habitId, HabitContent editedHabit)
    {
        try
        {
            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";
            string habitPath = $"{basePath}/{HABITS_PATH}/{habitId}/";

            // Common updates
            var updates = new Dictionary<string, object>
            {
                [habitPath + "difficulty"] = editedHabit.difficulty,
                [habitPath + "notes"] = editedHabit.notes,
                [habitPath + "title"] = editedHabit.title,
                [habitPath + "repetition"] = editedHabit.repetition,
                [habitPath + "isCompleted"] = editedHabit.isCompleted,
                [habitPath + "count"] = editedHabit.count,
                [habitPath + "isActive"] = editedHabit.isActive,
                [habitPath + "lastMark"] = editedHabit.lastMark,
                [habitPath + "lastPositiveCheck"] = editedHabit.lastPositiveCheck,
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);
            Debug.Log($"✓ Daily {habitId} updated with sucess!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Update daily failed: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> RemoveHabit(string habitId)
    {
        try
        {
            // Validation
            if (string.IsNullOrEmpty(habitId))
            {
                Debug.LogWarning("Habit ID is null or empty");
                return false;
            }

            string userId = References.Instance.firebaseManager.auth.CurrentUser.UserId;
            string basePath = $"{USERS_PATH}/{userId}";

            string habitPath = $"{basePath}/{HABITS_PATH}/{habitId}";
            string lastSavedPath = $"{basePath}/{SAVE_DATA_PATH}/lastSaved";

            var updates = new Dictionary<string, object>
            {
                [habitPath] = null,  // ✅ Setting to null removes the entry
                [lastSavedPath] = DateTime.UtcNow.ToString("o")
            };

            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);
            Debug.Log($"✓ Daily {habitId} deleted");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Delete daily failed: {e.Message}");
            return (false);
        }
    }
    #endregion

    #region Events

    private static void CallConnectionFailureEvent(string message) => Bus<ConnectionFailureEvent>.CallEvent(new ConnectionFailureEvent(message));

    #endregion

    #region Paths

    private static string SAVE_PATH => Application.persistentDataPath + "/data_saver.json";
    private static readonly string SAVE_DATA_PATH = "data";
    private static readonly string USERS_PATH = "users";
    private static readonly string CHARACTERS_PATH = "characters";
    private static readonly string DAILIES_PATH = "dailies";
    private static readonly string HABITS_PATH = "habits";

    #endregion

    #region Not in use

    public static async Task<int> GetDailiesCount()
    {
        try
        {
            var snapshot = await GetUserRef().Child(DAILIES_PATH).GetValueAsync();
            return snapshot.Exists ? (int)snapshot.ChildrenCount : 0;
        }
        catch
        {
            return 0;
        }
    }

    #endregion

}