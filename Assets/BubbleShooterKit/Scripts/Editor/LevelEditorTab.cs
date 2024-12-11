// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BubbleShooterKit
{
	/// <summary>
	/// The 'Level editor' tab in the editor.
	/// </summary>
	public class LevelEditorTab : EditorTab
	{
		private Object levelDbObj;
		private LevelInfo currentLevelInfo;

		private Vector2 scrollPos;

		private const float LevelTileButtonSize = 40f;

		private enum BrushType
		{
			Bubble,
			RandomBubble,
			Booster,
			Blocker,
			Cover,
			Collectable,
			Empty
		}

		private BrushType currentBrushType;
		private ColorBubbleType currentColorBubbleType;
		private RandomBubbleType currentRandomBubbleType;
		private BoosterBubbleType currentBoosterBubbleType;
		private BlockerBubbleType currentBlockerBubbleType;
		private CoverType currentCoverType;
		private CollectableBubbleType currentCollectableBubbleType;

		private enum BrushMode
		{
			Tile,
			Row,
			Column,
			Fill
		}

		private BrushMode currentBrushMode;
		
		private readonly Dictionary<string, Texture> tileTextures = new Dictionary<string, Texture>();

		private int prevRows;

		private ReorderableList availableColorsList;
		private ColorBubbleType currentColor;

		private ReorderableList goalList;
		private LevelGoal currentGoal;
		
		public LevelEditorTab(BubbleShooterKitEditor editor) : base(editor)
		{
			var editorImagesPath = new DirectoryInfo(Application.dataPath + "/BubbleShooterKit/Editor/Resources");
            var fileInfo = editorImagesPath.GetFiles("*.png", SearchOption.TopDirectoryOnly);
            foreach (var file in fileInfo)
            {
                var filename = Path.GetFileNameWithoutExtension(file.Name);
                tileTextures[filename] = Resources.Load(filename) as Texture;
            }
		}

		public override void Draw()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;

            GUILayout.Space(15);

            DrawMenu();

            if (currentLevelInfo != null)
            {
                GUILayout.Space(15);
	            
	            GUILayout.BeginHorizontal();

	            DrawLevel();
	            
	            GUILayout.BeginVertical();
	            DrawAvailableColors();
	            DrawGoals();
	            DrawAvailableBoosters();
	            GUILayout.EndVertical();
	            
	            GUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUILayout.EndScrollView();

			if (GUI.changed)
				EditorUtility.SetDirty(currentLevelInfo);
		}
		
		private void DrawMenu()
        {
			var oldDb = levelDbObj;
			levelDbObj = EditorGUILayout.ObjectField("Asset", levelDbObj, typeof(LevelInfo), false, GUILayout.Width(340));
			if (levelDbObj != oldDb)
			{
				currentLevelInfo = (LevelInfo)levelDbObj;
				if (currentLevelInfo.AvailableColors == null)
					currentLevelInfo.Initialize();
				currentLevelInfo.Columns = 10;
				CreateAvailableColorsList();
				CreateGoalsList();
			}
        }

		private void DrawLevel()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(500));
			
			var style = new GUIStyle
			{
				fontSize = 20,
				fontStyle = FontStyle.Bold,
				normal = { textColor = Color.white }
			};
			EditorGUILayout.LabelField("General", style);
			
			GUILayout.Space(10);
			
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox(
                "The general settings of this level.",
                MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Level number", "The number of this level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevelInfo.Number = EditorGUILayout.IntField(currentLevelInfo.Number, GUILayout.Width(30));
            GUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
			prevRows = currentLevelInfo.Rows;
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Rows", "The number of rows of this level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevelInfo.Rows = EditorGUILayout.IntField(currentLevelInfo.Rows, GUILayout.Width(30));
            GUILayout.EndHorizontal();

			GUILayout.Space(10);
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Bubbles", "The number of bubbles that can be used in this level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevelInfo.NumBubbles = EditorGUILayout.IntField(currentLevelInfo.NumBubbles, GUILayout.Width(30));
            GUILayout.EndHorizontal();
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Star 1 score", "The number of points needed to obtain the first star."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevelInfo.Star1Score = EditorGUILayout.IntField(currentLevelInfo.Star1Score, GUILayout.Width(70));
            GUILayout.EndHorizontal();
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Star 2 score", "The number of points needed to obtain the second star."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevelInfo.Star2Score = EditorGUILayout.IntField(currentLevelInfo.Star2Score, GUILayout.Width(70));
            GUILayout.EndHorizontal();
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Star 3 score", "The number of points needed to obtain the third star."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevelInfo.Star3Score = EditorGUILayout.IntField(currentLevelInfo.Star3Score, GUILayout.Width(70));
            GUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Background", "The sprite to use for the game background."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentLevelInfo.BackgroundSprite = (Sprite)EditorGUILayout.ObjectField(currentLevelInfo.BackgroundSprite, typeof(Sprite), false, GUILayout.Width(150));
            GUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Brush type", "The type of brush to paint the level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentBrushType = (BrushType)EditorGUILayout.EnumPopup(currentBrushType, GUILayout.Width(100));
            GUILayout.EndHorizontal();

			switch (currentBrushType)
			{
				case BrushType.Bubble:
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Bubble type", "The bubble type to use."),
						GUILayout.Width(EditorGUIUtility.labelWidth));
					currentColorBubbleType = (ColorBubbleType)EditorGUILayout.EnumPopup(currentColorBubbleType, GUILayout.Width(100));
					GUILayout.EndHorizontal();
					currentCoverType = CoverType.None;
					break;
				}

				case BrushType.RandomBubble:
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Random bubble type", "The random bubble type to use."),
						GUILayout.Width(EditorGUIUtility.labelWidth));
					currentRandomBubbleType = (RandomBubbleType)EditorGUILayout.EnumPopup(currentRandomBubbleType, GUILayout.Width(100));
					GUILayout.EndHorizontal();
					currentCoverType = CoverType.None;
					break;
				}
				
				case BrushType.Booster:
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Booster type", "The booster type to use."),
						GUILayout.Width(EditorGUIUtility.labelWidth));
					currentBoosterBubbleType = (BoosterBubbleType)EditorGUILayout.EnumPopup(currentBoosterBubbleType, GUILayout.Width(100));
					GUILayout.EndHorizontal();
					currentCoverType = CoverType.None;
					break;
				}
				
				case BrushType.Blocker:
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Blocker type", "The blocker type to use."),
						GUILayout.Width(EditorGUIUtility.labelWidth));
					currentBlockerBubbleType = (BlockerBubbleType)EditorGUILayout.EnumPopup(currentBlockerBubbleType, GUILayout.Width(100));
					GUILayout.EndHorizontal();
					break;
				}
				
				case BrushType.Cover:
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Cover type", "The cover type to use."),
						GUILayout.Width(EditorGUIUtility.labelWidth));
					currentCoverType = (CoverType)EditorGUILayout.EnumPopup(currentCoverType, GUILayout.Width(100));
					GUILayout.EndHorizontal();
					break;
				}
				
				case BrushType.Collectable:
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Collectable type", "The collectable type to use."),
						GUILayout.Width(EditorGUIUtility.labelWidth));
					currentCollectableBubbleType = (CollectableBubbleType)EditorGUILayout.EnumPopup(currentCollectableBubbleType, GUILayout.Width(100));
					GUILayout.EndHorizontal();
					break;
				}
			}
			
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Brush mode", "The brush mode to paint the level."),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            currentBrushMode = (BrushMode)EditorGUILayout.EnumPopup(currentBrushMode, GUILayout.Width(100));
            GUILayout.EndHorizontal();

			if (GUILayout.Button("Randomize", GUILayout.Width(100)))
				RandomizeLevel();
			
			GUILayout.Space(10);

			if (currentLevelInfo.Rows != prevRows)
			{
				if (currentLevelInfo.Tiles != null)
				{
					foreach (var row in currentLevelInfo.Tiles)
						foreach (var tile in row.Tiles)
							Object.DestroyImmediate(tile, true);
					
					foreach (var row in currentLevelInfo.Tiles)
						Object.DestroyImmediate(row, true);
				}

				if (currentLevelInfo.Rows > 0 && currentLevelInfo.Columns > 0)
				{
					currentLevelInfo.Tiles = new List<LevelRow>();
					var evenWidth = currentLevelInfo.Columns;
					var oddWidth = currentLevelInfo.Columns - 1;
					for (var i = 0; i < currentLevelInfo.Rows; i++)
					{
						if (i % 2 == 0)
						{
							var row = ScriptableObject.CreateInstance<LevelRow>();
							row.hideFlags = HideFlags.HideInHierarchy;
							AssetDatabase.AddObjectToAsset(row, currentLevelInfo);
							row.Tiles = new List<TileInfo>(evenWidth);
							row.Tiles.AddRange(Enumerable.Repeat<TileInfo>(null, evenWidth));
							currentLevelInfo.Tiles.Add(row);
						}
						else
						{
							var row = ScriptableObject.CreateInstance<LevelRow>();
							row.hideFlags = HideFlags.HideInHierarchy;
							AssetDatabase.AddObjectToAsset(row, currentLevelInfo);
							row.Tiles = new List<TileInfo>(oddWidth);
							row.Tiles.AddRange(Enumerable.Repeat<TileInfo>(null, oddWidth));
							currentLevelInfo.Tiles.Add(row);
						}
					}
				}
			}

			if (currentLevelInfo.Tiles != null)
			{
				GUILayout.BeginVertical();
				for (var i = 0; i < currentLevelInfo.Rows; i++)
				{
					GUILayout.BeginHorizontal();
					if (i % 2 == 1) GUILayout.Space(LevelTileButtonSize * 0.5f);
					var row = currentLevelInfo.Tiles[i];
					for (var j = 0; j < row.Tiles.Count; j++)
					{
						CreateButton(i, j);
					}

					GUILayout.EndHorizontal();
				}

				GUILayout.EndVertical();
			}

			GUILayout.EndVertical();
		}

		private void CreateGoalsList()
		{
            goalList = SetupReorderableList("Goals", currentLevelInfo.Goals, ref currentGoal, (rect, x) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                        x.ToString());
                },
                (x) =>
                {
                    currentGoal = x;
                },
                () =>
                {
                    var menu = new GenericMenu();
	                menu.AddItem(new GUIContent("Collect bubbles"), false, CreateGoalCallback, typeof(CollectBubblesGoal));
	                menu.AddItem(new GUIContent("Collect random bubbles"), false, CreateGoalCallback, typeof(CollectRandomBubblesGoal));
	                menu.AddItem(new GUIContent("Collect collectables"), false, CreateGoalCallback, typeof(CollectCollectablesGoal));
	                menu.AddItem(new GUIContent("Collect leaves"), false, CreateGoalCallback, typeof(CollectLeavesGoal));
                    menu.ShowAsContext();
                },
                (x) =>
                {
	                Object.DestroyImmediate(currentGoal, true);
                    currentGoal = null;
                });
		}

		private void DrawGoals()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300));
			
			var style = new GUIStyle
			{
				fontSize = 20,
				fontStyle = FontStyle.Bold,
				normal = { textColor = Color.white }
			};
			EditorGUILayout.LabelField("Goals", style);
			
			GUILayout.Space(10);
			
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox(
                "This list defines the goals needed to be achieved by the player in order to complete this level.",
                MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(300));
			goalList?.DoLayoutList();
			GUILayout.EndVertical();

            if (currentGoal != null)
                DrawGoal(currentGoal);

            GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
		}

		private void DrawGoal(LevelGoal goal)
		{
			var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;

            goal.Draw();

            EditorGUIUtility.labelWidth = oldLabelWidth;
		}
		
		private void CreateGoalCallback(object obj)
		{
			var goal = ScriptableObject.CreateInstance(((Type) obj).Name) as LevelGoal;
			if (goal != null)
			{
				goal.hideFlags = HideFlags.HideInHierarchy;
				currentLevelInfo.Goals.Add(goal);
				AssetDatabase.AddObjectToAsset(goal, currentLevelInfo);
			}
		}
		
		private void CreateAvailableColorsList()
		{
            availableColorsList = SetupReorderableList("Available colors", currentLevelInfo.AvailableColors, ref currentColor, (rect, x) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                        x.ToString());
                },
                (x) => { currentColor = x; },
                () =>
                {
					var menu = new GenericMenu();
	                foreach (var color in Enum.GetValues(typeof(ColorBubbleType)))
	                {
		                var isUsed = currentLevelInfo.AvailableColors.Contains((ColorBubbleType)color);
		                if (isUsed)
			                menu.AddDisabledItem(new GUIContent(color.ToString()));
		                else
							menu.AddItem(new GUIContent(color.ToString()), false, CreateColorCallback, color);
	                }
					menu.ShowAsContext();
                },
                (x) => { currentColor = ColorBubbleType.Black; });
		}
		
		private void CreateColorCallback(object obj)
        {
            currentLevelInfo.AvailableColors.Add((ColorBubbleType)obj);
        }
		
		private void DrawAvailableColors()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300));
			
			var style = new GUIStyle
			{
				fontSize = 20,
				fontStyle = FontStyle.Bold,
				normal = { textColor = Color.white }
			};
			EditorGUILayout.LabelField("Available colors", style);
			
			GUILayout.Space(10);
			
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox(
                "This list defines the available bubble colors in this level.",
                MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(300));
			availableColorsList?.DoLayoutList();
			GUILayout.EndVertical();

            GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
		}

		private void DrawAvailableBoosters()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300));
			
			var style = new GUIStyle
			{
				fontSize = 20,
				fontStyle = FontStyle.Bold,
				normal = { textColor = Color.white }
			};
			EditorGUILayout.LabelField("Available boosters", style);
			
			GUILayout.Space(10);
			
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            EditorGUILayout.HelpBox(
                "Here you can specify the available purchasable boosters for this level.",
                MessageType.Info);
            GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Super aim", "Super aim."),
				GUILayout.Width(EditorGUIUtility.labelWidth));
			currentLevelInfo.IsSuperAimAvailable = EditorGUILayout.Toggle(currentLevelInfo.IsSuperAimAvailable);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Rainbow bomb", "Rainbow bomb."),
				GUILayout.Width(EditorGUIUtility.labelWidth));
			currentLevelInfo.IsRainbowBombAvailable = EditorGUILayout.Toggle(currentLevelInfo.IsRainbowBombAvailable);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Horizontal bomb", "Horizontal bomb."),
				GUILayout.Width(EditorGUIUtility.labelWidth));
			currentLevelInfo.IsHorizontalBombAvailable = EditorGUILayout.Toggle(currentLevelInfo.IsHorizontalBombAvailable);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Circle bomb", "Circle bomb."),
				GUILayout.Width(EditorGUIUtility.labelWidth));
			currentLevelInfo.IsCircleBombAvailable = EditorGUILayout.Toggle(currentLevelInfo.IsCircleBombAvailable);
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
		}

		private void CreateButton(int row, int column)
		{
			var tileTypeName = string.Empty;
			if (currentLevelInfo.Tiles.Count == 0) return;
			var tile = currentLevelInfo.Tiles[row].Tiles[column];
			if (tile != null)
			{
				var bubbleTile = tile as BubbleTileInfo;
				if (bubbleTile != null)
				{
					tileTypeName = $"{bubbleTile.Type.ToString()}Bubble";
					if (bubbleTile.CoverType != CoverType.None)
						tileTypeName += "_" + bubbleTile.CoverType;
				}

				var randomBubbleTile = tile as RandomBubbleTileInfo;
				if (randomBubbleTile != null)
				{
					tileTypeName = $"{randomBubbleTile.Type.ToString()}";
					if (randomBubbleTile.CoverType != CoverType.None)
						tileTypeName += "_" + randomBubbleTile.CoverType;
				}

				var blockerTile = tile as BlockerTileInfo;
				if (blockerTile != null)
					tileTypeName = $"{blockerTile.BubbleType.ToString()}";

				var boosterTile = tile as BoosterTileInfo;
				if (boosterTile != null)
					tileTypeName = $"{boosterTile.BubbleType.ToString()}";
				
				var collectableTile = tile as CollectableTileInfo;
				if (collectableTile != null)
					tileTypeName = $"{collectableTile.Type.ToString()}";
				if (GUILayout.Button(tileTextures[tileTypeName], GUILayout.Width(LevelTileButtonSize),
					GUILayout.Height(LevelTileButtonSize)))
					DrawTile(row, column);
			}
			else
			{
				if (GUILayout.Button("", GUILayout.Width(LevelTileButtonSize),
					GUILayout.Height(LevelTileButtonSize)))
					DrawTile(row, column);
			}
		}

		private void DrawTile(int row, int column)
		{
			switch (currentBrushMode)
			{
				case BrushMode.Tile:
					currentLevelInfo.Tiles[row].Tiles[column] = DrawTile(currentLevelInfo.Tiles[row].Tiles[column], row);
					break;

				case BrushMode.Row:
				{
					var currentRow = currentLevelInfo.Tiles[row];
					for (var i = 0; i < currentRow.Tiles.Count; i++)
					{
						currentRow.Tiles[i] = DrawTile(currentRow.Tiles[i], row);
					}
				}
					break;

				case BrushMode.Column:
					for (var i = 0; i < currentLevelInfo.Rows; i++)
					{
						if (column < currentLevelInfo.Tiles[i].Tiles.Count)
							currentLevelInfo.Tiles[i].Tiles[column] = DrawTile(currentLevelInfo.Tiles[i].Tiles[column], row);
					}
					break;

				case BrushMode.Fill:
					for (var j = 0; j < currentLevelInfo.Rows; j++)
					{
						var currentRow = currentLevelInfo.Tiles[j];
						for (var i = 0; i < currentRow.Tiles.Count; i++)
						{
							currentLevelInfo.Tiles[j].Tiles[i] = DrawTile(currentLevelInfo.Tiles[j].Tiles[i], row);
						}
					}
					break;
			}
		}
		
		private TileInfo DrawTile(TileInfo tile, int row)
		{
			if (currentBrushType == BrushType.Cover)
			{
				var info = tile as BubbleTileInfo;
				if (info != null)
				{
					info.CoverType = currentCoverType;
					return info;
				}

				var bubbleTileInfo = tile as RandomBubbleTileInfo;
				if (bubbleTileInfo != null)
				{
					bubbleTileInfo.CoverType = currentCoverType;
					return bubbleTileInfo;
				}
			}
			
			Object.DestroyImmediate(tile, true);

			var tileInfo = GetTileInfo();

			if (tileInfo != null && !AssetDatabase.IsSubAsset(tileInfo))
				AssetDatabase.AddObjectToAsset(tileInfo, currentLevelInfo.Tiles[row]);
			
			return tileInfo;
		}
		
		private TileInfo GetTileInfo()
		{
			switch (currentBrushType)
			{
				case BrushType.Bubble:
				{
					var tileInfo = ScriptableObject.CreateInstance<BubbleTileInfo>();
					tileInfo.hideFlags = HideFlags.HideInHierarchy;
					tileInfo.Type = currentColorBubbleType;
					tileInfo.CoverType = currentCoverType;
					return tileInfo;
				}

				case BrushType.RandomBubble:
				{
					var tileInfo = ScriptableObject.CreateInstance<RandomBubbleTileInfo>();
					tileInfo.hideFlags = HideFlags.HideInHierarchy;
					tileInfo.Type = currentRandomBubbleType;
					tileInfo.CoverType = currentCoverType;
					return tileInfo;
				}

				case BrushType.Blocker:
				{
					var tileInfo = ScriptableObject.CreateInstance<BlockerTileInfo>();
					tileInfo.hideFlags = HideFlags.HideInHierarchy;
					tileInfo.BubbleType = currentBlockerBubbleType;
					return tileInfo;
				}

				case BrushType.Booster:
				{
					var tileInfo = ScriptableObject.CreateInstance<BoosterTileInfo>();
					tileInfo.hideFlags = HideFlags.HideInHierarchy;
					tileInfo.BubbleType = currentBoosterBubbleType;
					return tileInfo;
				}

				case BrushType.Collectable:
				{
					var tileInfo = ScriptableObject.CreateInstance<CollectableTileInfo>();
					tileInfo.hideFlags = HideFlags.HideInHierarchy;
					tileInfo.Type = currentCollectableBubbleType;
					return tileInfo;
				}

				default:
					return null;
			}
		}

		private void RandomizeLevel()
		{
			for (var j = 0; j < currentLevelInfo.Rows; j++)
			{
				for (var i = 0; i < currentLevelInfo.Tiles[j].Tiles.Count; i++)
				{
					var rndIdx = Random.Range(0, currentLevelInfo.AvailableColors.Count);
					var tileInfo = ScriptableObject.CreateInstance<RandomBubbleTileInfo>();
					tileInfo.hideFlags = HideFlags.HideInHierarchy;
					tileInfo.Type = (RandomBubbleType)rndIdx;
					currentLevelInfo.Tiles[j].Tiles[i] = tileInfo;
					if (!AssetDatabase.IsSubAsset(tileInfo))
						AssetDatabase.AddObjectToAsset(tileInfo, currentLevelInfo.Tiles[j]);
				}
			}
		}
	}
}
