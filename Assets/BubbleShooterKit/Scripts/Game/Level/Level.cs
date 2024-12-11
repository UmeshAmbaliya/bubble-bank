// Copyright (C) 2018-2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BubbleShooterKit
{
	/// <summary>
	/// Stores the information of a single level in the game.
	/// </summary>
	public class Level
	{
		public readonly List<List<Bubble>> Tiles = new List<List<Bubble>>();

		public int Rows;
		public int Columns;

		private readonly int evenWidth;
		private readonly int oddWidth;

		public Level(int rows, int columns)
		{
			Rows = rows;
			Columns = columns;
			evenWidth = columns;
			oddWidth = columns - 1;
			for (var i = 0; i < Rows; i++)
			{
				if (i % 2 == 0)
				{
					var row = new List<Bubble>(evenWidth);
					row.AddRange(Enumerable.Repeat<Bubble>(null, evenWidth));
					Tiles.Add(row);
				}
				else
				{
					var row = new List<Bubble>(oddWidth);
					row.AddRange(Enumerable.Repeat<Bubble>(null, oddWidth));
					Tiles.Add(row);
				}
			}
			
			var bottomRow = new List<Bubble>(Columns);
			bottomRow.AddRange(Enumerable.Repeat<Bubble>(null, Columns));
			Tiles.Add(bottomRow);
		}

		public Bubble GetTile(int row, int column)
		{
			if (row % 2 == 0)
			{
				if (row >= 0 && row < Rows && column >= 0 && column < evenWidth)
				{
					return Tiles[row][column];
				}
			}
			else
			{
				if (row >= 0 && row < Rows && column >= 0 && column < oddWidth)
				{
					return Tiles[row][column];
				}
			}
			return null;
		}

		public bool IsValidTile(int row, int column)
		{
			if (row % 2 == 0)
			{
				return column >= 0 && column < evenWidth;
			}
			else
			{
				return column >= 0 && column < oddWidth;
			}
		}

		public void AddBottomRow()
		{
			var row = new List<Bubble>(Columns);
			row.AddRange(Enumerable.Repeat<Bubble>(null, Columns));
			Tiles.Add(row);
			Rows = Tiles.Count;
		}

		public int GetGround()
		{
			var ground = Rows - 1;
			 
			for (var i = Rows - 1; i >= 0; i--)
			{
				var row = Tiles[i];
				var isEmpty = true;
				foreach (var tile in row)
				{
					if (tile != null)
					{
						isEmpty = false;
						break;
					}
				}

				if (!isEmpty)
				{
					break;
				}

				ground -= 1;
			}

			return ground;
		}
		
		public override string ToString()
		{
			var str = new StringBuilder();
			foreach (var row in Tiles)
			{
				foreach (var tile in row)
				{
					if (tile != null)
					{
						str.Append("X");
					}
					else
					{
						str.Append("O");
					}
				}

				str.Append("\n");
			}

			return str.ToString();
		}

		public List<List<Bubble>> GetTiles()
		{
			return Tiles;			
		}
	}
}
