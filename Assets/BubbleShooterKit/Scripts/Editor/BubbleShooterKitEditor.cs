// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// The custom editor window for Bubble Shooter Kit.
	/// </summary>
	public class BubbleShooterKitEditor : EditorWindow
	{
		private readonly List<EditorTab> tabs = new List<EditorTab>();

		private int selectedTabIndex = -1;
		private int prevSelectedTabIndex = -1;

		[MenuItem("Tools/Bubble Shooter Kit/Editor", false, 0)]
		private static void Init()
		{
			var window = GetWindow(typeof(BubbleShooterKitEditor));
			window.titleContent = new GUIContent("Bubble Shooter Kit Editor");
		}

		private void OnEnable()
		{
			tabs.Add(new GameSettingsTab(this));
			tabs.Add(new LevelEditorTab(this));
			tabs.Add(new AboutTab(this));
			selectedTabIndex = 0;
		}

		private void OnGUI()
		{
			selectedTabIndex = GUILayout.Toolbar(selectedTabIndex,
				new[] {"Game settings", "Level editor", "About"});
			if (selectedTabIndex >= 0 && selectedTabIndex < tabs.Count)
			{
				var selectedEditor = tabs[selectedTabIndex];
				if (selectedTabIndex != prevSelectedTabIndex)
				{
					selectedEditor.OnTabSelected();
					GUI.FocusControl(null);
				}
				selectedEditor.Draw();
				prevSelectedTabIndex = selectedTabIndex;
			}
		}
	}
}
