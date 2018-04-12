using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModel : ILevel {

	private int m_UnlockLevel;
	#region ILevel implementation
	void UnlockLevel (int level)
	{
		m_UnlockLevel = level; 
		Utils.SetInt (PrefKeys.UnlockLevel, level);
	}

	int unLocklevel 
	{
		get 
		{
			return m_UnlockLevel;
		}
	}

	#endregion



}
