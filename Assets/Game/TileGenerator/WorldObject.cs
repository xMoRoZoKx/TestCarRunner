using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : ConnectableMonoBehaviour
{
    public string id;

    public virtual void Init(string id)
    {
        this.id = id;
    }

}
