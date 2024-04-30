using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[ShowOdinSerializedPropertiesInInspector]
[CreateAssetMenu(fileName = "TempMapData", menuName = "Map Data", order = 0)]
public class CurrentState : ScriptableObject
{
    public List<Vector3Int> Dirs = new List<Vector3Int>()
    {
        new Vector3Int(1, 0), new Vector3Int(-1, 0), new Vector3Int(0, 1), new Vector3Int(0, -1)
    };

    public Dictionary<Vector3Int, GridTileBase> TileList = new Dictionary<Vector3Int, GridTileBase>();
    public List<List<Vector3Int>> Lands;
    public GridTileBase[] TileBases;
}