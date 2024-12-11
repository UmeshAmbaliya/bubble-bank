// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// A collection of sounds. We use two collections in the game: one for the
	/// menu sounds and another for the game sounds.
	/// </summary>
	[CreateAssetMenu(fileName = "SoundCollection", menuName = "Bubble Shooter Kit/Sound collection", order = 2)]
	public class SoundCollection : ScriptableObject
	{
		public List<AudioClip> Sounds;
	}
}
