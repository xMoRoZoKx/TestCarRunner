using System.Collections;
using System.Collections.Generic;
using UniTools;
using UnityEngine;

public class RandomizedTileObject : WorldObject
{
    [SerializeField, Range(0, 100)] private float spawnChance = 50;
    [SerializeField] private List<WorldObject> randomObjects;
    public override void Init(string id)
    {
        base.Init(id);
        TryActivate(id);
    }
    public void TryActivate(string key)
    {
        gameObject.SetActive(RandomTools.GetDeterministicChance(spawnChance, key));
        randomObjects.ForEachWithIndexes((obj, idx) =>
        {
            obj.Init(key + idx);
        });
    }
    public virtual void Tick(float dt)
    {

    }
}
