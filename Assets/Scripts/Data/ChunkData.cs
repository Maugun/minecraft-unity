using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData {

    public int y;
    public int x;
    public int groundLevel;

    public Vector2Int Position {
        get { return new Vector2Int(x, y); }
        set { x = value.x; y = value.y; }
    }

    public ChunkData(Vector2Int position) { Position = position; }

}
