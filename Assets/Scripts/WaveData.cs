using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WaveData
{
    public int wave = 0;
    public List<GameObject> enemyPrefab;
    public int level = 1;
    public float interval = 3;
}
