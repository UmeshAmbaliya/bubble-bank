// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
    /// <summary>
    /// When a bubble explodes, this event is sent to all the interested parties.
    /// </summary>
    public struct BubblesCollectedEvent
    {
        public readonly ColorBubbleType Type;
        public readonly int Amount;

        public BubblesCollectedEvent(ColorBubbleType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
