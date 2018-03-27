using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;

public class GameStart : ContextView {

	void Awake()
	{
		//启动框架
		context = new GameContext(this);
	}
}
