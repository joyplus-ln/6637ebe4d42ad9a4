using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;

public class StartSdkCommand : EventCommand {

	public override void Execute()
	{
		//在这处理各个平台sdk初始化逻辑


		#if UNITY_EDITOR 
		#elif (UNITY_IPHONE || UNITY_IOS) 
		IronSource.Agent.init ("6f8940fd");	
		#elif UNITY_ANDROID
		IronSource.Agent.init ("6f32bb75");
		#endif
		IronSource.Agent.validateIntegration();
	}
}


