// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleShooterKit
{
    /// <summary>
    /// Miscellaneous level utilities.
    /// </summary>
    public static class LevelUtils
    {
        public static float currentLevelOffsetY;
        public struct EmptyTileInfo
        {
            public int Row;
            public int Column;
            public Vector2 Position;
        }

        public static List<Bubble> GetNeighbours(Level level, Bubble bubble)
        {
            var neighbours = new List<Bubble>();

            if (bubble.Row % 2 == 0)
            {
                var topLeft = level.GetTile(bubble.Row - 1, bubble.Column - 1);
                var topRight = level.GetTile(bubble.Row - 1, bubble.Column);
                var left = level.GetTile(bubble.Row, bubble.Column - 1);
                var right = level.GetTile(bubble.Row, bubble.Column + 1);
                var bottomLeft = level.GetTile(bubble.Row + 1, bubble.Column - 1);
                var bottomRight = level.GetTile(bubble.Row + 1, bubble.Column);
                if (topLeft != null ) neighbours.Add(topLeft);
                if (topRight != null ) neighbours.Add(topRight);    
                if (left != null) neighbours.Add(left);
                if (right != null && right.isInView) neighbours.Add(right);
                if (bottomLeft != null) neighbours.Add(bottomLeft);
                if (bottomRight != null && bottomRight.isInView) neighbours.Add(bottomRight);
            }
            else
            {
                var topLeft = level.GetTile(bubble.Row - 1, bubble.Column);
                var topRight = level.GetTile(bubble.Row - 1, bubble.Column + 1);
                var left = level.GetTile(bubble.Row, bubble.Column - 1);
                var right = level.GetTile(bubble.Row, bubble.Column + 1);
                var bottomLeft = level.GetTile(bubble.Row + 1, bubble.Column);
                var bottomRight = level.GetTile(bubble.Row + 1, bubble.Column + 1);
                if (topLeft != null) neighbours.Add(topLeft);
                if (topRight != null) neighbours.Add(topRight);
                if (left != null) neighbours.Add(left);
                if (right != null) neighbours.Add(right);
                if (bottomLeft != null) neighbours.Add(bottomLeft);
                if (bottomRight != null) neighbours.Add(bottomRight);
            }

            return neighbours;
        }

        public static List<Bubble> GetNeighboursInRadius(Level level, Bubble bubble, int radius)
        {
            var neighbours = new List<Bubble>();

            neighbours.AddRange(GetNeighboursInRadius(level, bubble));
            --radius;
            while (radius > 0)
            {
                var newNeighbours = new List<Bubble>();
                foreach (var neighbour in neighbours)
                {
                    newNeighbours.AddRange(GetNeighboursInRadius(level, neighbour));
                }

                foreach (var neighbour in newNeighbours)
                {
                    if (!neighbours.Contains(neighbour) && neighbour.isInView)
                        neighbours.Add(neighbour);
                }

                --radius;
            }

            return neighbours;
        }

        public static List<Bubble> GetNeighboursInRadius(Level level, Bubble bubble)
        {
            var neighbours = new List<Bubble>();

            var row = bubble.Row;
            var column = bubble.Column;
            if (row % 2 == 0)
            {
                AddNeighbour(level, neighbours, row, column);
                AddNeighbour(level, neighbours, row - 1, column - 1);
                AddNeighbour(level, neighbours, row - 1, column);
                AddNeighbour(level, neighbours, row, column - 1);
                AddNeighbour(level, neighbours, row, column + 1);
                AddNeighbour(level, neighbours, row + 1, column - 1);
                AddNeighbour(level, neighbours, row + 1, column);
            }
            else
            {
                AddNeighbour(level, neighbours, row, column);
                AddNeighbour(level, neighbours, row - 1, column);
                AddNeighbour(level, neighbours, row - 1, column + 1);
                AddNeighbour(level, neighbours, row, column - 1);
                AddNeighbour(level, neighbours, row, column + 1);
                AddNeighbour(level, neighbours, row + 1, column);
                AddNeighbour(level, neighbours, row + 1, column + 1);
            }

            return neighbours;
        }

        public static List<Bubble> GetRing(Level level, Bubble bubble, int radius)
        {
            var neighbours = new List<Bubble>();

            if (radius == 0)
            {
                neighbours.Add(bubble);
                return neighbours;
            }

            var row = bubble.Row;
            var column = bubble.Column;

            if (row % 2 == 0)
            {
                var i = 1;
                var j = 0;
                var k = 0;
                while (i <= radius)
                {
                    var leftTop = level.GetTile(row - i, column - radius + j);
                    var rightTop = level.GetTile(row - i, column + j + radius - k - 1);

                    var leftBottom = level.GetTile(row + i, column - radius + j);
                    var rightBottom = level.GetTile(row + i, column + j + radius - k - 1);

                    if (leftTop != null) neighbours.Add(leftTop);
                    if (rightTop != null) neighbours.Add(rightTop);
                    if (leftBottom != null) neighbours.Add(leftBottom);
                    if (rightBottom != null) neighbours.Add(rightBottom);

                    if (i == radius)
                    {
                        var c = column - radius + j + 1;
                        while (c < (column + j + radius - k - 1))
                        {
                            var tile = level.GetTile(row + i, c);
                            if (tile != null) neighbours.Add(tile);

                            tile = level.GetTile(row - i, c);
                            if (tile != null) neighbours.Add(tile);
                            ++c;
                        }
                    }

                    ++i;
                    if (i % 2 == 0)
                        ++j;
                    ++k;
                }
            }
            else
            {
                var i = 1;
                var j = 1;
                var k = 0;
                while (i <= radius)
                {
                    var leftTop = level.GetTile(row - i, column - radius + j);
                    var rightTop = level.GetTile(row - i, column + radius + j - k - 1);

                    var leftBottom = level.GetTile(row + i, column - radius + j);
                    var rightBottom = level.GetTile(row + i, column + radius + j - k - 1);

                    if (leftTop != null) neighbours.Add(leftTop);
                    if (rightTop != null) neighbours.Add(rightTop);
                    if (leftBottom != null) neighbours.Add(leftBottom);
                    if (rightBottom != null) neighbours.Add(rightBottom);

                    if (i == radius)
                    {
                        var c = column - radius + j + 1;
                        while (c < (column + j + radius - k - 1))
                        {
                            var tile = level.GetTile(row + i, c);
                            if (tile != null) neighbours.Add(tile);

                            tile = level.GetTile(row - i, c);
                            if (tile != null) neighbours.Add(tile);
                            ++c;
                        }
                    }

                    ++i;
                    if (i % 2 != 0)
                        ++j;
                    ++k;
                }
            }

            var left = level.GetTile(row, column - radius);
            var right = level.GetTile(row, column + radius);
            if (left != null) neighbours.Add(left);
            if (right != null) neighbours.Add(right);

            return neighbours;
        }

        private static void AddNeighbour(Level level, List<Bubble> neighbours, int row, int column)
        {
            var neighbour = level.GetTile(row, column);
            if (neighbour != null && !neighbours.Contains(neighbour))
                neighbours.Add(neighbour);
        }

        public static List<ColorBubble> GetMatches(Level level, ColorBubble colorBubble)
        {
            var matches = new List<ColorBubble>();
            GetMatchesRecursive(level, colorBubble, matches);
            if (!matches.Contains(colorBubble) && colorBubble.isInView)
            {
                matches.Add(colorBubble);
            }

            return matches;
        }

        private static void GetMatchesRecursive(Level level, ColorBubble colorBubble, List<ColorBubble> matchedBubbles)
        {
            var neighbours = GetNeighbours(level, colorBubble).OfType<ColorBubble>();

            var hasMatch = false;
            var enumerable = neighbours as ColorBubble[] ?? neighbours.ToArray();
            foreach (var neighbour in enumerable)
            {
                if (neighbour.Type == colorBubble.Type && colorBubble.isInView)
                {
                    hasMatch = true;
                }
            }

            if (!hasMatch)
            {
                return;
            }

            if (!matchedBubbles.Contains(colorBubble) && colorBubble.isInView)
            {
                matchedBubbles.Add(colorBubble);
            }

            foreach (var neighbour in enumerable)
            {
                if (neighbour.Type == colorBubble.Type &&
                    !matchedBubbles.Contains(neighbour) && neighbour.isInView)
                {
                    GetMatchesRecursive(level, neighbour, matchedBubbles);
                }
            }
        }

        private static List<Bubble> FindIsland(Level level, Bubble bubble, List<Bubble> processed)
        {
            var toProcess = new Stack<Bubble>();
            toProcess.Push(bubble);

            processed.Add(bubble);

            var foundIsland = new List<Bubble>();

            while (toProcess.Count > 0)
            {
                var processedBubble = toProcess.Pop();

                if (processedBubble == null)
                {
                    continue;
                }

                foundIsland.Add(processedBubble);

                var neighbours = GetNeighbours(level, processedBubble);
                foreach (var neighbour in neighbours)
                {
                    if (!processed.Contains(neighbour))
                    {
                        toProcess.Push(neighbour);
                        processed.Add(neighbour);
                    }
                }
            }

            return foundIsland;
        }

        public static List<List<Bubble>> FindFloatingIslands(Level level)
        {
            var foundIslands = new List<List<Bubble>>();
            var processed = new List<Bubble>();

            foreach (var row in level.Tiles)
            {
                foreach (var tile in row)
                {
                    if (!processed.Contains(tile))
                    {
                        var foundCluster = FindIsland(level, tile, processed);

                        if (foundCluster.Count <= 0)
                        {
                            continue;
                        }

                        var floating = true;
                        foreach (var b in foundCluster)
                        {
                            if (b.Row == 0)
                            {
                                floating = false;
                                break;
                            }
                        }

                        if (floating)
                        {
                            foundIslands.Add(foundCluster);
                        }
                    }
                }
            }

            return foundIslands;
        }

        public static List<EmptyTileInfo> GetEmptyNeighbours(Level level, int row, int column, ScreenLayoutInfo layoutInfo)
        {
            var emptyNeighboursInfo = new List<EmptyTileInfo>();

            if (row % 2 == 0)
            {
                var self = level.GetTile(row, column);
                var topLeft = level.GetTile(row - 1, column - 1);
                var topRight = level.GetTile(row - 1, column);
                var left = level.GetTile(row, column - 1);
                var right = level.GetTile(row, column + 1);
                var bottomLeft = level.GetTile(row + 1, column - 1);
                var bottomRight = level.GetTile(row + 1, column);
                if (self == null && level.IsValidTile(row, column))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row, column));
                }
                if (topLeft == null && level.IsValidTile(row - 1, column - 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row - 1, column - 1));
                }
                if (topRight == null && level.IsValidTile(row - 1, column))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row - 1, column));
                }
                if (left == null && level.IsValidTile(row, column - 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row, column - 1));
                }
                if (right == null && level.IsValidTile(row, column + 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row, column + 1));
                }
                if (bottomLeft == null && level.IsValidTile(row + 1, column - 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row + 1, column - 1));
                }
                if (bottomRight == null && level.IsValidTile(row + 1, column))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row + 1, column));
                }
            }
            else
            {
                var self = level.GetTile(row, column);
                var topLeft = level.GetTile(row - 1, column);
                var topRight = level.GetTile(row - 1, column + 1);
                var left = level.GetTile(row, column - 1);
                var right = level.GetTile(row, column + 1);
                var bottomLeft = level.GetTile(row + 1, column);
                var bottomRight = level.GetTile(row + 1, column + 1);
                if (self == null  && level.IsValidTile(row, column))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row, column));
                }
                if (topLeft == null && level.IsValidTile(row - 1, column))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row - 1, column));
                }
                if (topRight == null && level.IsValidTile(row - 1, column + 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row - 1, column + 1));
                }
                if (left == null && level.IsValidTile(row, column - 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row, column - 1));
                }
                if (right == null && level.IsValidTile(row, column + 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row, column + 1));
                }
                if (bottomLeft == null && level.IsValidTile(row + 1, column))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row + 1, column));
                }
                if (bottomRight == null && level.IsValidTile(row + 1, column + 1))
                {
                    emptyNeighboursInfo.Add(GenerateEmptyTileInfo(layoutInfo, row + 1, column + 1));
                }
            }

            return emptyNeighboursInfo;
        }

        private static EmptyTileInfo GenerateEmptyTileInfo(ScreenLayoutInfo layoutInfo, int row, int column)
        {
            return new EmptyTileInfo
            {
                Row = row,
                Column = column,
                Position = CalculatePosition(layoutInfo, row, column)
            };
        }

        private static Vector2 CalculatePosition(ScreenLayoutInfo layoutInfo, int row, int column)
        {
            float rowOffset;
            if (row % 2 == 0)
            {
                rowOffset = 0;
            }
            else
            {
                rowOffset = layoutInfo.TileWidth * 0.5f;
            }

            var bottomPivot = new Vector2(0, Camera.main.pixelHeight * GameplayConstants.BottomPivotHeight);
            var bottomPivotPos = Camera.main.ScreenToWorldPoint(bottomPivot);

            var pos = new Vector2(
                (column * layoutInfo.TileWidth * GameplayConstants.TileWidthMultiplier) + rowOffset,
                -row * layoutInfo.TileHeight * GameplayConstants.TileHeightMultiplier);
            var newPos = pos;
            newPos.x -= layoutInfo.TotalWidth / 2f;
            newPos.x += (layoutInfo.TileWidth * GameplayConstants.TileWidthMultiplier) / 2f;
            newPos.y += bottomPivotPos.y + layoutInfo.TotalHeight;
            newPos.y += PlayerPrefs.GetFloat("scrolled_height") + currentLevelOffsetY;
            return newPos;
        }
    }
}
    