// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using TMPro;
using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// This class contains the logic associated to the generic confirmation popup.
    /// </summary>
	public class ConfirmationPopup : Popup
	{
		[SerializeField]
		private TextMeshProUGUI primaryText = null;
		
		[SerializeField]
		private TextMeshProUGUI secondaryText = null;
		
		private Action onAcceptAction;
		
		public void OnAcceptButtonPressed()
		{
			onAcceptAction();
		}

		public void SetInfo(string primary, string secondary, Action onAccept)
		{
			primaryText.text = primary;
			secondaryText.text = secondary;
			onAcceptAction = onAccept;
		}
	}
}
