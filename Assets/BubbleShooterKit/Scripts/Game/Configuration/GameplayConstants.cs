// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
	/// <summary>
	/// Gameplay-related constants.
	/// </summary>
	public static class GameplayConstants
	{
		public const int NumBubblesNeededForMatch = 3;
		public const float BubbleHitStrength = 0.05f;
		public const float TileWidthMultiplier = 0.91f;
		public const float TileHeightMultiplier = 0.85f;
		public const float FloatingIslandsRemovalDelay = 0.04f;
		public const float FallingMinXForce = 1.0f;
		public const float FallingMaxXForce = 1.5f;
		public const float FallingMinYForce = 2.0f;
		public const float FallingMaxYForce = 2.5f;
		public const float FallingGravityMultiplier = 4.0f;
		public const float FallingFadeOutDelay = 0.4f;
		public const float FallingFadeOutDuration = 0.25f;
		public const float NewBubbleSpawningDelay = 0.4f;
		public const float VoidChainSpeed = 1.0f;
		public const float VoidChainFinishDelay = 0.5f;
		public const float BubbleBlinkRate = 5.0f;
		public const float LevelScrollSpeed = 0.1f;
		public const float TopPivotHeight = 0.7f;
		public const float BottomPivotHeight = 0.6f;
		public const float WinPopupDelay = 0.5f;
		public const float LosePopupDelay = 0.5f;
		public const float OutOfBubblesPopupDelay = 0.5f;
		public const float EndOfGameBubbleSequenceStartingDelay = 1.0f;
		public const float EndOfGameBubbleSequenceSpeed = 0.75f;
	}
}
