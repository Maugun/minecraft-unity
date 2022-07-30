using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePos {
    private const float TEXTURE_ATLAS_SIZE_IN_BLOCK = 16f;

    private int _x, _y;
    private Vector2[] _uvs;

    public TilePos(int x, int y) {
        _x = x;
        _y = y;
        _uvs = new Vector2[]
        {
            new Vector2(_x / TEXTURE_ATLAS_SIZE_IN_BLOCK + .004f, _y / TEXTURE_ATLAS_SIZE_IN_BLOCK + .004f),
            new Vector2(_x / TEXTURE_ATLAS_SIZE_IN_BLOCK + .004f, (_y + 1) / TEXTURE_ATLAS_SIZE_IN_BLOCK - .004f),
            new Vector2((_x + 1) / TEXTURE_ATLAS_SIZE_IN_BLOCK - .004f, (_y + 1) / TEXTURE_ATLAS_SIZE_IN_BLOCK - .004f),
            new Vector2((_x + 1) / TEXTURE_ATLAS_SIZE_IN_BLOCK - .004f, _y / TEXTURE_ATLAS_SIZE_IN_BLOCK + .004f),
        };
    }

    public Vector2[] GetUVs() {
        return _uvs;
    }

    public static Dictionary<Tile, TilePos> tiles = new Dictionary<Tile, TilePos>()
    {
        {Tile.Dirt, new TilePos(2,15)},
        {Tile.Grass, new TilePos(0,15)},
        {Tile.GrassSide, new TilePos(3,15)},
        {Tile.Stone, new TilePos(1,15)},
        {Tile.TreeSide, new TilePos(4,14)},
        {Tile.TreeCX, new TilePos(5,14)},
        {Tile.Leaves, new TilePos(5,12)},
        {Tile.BedRock, new TilePos(1,14)},
    };
}

public enum Tile { Dirt, Grass, GrassSide, Stone, TreeSide, TreeCX, Leaves, BedRock }
