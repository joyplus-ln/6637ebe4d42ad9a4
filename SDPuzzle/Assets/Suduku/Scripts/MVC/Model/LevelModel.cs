using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModel : ILevel {

	public LevelModel()
	{
		m_UnlockLevel = Utils.GetInt(PrefKeys.UnlockLevel, 1);
		maxlevel = 50;
	}

	private int m_UnlockLevel;
	#region ILevel implementation

	/// <summary>
	/// 关卡进度加1.
	/// </summary>
	public void UnlockLevel()
	{
		++m_UnlockLevel;
		Utils.SetInt (PrefKeys.UnlockLevel, m_UnlockLevel);
	}

	/// <summary>
	/// 新解锁了关卡.
	/// </summary>
	/// <param name="level">Level.</param>
	public void UnlockLevel (int level)
	{
		m_UnlockLevel = level; 
		Utils.SetInt (PrefKeys.UnlockLevel, level);
	}


	/// <summary>
	/// 获取当前解锁的关卡.
	/// </summary>
	/// <value>The unlocklevel.</value>
	public int unlocklevel 
	{
		get 
		{
			return m_UnlockLevel;
		}
	}

	public int currentlevel { get; set;}
	public int maxlevel { get; set;}
	#endregion



}
