using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{
    public static Vector2 Vector3to2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }
		
	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	/// <summary>
	/// Removes key and its corresponding value from the preferences.
	/// </summary>
	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	/// <summary>
	/// Removes all keys and values from the preferences. Use with caution.
	/// </summary>
	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	/// <summary>
	/// Writes all modified preferences to disk.
	/// </summary>
	public static void Save()
	{
		PlayerPrefs.Save();
	}


	public static void SetInt(string key, int val)
	{
		PlayerPrefs.SetInt(key, val);
	}

	/// <summary>
	/// Sets the value of the preference identified by key.
	/// </summary>
	public static void SetLong(string key, long val)
	{
		SetString(key, val.ToString());
	}

	/// <summary>
	/// Sets the value of the preference identified by key.
	/// </summary>
	public static void SetString(string key, string val)
	{
		PlayerPrefs.SetString(key, val);
	}

	/// <summary>
	/// Sets the value of the preference identified by key.
	/// </summary>
	public static void SetFloat(string key, float val)
	{
		SetString(key, val.ToString());
	}

	public static void SetBool(string key, bool value)
	{
		SetInt(key, value ? 1 : 0);
	}


	public static int GetInt(string key, int defaultValue)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}

		return PlayerPrefs.GetInt(key);
	}

	public static int GetInt(string key)
	{
		return GetInt(key, 0);
	}

	public static long GetLong(string key, long defaultValue)
	{
		return long.Parse(GetString(key, defaultValue.ToString()));
	}

	public static long GetLong(string key)
	{
		return GetLong(key, 0);
	}

	public static string GetString(string key, string defaultValue)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}

		return PlayerPrefs.GetString (key);
	}

	public static string GetString(string key)
	{
		return GetString(key, "");
	}

	public static float GetFloat(string key, float defaultValue)
	{
		return float.Parse(GetString(key, defaultValue.ToString()));
	}

	public static float GetFloat(string key)
	{
		return GetFloat(key, 0);
	}

	public static bool GetBool(string key, bool defaultValue = false)
	{
		if (!HasKey(key))
			return defaultValue;

		return GetInt(key) == 1;
	}

}