using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace GridSystem
{
    public class GridSystem : MonoBehaviour
    {
        private Grid _grid;
        private Camera _cam;
        [SerializeField] private Vector2Int _size;
        [SerializeField, Range(0, 1)] private float _landAmount = 0.3f;
        public CurrentState CurrentState;
        [SerializeField] private Vector3Int _a;
        [SerializeField] private Vector3Int _b;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            _cam = Camera.main;
            Generate();
            CurrentState.Lands = FindAllIslands();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log(isLandConnected(_a, _b));
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                int landAIndex = CurrentState.TileList[_a].LandIndex;
                int landBIndex = CurrentState.TileList[_b].LandIndex;
                Debug.Log(ComputeIslandDistance(landAIndex, landBIndex));
            }
        }

        //时间复杂度：O(n^2), 两个for循环生成coordinates，然后用coordinates生成prefab，利用Grid中的GetCellCenterWorld
        private void Generate()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            var coordinates = new List<Vector3Int>();

            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    coordinates.Add(new Vector3Int(x, y));
                }
            }

            var LandCount = Mathf.FloorToInt(coordinates.Count * _landAmount);
            var index = 0;
            var rand = new Random(300);

            foreach (var coordinate in coordinates.OrderBy(t => rand.Next()))
            {
                var isLand = index++ < LandCount;
                var prefab = isLand ? CurrentState.TileBases[1] : CurrentState.TileBases[0];
                var position = _grid.GetCellCenterWorld(coordinate);
                var spawned = Instantiate(prefab, position, Quaternion.identity, transform);
                spawned.name = spawned.name + $"({coordinate.x}, {coordinate.y})";
                CurrentState.TileList.Add(coordinate, spawned);
            }
        }

        //时间复杂度：O(n^2), for循环内部调用FindNeighborLand(tile.Key, visited, land);, 这个method内部又有一个for循环
        private List<List<Vector3Int>> FindAllIslands()
        {
            var visited = new List<Vector3Int>();
            var lands = new List<List<Vector3Int>>();
            foreach (var tile in CurrentState.TileList)
            {
                if (tile.Value.LandType == LandType.Land && !visited.Contains(tile.Key))
                {
                    var land = new List<Vector3Int>();
                    FindNeighborIsLand(tile.Key, visited, land);
                    lands.Add(land);
                    foreach (var landCoordinate in land)
                    {
                        CurrentState.TileList[landCoordinate].Init(lands.IndexOf(land));
                    }
                }
            }

            return lands;
        }

        // O(n), 源于FindNeighborLand(a, visited, land);
        private bool isLandConnected(Vector3Int a, Vector3Int b)
        {
            var visited = new List<Vector3Int>();
            var land = new List<Vector3Int>();
            FindNeighborIsLand(a, visited, land);
            return land.Contains(b);
        }

        // O(n^2), 源于后面两个for循环嵌套.
        private float ComputeIslandDistance(int a, int b)
        {
            var lands = FindAllIslands();
            var landA = lands[a];
            var landB = lands[b];
            var minDistance = float.MaxValue;
            foreach (var tileA in landA)
            {
                foreach (var tileB in landB)
                {
                    var distance = Vector3Int.Distance(tileA, tileB);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
            }

            return minDistance;
        }

        //O(n), foreach循环
        private void FindNeighborIsLand(Vector3Int tileKey, List<Vector3Int> visited, List<Vector3Int> land)
        {
            if (visited.Contains(tileKey)) return;
            visited.Add(tileKey);
            land.Add(tileKey);
            foreach (var dir in CurrentState.Dirs)
            {
                var neighbor = tileKey + dir;
                if (CurrentState.TileList.ContainsKey(neighbor) &&
                    CurrentState.TileList[neighbor].LandType == LandType.Land)
                {
                    FindNeighborIsLand(neighbor, visited, land);
                }
            }
        }
    }
}