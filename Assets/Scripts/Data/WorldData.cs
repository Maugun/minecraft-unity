using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData {

    public string name;
    public int chunkSize;
    public int chunkHeight;

    public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();

    public ChunkData GetChunk(Vector2Int position) {
        if (chunks.ContainsKey(position)) {
            return chunks[position];
        } else {
            return null;
        }
    }

}
