using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "Confugs/LevelData")]
public class LevelData : ScriptableObject
{
    public string seed = "Level";
    public int levelLength = 10;
}
