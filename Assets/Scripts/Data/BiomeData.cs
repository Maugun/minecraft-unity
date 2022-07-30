using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "VoxelTerrain/Biome")]
public class BiomeData : ScriptableObject {

    [Header("General")]
    public string biomeName;

    [Header("Terrain")]
    public int terrainHeight;
    public float terrainScale;

    public BlockType surface;
    public BlockType underground;

    [Header("Terrain Noise")]
    public NoiseSettings noise;

}
