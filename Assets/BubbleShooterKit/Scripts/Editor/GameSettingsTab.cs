// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BubbleShooterKit
{
	/// <summary>
	/// The 'Game settings' tab in the editor.
	/// </summary>
	public class GameSettingsTab : EditorTab
	{
		private Object gameConfigurationDbObj;
		private GameConfiguration gameConfig;

		private int selectedTabIndex;
		private Vector2 scrollPos;
		
        private ReorderableList iapItemsList;
        private IapItem currentIapItem;
		
		private int newLevel;
		
		public GameSettingsTab(BubbleShooterKitEditor editor) : base(editor)
		{
			var path = EditorPrefs.GetString("GameConfigurationPath");
			if (!string.IsNullOrEmpty(path))
			{
				gameConfigurationDbObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameConfiguration));
				gameConfig = (GameConfiguration)gameConfigurationDbObj;
                CreateIapItemsList();
			}
			
            newLevel = PlayerPrefs.GetInt("next_level");
		}

		public override void Draw()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            GUILayout.Space(15);

			var oldDb = gameConfigurationDbObj;
			gameConfigurationDbObj = EditorGUILayout.ObjectField("Asset", gameConfigurationDbObj, typeof(GameConfiguration), false, GUILayout.Width(340));
			if (gameConfigurationDbObj != oldDb)
			{
				gameConfig = (GameConfiguration)gameConfigurationDbObj;
                CreateIapItemsList();
				EditorPrefs.SetString("GameConfigurationPath", AssetDatabase.GetAssetPath(gameConfigurationDbObj));
			}

			if (gameConfig != null)
			{
				GUILayout.Space(15);

				var prevSelectedIndex = selectedTabIndex;
				selectedTabIndex = GUILayout.Toolbar(selectedTabIndex,
					new[] {"Game", "Monetization", "Player preferences"}, GUILayout.Width(500));

				if (selectedTabIndex != prevSelectedIndex)
					GUI.FocusControl(null);

				if (selectedTabIndex == 0)
					DrawGameTab();
				else if (selectedTabIndex == 1)
					DrawMonetizationTab();
				else
					DrawPreferencesTab();
			}

			EditorGUIUtility.labelWidth = oldLabelWidth;
			EditorGUILayout.EndScrollView();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(gameConfig);
			}
		}

		private void CreateIapItemsList()
		{
            iapItemsList = SetupReorderableList("In-app purchase items", gameConfig.IapItems,
                ref currentIapItem, (rect, x) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 350, EditorGUIUtility.singleLineHeight), x.StoreId);
                },
                (x) =>
                {
                    currentIapItem = x;
                },
                () =>
                {
                    var newItem = new IapItem();
                    gameConfig.IapItems.Add(newItem);
                },
                (x) =>
                {
                    currentIapItem = null;
                });
		}

		private void DrawGameTab()
		{
			DrawScoreSettings();
			GUILayout.Space(15);
			DrawLivesSettings();
			GUILayout.Space(15);
			DrawCoinsSettings();
			GUILayout.Space(15);
            DrawInGameBoosterSettings();
			GUILayout.Space(15);
			DrawContinueGameSettings();
		}

		private void DrawMonetizationTab()
		{
			DrawRewardedAdSettings();
			GUILayout.Space(15);
			DrawIapSettings();
		}

		private void DrawPreferencesTab()
		{
			DrawPreferencesSettings();
		}

		private void DrawScoreSettings()
		{
            EditorGUILayout.LabelField("Score", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            const string helpText =
                "The score given to the player when a bubble explodes.";
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Default score", "The default score of bubbles."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            gameConfig.DefaultBubbleScore = EditorGUILayout.IntField(gameConfig.DefaultBubbleScore, GUILayout.Width(70));
            GUILayout.EndHorizontal();
		}

		private void DrawLivesSettings()
		{
			EditorGUILayout.LabelField("Lives", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal(GUILayout.Width(300));
			EditorGUILayout.HelpBox(
				"The settings related to the lives system.", MessageType.Info);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Max lives",
					"The maximum number of lives that the player can have."),
				GUILayout.Width(EditorGUIUtility.labelWidth));
			gameConfig.MaxLives = EditorGUILayout.IntField(gameConfig.MaxLives, GUILayout.Width(30));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Time to next life",
					"The number of seconds that need to pass before the player is given a free life."),
				GUILayout.Width(EditorGUIUtility.labelWidth));
			gameConfig.TimeToNextLife = EditorGUILayout.IntField(gameConfig.TimeToNextLife, GUILayout.Width(70));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Refill cost",
					"The cost in coins of refilling the lives of the player up to its maximum number."),
				GUILayout.Width(EditorGUIUtility.labelWidth));
			gameConfig.LivesRefillCost = EditorGUILayout.IntField(gameConfig.LivesRefillCost, GUILayout.Width(70));
			GUILayout.EndHorizontal();
		}
		
        private void DrawCoinsSettings()
        {
            EditorGUILayout.LabelField("Coins", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox(
                "The settings related to the coins system.", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Initial coins",
                    "The initial number of coins given to the player."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            gameConfig.InitialCoins = EditorGUILayout.IntField(gameConfig.InitialCoins, GUILayout.Width(70));
            GUILayout.EndHorizontal();
        }

		private void DrawInGameBoosterSettings()
		{
            EditorGUILayout.LabelField("In-game boosters", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox(
                "The settings related to the boosters that can be purchased by the player in-game.", MessageType.Info);
            GUILayout.EndHorizontal();

			var oldLabelWidth = EditorGUIUtility.labelWidth;
			DrawInGameBooster("Super aim", ref gameConfig.SuperAimBoosterAmount, ref gameConfig.SuperAimBoosterPrice);
			DrawInGameBooster("Rainbow bubble", ref gameConfig.RainbowBubbleBoosterAmount, ref gameConfig.RainbowBubbleBoosterPrice);
			DrawInGameBooster("Horizontal bomb", ref gameConfig.HorizontalBombBoosterAmount, ref gameConfig.HorizontalBombBoosterPrice);
			DrawInGameBooster("Circle bomb", ref gameConfig.CircleBombBoosterAmount, ref gameConfig.CircleBombBoosterPrice);
			EditorGUIUtility.labelWidth = oldLabelWidth;
		}

		private void DrawInGameBooster(string boosterName, ref int boosterAmount, ref int boosterPrice)
		{
			EditorGUIUtility.labelWidth = 150;

			GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel($"{boosterName} amount");
			boosterAmount = EditorGUILayout.IntField(boosterAmount, GUILayout.Width(30));

			GUILayout.Space(15);
			
			EditorGUIUtility.labelWidth = 140;

			EditorGUILayout.PrefixLabel($"{boosterName} price");
			boosterPrice = EditorGUILayout.IntField(boosterPrice, GUILayout.Width(70));
			GUILayout.EndHorizontal();
		}
		
        private void DrawContinueGameSettings()
        {
            EditorGUILayout.LabelField("Continue game", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox(
                "The settings related to the options given to the player when losing a game.", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Extra bubbles",
                    "The number of extra bubbles that can be purchased by the player when a game is lost."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            gameConfig.NumExtraBubbles = EditorGUILayout.IntField(gameConfig.NumExtraBubbles, GUILayout.Width(30));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Bubbles cost",
                    "The cost in coins of the extra bubbles."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            gameConfig.ExtraBubblesCost = EditorGUILayout.IntField(gameConfig.ExtraBubblesCost, GUILayout.Width(70));
            GUILayout.EndHorizontal();
        }
		
		private void DrawRewardedAdSettings()
		{
            EditorGUILayout.LabelField("Rewarded ad", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            const string helpText =
                "The number of coins given to the player after watching a rewarded ad.";
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Coins", "The number of coins awarded to the player after watching a rewarded ad."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            gameConfig.RewardedAdCoins =
                EditorGUILayout.IntField(gameConfig.RewardedAdCoins, GUILayout.Width(70));
            GUILayout.EndHorizontal();
		}

        private void DrawIapSettings()
        {
            EditorGUILayout.LabelField("In-app purchases", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            const string helpText =
                "The settings of your in-game purchasable items.";
            EditorGUILayout.HelpBox(helpText, MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(350));
	        iapItemsList?.DoLayoutList();
	        GUILayout.EndVertical();

            if (currentIapItem != null)
                DrawIapItem(currentIapItem);

            GUILayout.EndHorizontal();
        }
		
		private void DrawPreferencesSettings()
		{
            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
		    GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Level", "The current level number."),
                GUILayout.Width(50));
            newLevel = EditorGUILayout.IntField(newLevel, GUILayout.Width(50));
		    GUILayout.EndHorizontal();

		    if (GUILayout.Button("Set progress", GUILayout.Width(120), GUILayout.Height(30)))
		        PlayerPrefs.SetInt("next_level", newLevel);

		    GUILayout.Space(15);

            EditorGUILayout.LabelField("PlayerPrefs", EditorStyles.boldLabel);
		    if (GUILayout.Button("Delete PlayerPrefs", GUILayout.Width(120), GUILayout.Height(30)))
		        PlayerPrefs.DeleteAll();

		    GUILayout.Space(15);

            EditorGUILayout.LabelField("EditorPrefs", EditorStyles.boldLabel);
		    if (GUILayout.Button("Delete EditorPrefs", GUILayout.Width(120), GUILayout.Height(30)))
		        EditorPrefs.DeleteAll();
		}
		
        private void DrawIapItem(IapItem item)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            item.Draw();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
	}
}
