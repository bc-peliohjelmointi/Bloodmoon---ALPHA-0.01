using UnityEngine;

public class PauseMenuSaveLoad : MonoBehaviour
{
    public void SaveGame()
    {
        SaveSystem.Save();
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        SaveSystem.Load();
        Debug.Log("Game Loaded");
    }
}