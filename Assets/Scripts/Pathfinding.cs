using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Pathfinding : MonoBehaviour
{
    public RectInt bounds;
    public LayerMask obstacleMask;
    private GridNode[,] Graph;

    private void OnEnable()
    {
        if(_pathfinders == null) _pathfinders = new List<Pathfinding>();
        _pathfinders.Add(this);
    }

    private void Start()
    {
        BuildGraph();
    }

    private static List<Pathfinding> _pathfinders;
    public static Pathfinding GetPathfinding(Vector2 pos)
    {
        return _pathfinders.Where(pathfinder => pathfinder.bounds.Contains(Vector2Int.RoundToInt(pos))).FirstOrDefault();
    }

    private void BuildGraph()
    {
        Graph = new GridNode[bounds.size.x,bounds.size.y];
        Debug.Log(Graph.GetLength(0));
        Debug.Log(Graph.GetLength(1));
        for (int i = 0; i < Graph.GetLength(0); i++)
        {
            for (int j = 0; j < Graph.GetLength(1); j++)
            {
                var worldPos = new Vector2(bounds.xMin + i, bounds.yMin + j);
                var isWalkable = Physics2D.OverlapCircleAll(worldPos + (Vector2.one * .5f), Mathf.Sqrt(2)/2, obstacleMask).Length == 0;
                if(!isWalkable) Debug.Log("Not walkable node at "+ new Vector2(i,j));
                Graph[i,j] = new GridNode(new Vector2Int(i,j), isWalkable);
            }
        }
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
        Debug.Log(start);
        Debug.Log(destination);
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
                Debug.Log(path.Count);
                
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
    
    private List<GridNode> FindNeighbors(GridNode node)
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
            if(neighbor.Walkable) neighbors.Add(neighbor);
        }

        return neighbors;
    }
    
    private class GridNode
    {
        public static int width;
        public static int height;
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

