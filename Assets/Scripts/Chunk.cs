using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Chunk {

    public static int idNumber = 0;
    public int Id { get; set; }

    // General
    private int _size;
    private int _height;
    private Vector2 _coord;
    private GameObject _chunkObj;

    // Block Map
    //private Dictionary<Vector3, BlockType> _blockMap = new Dictionary<Vector3, BlockType>();
    private BlockType[,,] _blockMap;
    private bool _isBlockMapReceived;
    private BiomeData[] _biomes;

    // Mesh
    private Mesh _mesh = new Mesh();
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();
    private bool _isMeshReceived;

    private bool _setCollider;
    public bool IsLoad { get; set; }

    #region Constructor
    public Chunk(Vector2 coord, int size, int height, BiomeData[] biomes) {
        // General
        _size = size;
        _height = height;
        _coord = coord * size;
        Id = idNumber;
        idNumber++;
        IsLoad = false;
        _biomes = biomes;
    }
    #endregion

    #region Load Data
    public async void LoadChunk(bool setCollider) {
        _setCollider = setCollider;
        //ThreadedDataRequester.RequestData(() => BuildBlockMap(_seed, _coord), OnBlockMapReceived);
        //ThreadedDataRequester.RequestData(() => GenerateChunkData(), OnMeshReceived);

        OnMeshReceived(await Task.Run(GenerateChunkData));

        // await Task.Run(GenerateChunkData);
        // World.Instance._toUpdate.Add(this);
    }

    public bool GenerateChunkData() {
        _blockMap = BuildBlockMap(_biomes, _coord);
        _isBlockMapReceived = true;
        return BuildMesh();
    }

    public void OnMeshReceived(object success) {
        if (!(bool)success) {
            Debug.LogError("Error: load mesh for chunk " + Id);
            return;
        }
        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.uv = _uvs.ToArray();
        // Debug.Log("Vertices: " + _vertices.Count);
        // Debug.Log("Triangles: " + _triangles.Count);
        _mesh.RecalculateNormals();
        _isMeshReceived = true;
        LoadChunkObj(_setCollider);
    }

    public bool LoadChunkObj(bool setCollider) {
        if (!_chunkObj || !_isMeshReceived) return false;

        _chunkObj.name = "Terrain Chunk " + Id;
        _chunkObj.transform.position = new Vector3(_coord.x, 0, _coord.y);
        _chunkObj.transform.GetComponent<MeshFilter>().mesh = _mesh;
        if (setCollider) SetCollider();
        IsLoad = true;
        SetVisible(true);

        return true;
    }
    #endregion

    #region Block Map
    public BlockType[,,] BuildBlockMap(BiomeData[] biomes, Vector2 chunkPos) {
        BlockType[,,] blockMap = new BlockType[_size, _height, _size];
        float groundLevelOffset = _height * .25f;
        Noise heigtNoise = new Noise(biomes[0].noise, chunkPos);
        Noise altitudeNoise = new Noise(biomes[1].noise, chunkPos);

        for (int x = 0; x < _size; x++) {
            for (int z = 0; z < _size; z++) {

                // Get Noise Height
                float heightMap = heigtNoise.Perlin(x, z);
                float altitudeMap = altitudeNoise.Perlin(x, z);

                float map = heightMap * altitudeMap;

                // Ground Level
                int groundLevel = Mathf.CeilToInt(groundLevelOffset + map * 100); // Add Height Map to half of chunk's height
                groundLevel = (groundLevel < _height) ? groundLevel : _height;

                // Get Block for y Axe
                for (int y = 0; y < _height; y++) {
                    blockMap[x, y, z] = GetBlock(x + chunkPos.x, y, z + chunkPos.y, groundLevel);
                }
            }
        }
        return blockMap;
    }

    private BlockType GetBlock(float x, float y, float z, int groundLevel) {
        BlockType block = BlockType.Air;

        if (y == groundLevel) return block = BlockType.Grass;
        if (y == groundLevel - 1) return block = BlockType.Stone;
        if (y == 0) return block = BlockType.BedRock;
        if (y <= 5) return block = BlockType.Stone;
        if (y < groundLevel) {
            float noise3Dy = Noise.Perlin3D(x, y, z, 0.05f);

            if (noise3Dy >= .45f) {
                block = BlockType.Stone;
            } else {
                block = BlockType.Air;
            }
            // block = BlockType.Dirt;

            return block;
        }

        return block;
    }
    #endregion

    #region Mesh
    public bool BuildMesh() {
        if (!_isBlockMapReceived) return false;

        for (int x = 0; x < _size; x++) {
            for (int z = 0; z < _size; z++) {
                for (int y = 0; y < _height; y++) {
                    AddBlockMesh(x, y, z);
                }
            }
        }
        return true;
    }

    void AddBlockMesh(int x, int y, int z) {

        if (_blockMap[x, y, z] != BlockType.Air) {
            int faces = 0;
            int nx = x;
            int ny = y;
            int nz = z;
            Vector3 pos = new Vector3(x, y, z);

            // Up Face
            ny++;
            if (ny >= _height || _blockMap[nx, ny, nz] == BlockType.Air) {
                _vertices.AddRange(MeshGenerator.MakeCubeFace(pos, Direction.Up));
                _uvs.AddRange(Block.blocks[_blockMap[x, y, z]].topPos.GetUVs());
                faces++;
            }
            ny--;

            // Down Face
            ny--;
            if (ny < 0 || _blockMap[nx, ny, nz] == BlockType.Air) {
                _vertices.AddRange(MeshGenerator.MakeCubeFace(pos, Direction.Down));
                _uvs.AddRange(Block.blocks[_blockMap[x, y, z]].bottomPos.GetUVs());
                faces++;
            }
            ny++;

            // North Face
            nz++;
            if (nz >= _size || _blockMap[nx, ny, nz] == BlockType.Air) {
                _vertices.AddRange(MeshGenerator.MakeCubeFace(pos, Direction.North));
                _uvs.AddRange(Block.blocks[_blockMap[x, y, z]].sidePos.GetUVs());
                faces++;
            }
            nz--;

            // East Face
            nx++;
            if (nx >= _size || _blockMap[nx, ny, nz] == BlockType.Air) {
                _vertices.AddRange(MeshGenerator.MakeCubeFace(pos, Direction.East));
                _uvs.AddRange(Block.blocks[_blockMap[x, y, z]].sidePos.GetUVs());
                faces++;
            }
            nx--;

            // South Face
            nz--;
            if (nz < 0 || _blockMap[nx, ny, nz] == BlockType.Air) {
                _vertices.AddRange(MeshGenerator.MakeCubeFace(pos, Direction.South));
                _uvs.AddRange(Block.blocks[_blockMap[x, y, z]].sidePos.GetUVs());
                faces++;
            }
            nz++;

            // West Face
            nx--;
            if (nx < 0 || _blockMap[nx, ny, nz] == BlockType.Air) {
                _vertices.AddRange(MeshGenerator.MakeCubeFace(pos, Direction.West));
                _uvs.AddRange(Block.blocks[_blockMap[x, y, z]].sidePos.GetUVs());
                faces++;
            }
            nx++;

            // Add Face Triangles
            _triangles.AddRange(MeshGenerator.AddCubeFaceTriangles(_vertices.Count, faces));
        }
    }
    #endregion

    #region Utils

    public bool SetCollider() {
        if (!_chunkObj) return false;
        _chunkObj.transform.GetComponent<MeshCollider>().sharedMesh = _mesh;
        return true;
    }

    public bool RemoveCollider() {
        if (!_chunkObj) return false;
        MeshCollider collider = _chunkObj.transform.GetComponent<MeshCollider>();
        if (collider.sharedMesh != null) collider.sharedMesh = null;
        return true;
    }

    public void SetChunkObj(GameObject obj) {
        _chunkObj = obj;
    }

    public GameObject GetChunkObj() {
        return _chunkObj;
    }

    public void SetVisible(bool visible) {
        if (!_chunkObj) return;
        _chunkObj.SetActive(visible);
    }

    public bool IsVisible() {
        if (!_chunkObj) return false;
        return _chunkObj.activeSelf;
    }
    #endregion
}
