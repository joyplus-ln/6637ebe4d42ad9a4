using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModel : ILevel {
	#region ILevel implementation

	void ILevel.UnlockLevel (int level)
	{
		throw new System.NotImplementedException ();
	}

	int ILevel.unLocklevel {
		get {
			throw new System.NotImplementedException ();
		}
	}

	#endregion



}
