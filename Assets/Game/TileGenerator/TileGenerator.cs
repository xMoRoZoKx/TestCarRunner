using System.Collections.Generic;
using UniTools;
using UniTools.Reactive;
using UnityEngine;

public class TileGenerator : ConnectableMonoBehaviour
{
    [SerializeField] private List<TileView> tilePrefabs;
    [SerializeField] private float tileLength = 300f;
    [SerializeField] private float tileSpeed = 3.5f;
    [SerializeField] private int startDistance = 0;
    [SerializeField] private Transform tileParent;

    private string seed = "MySeed";
    private readonly List<TileView> activeTiles = new();
    private readonly Reactive<int> currentCenterIndex = new();
    private readonly Reactive<float> tilesXOffset = new();
    private readonly Reactive<bool> isRunning = new(false);

    private Dictionary<TileView, ObjectPool> tilePools = new();

    public IReactive<int> CurrentCenterIndex => currentCenterIndex;
    public float TileLenght => tileLength;
    public float TileSpeed => tileSpeed;
    public IReadOnlyList<TileView> ActiveTiles => activeTiles;

    private void Awake()
    {
        
    }

    public void SetSeed(string newSeed)
    {
        seed = newSeed;
    }

    public void StartGeneration()
    {
        isRunning.value = true;
    }

    private void FixedUpdate()
    {
        if (!isRunning.value) return;

        foreach (var tile in activeTiles)
        {
            tile.transform.position += Vector3.left * tileSpeed * Time.deltaTime;
        }

        TileView centerTile = GetTileByIndex(currentCenterIndex.value);
        if (centerTile == null) return;

        tilesXOffset.value = centerTile.transform.position.x;

        if (Mathf.Abs(centerTile.transform.position.x - transform.position.x) > tileLength)
        {
            ShiftTilesForward();
            tilesXOffset.value = 0;
        }

        foreach (var tileView in activeTiles)
        {
            tileView.Tick(Time.fixedDeltaTime);
        }
    }

    public void Init()
    {
        foreach (var prefab in tilePrefabs)
        {
            ObjectPool pool = new ObjectPool(prefab.gameObject, 5, tileParent);
            tilePools[prefab] = pool;
        }
        for (int i = -3; i <= 3; i++)
        {
            SpawnTile(currentCenterIndex.value + i);
        }
    }

    private void SpawnTile(int index)
    {
        int prefabIndex = Mathf.FloorToInt(RandomTools.RangeDeterministic(seed + index, 0, tilePrefabs.Count));
        TileView prefab = tilePrefabs[prefabIndex];

        if (!tilePools.TryGetValue(prefab, out var pool))
        {
            Debug.LogError("No pool found for prefab: " + prefab.name);
            return;
        }

        GameObject tileGO = pool.Get();
        TileView tile = tileGO.GetComponent<TileView>();

        tile.transform.position = new Vector3(GetWorldPositionX(index), transform.position.y, transform.position.z);
        tile.Init(seed, index);
        activeTiles.Add(tile);
    }

    private void ShiftTilesForward()
    {
        TileView backTile = GetTileByIndex(currentCenterIndex.value - 4);
        if (backTile != null)
        {
            activeTiles.Remove(backTile);
            ReturnToPool(backTile);
        }

        currentCenterIndex.value++;
        SpawnTile(currentCenterIndex.value + 3);
    }

    private float GetWorldPositionX(int tileIndex)
    {
        int delta = tileIndex - currentCenterIndex.value;
        return transform.position.x + delta * tileLength + tilesXOffset.value;
    }

    public TileView GetTileByIndex(int index)
    {
        return activeTiles.Find(t => t.TileIndex == index);
    }

    public void StopGeneration() => isRunning.value = false;
    public void ResumeGeneration() => isRunning.value = true;

    private void ReturnToPool(TileView tile)
    {
        TileView matchingPrefab = tilePrefabs.Find(p => p.name == tile.name.Replace("(Clone)", "").Trim());

        if (matchingPrefab != null && tilePools.TryGetValue(matchingPrefab, out var pool))
        {
            pool.Return(tile.gameObject);
        }
        else
        {
            Debug.LogWarning("No pool found to return tile: " + tile.name);
            Destroy(tile.gameObject);
        }
    }
}
