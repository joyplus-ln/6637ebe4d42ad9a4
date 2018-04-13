using System;

public interface ILevel
{
	int unlocklevel{ get;}
	int currentlevel { get; set;}
	int maxlevel { get; set;}
	void UnlockLevel();
	void UnlockLevel(int level);

}

