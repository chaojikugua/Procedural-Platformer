﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Vaults {
	public enum VaultType {
		Entrance,
		Exit,
		Floating,
		Configured
	}
		
	public enum TileType {
		Empty,
		Wall,
		Lava,
		Spike
	}

	public class Vault {
		public string name;
		public Coordinates size;
		public string csv;
		public VaultType type = VaultType.Floating;
		public List<TiledObject> objects = new List<TiledObject>();
		public int minDepth, maxDepth;

		public TileType[,] tiles { get; set;}

		public void ParseCSV() {
			tiles = new TileType[size.x, size.y];
			int i = 0;
			string[] tileIds = csv.Split (',');
			for (int x = 0; x < size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					string c = tileIds [i];
					switch (int.Parse(c)) {
					case 5:
					case 6:
					case 7:
					case 8:
						tiles [x, y] = TileType.Wall;
						break;
					case 9:
						tiles [x, y] = TileType.Lava;
						break;
					case 11: 
						tiles [x, y] = TileType.Spike;
						break;
					default:
						tiles [x, y] = TileType.Empty;
						break;
					}
					i++;
				}
			}
		}
	}
}