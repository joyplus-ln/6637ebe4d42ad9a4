using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;

public class StartAppCommand : EventCommand {

	public override void Execute()
	{
		dispatcher.Dispatch(GameEvent.LOAD_SCENE, WorldLevel.Map.ToString());
	}
}
