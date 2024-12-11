// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace BubbleShooterKit
{
    /// <summary>
    /// When a collectable bubble is collected, this event is sent to all the
    /// interested parties.
    /// </summary>
    public struct CollectablesCollectedEvent
    {
        public readonly CollectableBubbleType Type;
        public readonly int Amount;

        public CollectablesCollectedEvent(CollectableBubbleType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
