using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour {

    public Transform _viewer;
    public int _chunkSize;
    public int _chunkHeight;
    public Material _material;

    [Range(2, 20)]
    public int _visibleChunkInDistance;

    public BiomeData[] _biomes;

    private Dictionary<Vector2, Chunk> _chunkDictionary = new Dictionary<Vector2, Chunk>();
    private List<Vector2> _visibleChunks = new List<Vector2>();
    private List<GameObject> _chunkObjectList = new List<GameObject>();
    private Vector2 _viewerPosition;
    private Vector2 _viewerPositionOld;
    private Vector2 _currentChunkCoord;

    private List<Vector2> _previousVisibleChunks = new List<Vector2>();

    private bool _updateVisibleChunksList = false;
    private bool _updateVisibleChunks = false;
    private bool _updateChunksCollider = false;
    private int _updateNb = 0;

    [System.NonSerialized]
    public List<Chunk> _toUpdate = new List<Chunk>();

    private static World _instance;
    public static World Instance { get { return _instance; } }

    public void Awake() {
        // Singleton
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        _instance = this;
        int chunkObjNumber = (_visibleChunkInDistance + 1) * 2;
        chunkObjNumber *= chunkObjNumber;
        for (int i = 0; i < chunkObjNumber; i++) {
            GameObject chunkObj = new GameObject("Terrain Chunk");
            chunkObj.layer = LayerMask.NameToLayer("Chunk");
            chunkObj.AddComponent<MeshRenderer>();
            chunkObj.AddComponent<MeshFilter>();
            chunkObj.AddComponent<MeshCollider>();
            chunkObj.transform.GetComponent<MeshRenderer>().material = _material;
            chunkObj.transform.position = Vector3.zero;
            chunkObj.transform.parent = transform;
            chunkObj.SetActive(false);
            _chunkObjectList.Add(chunkObj);
        }
        _currentChunkCoord = GetCurrentChunkCoord();
        InitVisibleChunks();
    }

    void Update() {
        _viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);
        Vector2 previousCurrentChunk = _currentChunkCoord;
        _currentChunkCoord = GetCurrentChunkCoord();
        if (_currentChunkCoord != previousCurrentChunk) {
            _updateVisibleChunksList = true;
        }

        // if (_toUpdate.Count > 0 && _updateNb % 3 == 0) {
        //     _toUpdate[0].OnMeshReceived(true);
        //     _toUpdate.RemoveAt(0);
        // }

        if (_updateNb % 10 == 0) {
            if (_updateVisibleChunksList && !_updateVisibleChunks && !_updateChunksCollider) {
                UpdateVisibleChunksList();
                _updateVisibleChunksList = false;
                _updateVisibleChunks = true;
            }
            if (_updateVisibleChunks) {
                UpdateVisibleChunks();
                _updateVisibleChunks = false;
                _updateChunksCollider = true;
            }
            if (_updateChunksCollider) {
                UpdateChunksCollider();
                _updateChunksCollider = false;
            }
        }
        if (_updateNb > 90) _updateNb = 0;
        _updateNb++;
    }

    private void InitVisibleChunks() {
        UpdateVisibleChunksList();

        for (int i = 0; i < _visibleChunks.Count; i++) {
            bool loadChunkCollider = (i < 9) ? true : false;
            Chunk newChunk = new Chunk(_visibleChunks[i], _chunkSize, _chunkHeight, _biomes);
            _chunkDictionary.Add(_visibleChunks[i], newChunk);
            newChunk.SetChunkObj(_chunkObjectList[i]);
            newChunk.LoadChunk(loadChunkCollider);
        }

    }

    private void UpdateVisibleChunksList() {
        _previousVisibleChunks = _visibleChunks.ToList();
        _visibleChunks.Clear();
        for (int y = -_visibleChunkInDistance - 1; y <= _visibleChunkInDistance; y++) {
            for (int x = -_visibleChunkInDistance - 1; x <= _visibleChunkInDistance; x++) {
                _visibleChunks.Add(new Vector2(x, y) + _currentChunkCoord);
            }
        }
        _visibleChunks = _visibleChunks.OrderBy(chunkCoord => (_currentChunkCoord - chunkCoord).sqrMagnitude).ToList();
    }

    private void UpdateVisibleChunks() {
        List<Vector2> changedVisibleChunks = _previousVisibleChunks.Except(_visibleChunks).ToList();
        List<Vector2> newVisibleChunks = _visibleChunks.Except(_previousVisibleChunks).ToList();

        for (int i = 0; i < changedVisibleChunks.Count; i++) {
            GameObject freeObj = _chunkDictionary[changedVisibleChunks[i]].GetChunkObj();
            freeObj.SetActive(false);
            _chunkDictionary[changedVisibleChunks[i]].SetChunkObj(null);

            if (_chunkDictionary.ContainsKey(newVisibleChunks[i])) {
                _chunkDictionary[newVisibleChunks[i]].SetChunkObj(freeObj);
                _chunkDictionary[newVisibleChunks[i]].LoadChunkObj(false);
            } else {
                Chunk newChunk = new Chunk(newVisibleChunks[i], _chunkSize, _chunkHeight, _biomes);
                _chunkDictionary.Add(newVisibleChunks[i], newChunk);
                newChunk.SetChunkObj(freeObj);
                newChunk.LoadChunk(true);
            }
        }
    }

    private void UpdateChunksCollider() {
        for (int i = 0; i < _visibleChunks.Count; i++) {
            if (i < 9) {
                _chunkDictionary[_visibleChunks[i]].SetCollider();
                continue;
            }
            _chunkDictionary[_visibleChunks[i]].RemoveCollider();
        }
    }


    public Vector2 GetCurrentChunkCoord() {
        int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

        return new Vector2(currentChunkCoordX, currentChunkCoordY);
    }
}
