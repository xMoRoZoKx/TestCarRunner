using System.Collections.Generic;
using UniTools;
using UnityEngine;

public class TileView : MonoBehaviour
{
    public int TileIndex { get; private set; }

    [SerializeField] private List<RandomizedTileObject> randomObjects;
    [SerializeField] private PatchSystem patchSystem;
    public PatchSystem PatchSystem => patchSystem;

    public void Init(string seed, int index)
    {
        TileIndex = index;
        randomObjects.ForEachWithIndexes((obj, idx) =>
        {
            string uKey = seed + index + idx;
            obj.Init(uKey);
        });
    }
    public void Tick(float dt)
    {
        foreach (var randomObject in randomObjects)
        {
            if (randomObject == null) continue;
            randomObject.Tick(dt);
        }
    }
}
