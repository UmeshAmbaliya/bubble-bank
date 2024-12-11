// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	public abstract class LevelGoal : ScriptableObject
	{
		public abstract bool IsComplete(GameState state);
		
#if UNITY_EDITOR
		public abstract void Draw();
#endif
	}
}
