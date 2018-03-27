using System;
using strange.extensions.command.impl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneCommand : EventCommand
{
	public override void Execute()
	{
		string filepath = evt.data as string;

		//Load the component
		if (String.IsNullOrEmpty(filepath))
		{
			throw new Exception("Can't load a module with a null or empty filepath.");
		}

		SceneManager.LoadSceneAsync (filepath);
	}
}

