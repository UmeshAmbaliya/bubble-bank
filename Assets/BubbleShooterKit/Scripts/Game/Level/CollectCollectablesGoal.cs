// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace BubbleShooterKit
{
	public class CollectCollectablesGoal : LevelGoal
	{
		public CollectableBubbleType Type;
		public int Amount;

		public override bool IsComplete(GameState state)
		{
			return state.CollectedCollectables[Type] >= Amount;
		}

		public override string ToString()
		{
			return $"Collect {Amount} collectables";
		}

#if UNITY_EDITOR
		public override void Draw()
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Type");
			Type = (CollectableBubbleType) EditorGUILayout.EnumPopup(Type, GUILayout.Width(100));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Amount");
			Amount = EditorGUILayout.IntField(Amount, GUILayout.Width(30));
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}
#endif
	}
}
