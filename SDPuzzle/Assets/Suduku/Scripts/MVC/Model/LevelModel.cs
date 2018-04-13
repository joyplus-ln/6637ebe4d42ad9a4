using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModel : ILevel {

	public LevelModel()
	{
		m_UnlockLevel = Utils.GetInt(PrefKeys.UnlockLevel, 1);
	}

	private int m_UnlockLevel;
	#region ILevel implementation
	public void UnlockLevel (int level)
	{
		m_UnlockLevel = level; 
		Utils.SetInt (PrefKeys.UnlockLevel, level);
	}

	public int unlocklevel 
	{
		get 
		{
			return m_UnlockLevel;
		}
	}

	#endregion



}
