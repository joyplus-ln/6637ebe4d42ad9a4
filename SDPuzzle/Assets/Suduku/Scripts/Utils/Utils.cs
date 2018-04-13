using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{
    public static Vector2 Vector3to2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    public static void GoToPlayScene(int level)
    {
        SetCurrentLevel(level);
        SceneManager.LoadSceneAsync(2);
    }

    public static void GoToMapScene()
    {
        SceneManager.LoadSceneAsync(1);
    }


    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    public static void SetCurrentLevel(int level)
    {
        PlayerPrefs.SetInt("CurrentLevel", level);
    }


    public static int GetMaxUnlockLevel()
    {
        return PlayerPrefs.GetInt("MaxUnlockLevel", 1);
    }
    public static void SetMaxUnlockLevel(int unlockLevels = 1)
    {
        PlayerPrefs.SetInt("MaxUnlockLevel", GetMaxUnlockLevel() + unlockLevels);
    }

}