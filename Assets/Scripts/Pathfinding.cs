using System;
using System.Collections.Generic;
using System.Linq;
using Environment;
using UnityEngine;
public class Pathfinding : MonoBehaviour
{
    public RectInt bounds;
    public LayerMask obstacleMask;
    private GridNode[,] Graph;

    private void Awake()
    {
        if(_pathfinders == null) _pathfinders = new List<Pathfinding>();
        _pathfinders.Add(this);
    }

    private void Start()
    {
        BuildGraph();
    }

    private static List<Pathfinding> _pathfinders;
    public static Pathfinding GetPathfinding()
    {
        return Map.Instance.current.pathfinding;
    }

    public bool PathExist(Vector2 start, Vector2 destination)
    {
        if(!bounds.Contains(Vector2Int.RoundToInt(start)) || !bounds.Contains(Vector2Int.RoundToInt(destination)))
        {
            return false;
        }

        if (!Graph[(int) start.x - bounds.xMin, (int) start.y - bounds.yMin].Walkable) return false;
        return GetPath(start, destination) == null;
    }
    private void BuildGraph()
    {
        Graph = new GridNode[bounds.size.x,bounds.size.y];
        for (int i = 0; i < Graph.GetLength(0); i++)
        {
            for (int j = 0; j < Graph.GetLength(1); j++)
            {
                var worldPos = new Vector2(bounds.xMin + i, bounds.yMin + j);
                var isWalkable = Physics2D.OverlapCircleAll(worldPos + (Vector2.one * .5f), Mathf.Sqrt(2)/2, obstacleMask).Length == 0;
                Graph[i,j] = new GridNode(new Vector2Int(i,j), isWalkable);
            }
        }
    }

    public Vector2Int FindNearestWalkable(Vector2 target)
    {
        Queue<GridNode> nodeQueue = new Queue<GridNode>();
        nodeQueue.Enqueue(Graph[(int)target.x - bounds.xMin, (int)target.y - bounds.yMin]);
        while (nodeQueue.Count > 0)
        {
            var current = nodeQueue.Dequeue();
            if (current.Walkable) return current.Coords + new Vector2Int(bounds.xMin, bounds.yMin);
            foreach (var node in FindNeighbors(current,true))
            {
                nodeQueue.Enqueue(node);
            }
        }

        return Vector2Int.zero;
    }

    public List<Vector2> GetPath(Vector2 worldStart, Vector2 worldDestination)
    {
        if(!bounds.Contains(Vector2Int.RoundToInt(worldDestination)) || !bounds.Contains(Vector2Int.RoundToInt(worldStart)))
        {
            Debug.LogError("Invalid destination or location");
            return null;
        }
        return FindPath(new Vector2Int((int)worldStart.x - bounds.xMin, (int)worldStart.y - bounds.yMin),
            new Vector2Int((int)worldDestination.x - bounds.xMin, (int)worldDestination.y - bounds.yMin));
    }
    
    private List<Vector2> FindPath(Vector2Int start, Vector2Int destination)
    {
        var openList = new List<GridNode>();
        var closedList = new List<GridNode>();
        openList.Add(Graph[start.x, start.y]);
        while (openList.Count > 0)
        {
            GridNode current = openList[0];
            foreach (var node in openList)
            {
                if (node.FCost < current.FCost) current = node;
            }

            openList.Remove(current);
            closedList.Add(current);
            
            var neighbors = FindNeighbors(current);
            foreach (var neighbor in neighbors)
            {
                if (closedList.Contains(neighbor)) continue;
                if (!openList.Contains(neighbor))
                {
                    neighbor.GCost = Mathf.Max(neighbor.GCost, current.GCost + 1);
                    neighbor.HCost = Mathf.Max(neighbor.HCost, Vector2.Distance(neighbor.Coords, destination));
                    neighbor.FCost = neighbor.GCost + neighbor.HCost;
                    neighbor.LastNode = current;
                    openList.Add(neighbor);
                }
                else
                {
                    if (neighbor.GCost + neighbor.HCost > neighbor.FCost)
                    {
                        neighbor.FCost = neighbor.GCost + neighbor.HCost;
                        neighbor.LastNode = current;
                    }
                }
            }
            if (current.Coords == destination)
            {
                var path = new List<Vector2>();
                var node = current;
                while (node.LastNode != null)
                {
                    path.Add(node.Coords+ new Vector2Int(bounds.xMin, bounds.yMin));
                    node = node.LastNode;
                }
                ClearGraph();
                path.Reverse();
//                Debug.Log(path.Count);
                
                return path;
            }
        }
        
        return null;
    }

    private void ClearGraph()
    {
        for (int i = 0; i < Graph.GetLength(0); i++)
        {
            for (int j = 0; j < Graph.GetLength(1); j++)
            {
                Graph[i, j].FCost = 0;
                Graph[i, j].GCost = 0;
                Graph[i, j].HCost = 0;
                Graph[i, j].LastNode = null;
            }
        }
    }
    
    private List<GridNode> FindNeighbors(GridNode node, bool includeUnwalkable = false)
    {
        var coords = node.Coords;
        var neighbors = new List<GridNode>(8);
        var smeryY = new[] {0, 1, 1, 1, 0,-1,-1,-1};
        var smeryX = new []{1, 1, 0,-1,-1,-1, 0, 1};
        for (int i = 0; i < smeryX.Length; i++)
        {
            if(coords.x + smeryX[i] < 0 || coords.x + smeryX[i] >= Graph.GetLength(0)) continue;
            if(coords.y + smeryY[i] < 0 || coords.y + smeryY[i] >= Graph.GetLength(1)) continue;
            var neighbor = Graph[coords.x + smeryX[i], coords.y + smeryY[i]];
            if(!neighbor.Walkable && !includeUnwalkable) continue; 
            neighbors.Add(neighbor);
        }

        return neighbors;
    }
    
    private class GridNode
    {
        public Vector2Int Coords;
        public GridNode LastNode;
        public bool Walkable;
        public float FCost;
        public float GCost;
        public float HCost;
        
        public GridNode(Vector2Int coords, bool walkable)
        {
            Coords = coords;
            Walkable = walkable;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
       
        Gizmos.DrawWireCube(bounds.center,  new Vector3(bounds.size.x, bounds.size.y, 2));
        if (Graph == null) return;
        for (int i = 0; i < Graph.GetLength(0); i++)
        {
            for (int j = 0; j < Graph.GetLength(1); j++)
            {
                Gizmos.color = Graph[i, j].Walkable ? Color.white : Color.red;
                Gizmos.DrawWireSphere(new Vector3(i + bounds.xMin,j + bounds.yMin), .2f);
            }
        }
    }
}

