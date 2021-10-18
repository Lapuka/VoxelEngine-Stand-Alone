using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using VoxelEngineModule;
using System.Linq;

public class AStar
{ 
    
    private bool _onlyAdjacent;   

    private Dictionary<Vector3Int, float> _open;
    private HashSet<Vector3Int> _closed;
    private Dictionary<Vector3Int, Vector3Int> _parents; //child, parent
   
    private VoxelEngineModuleManager _voxelManager;
    public AStar(bool onlyAdjacent)
    {      
        _voxelManager = VoxelEngineModuleManager.Instance;
        _onlyAdjacent = onlyAdjacent;
    }   

    /// <summary>
    /// Returns Queue with path node locations
    /// </summary>
    /// <param name="start">Path Beginning</param>
    /// <param name="finish">Path End</param>
    /// <returns></returns>
    public Queue<Vector3Int> GetPath(Vector3Int start, Vector3Int finish)
    {     
       
        Queue<Vector3Int> path = new Queue<Vector3Int>();
        _parents = new Dictionary<Vector3Int, Vector3Int>();
        _open = new Dictionary<Vector3Int, float>();
        _closed = new HashSet<Vector3Int>();

        Vector3Int current = start;
        _open.Add(current, Vector3Int.Distance(current, finish));
        while(_open.Count > 0)
        {
            if(current == finish)
            {
                Vector3Int p = finish;
                path.Enqueue(p);
                while (p != start)
                {
                    p = _parents[p];
                    path.Enqueue(p);
                              
                }
              
                path = new Queue<Vector3Int>(path.Reverse());
                return path;

            }
            else
             {
                _closed.Add(current);
              
                _open.Remove(current);
                AddNeighbors(current, finish);
                current = GetClosest(current, finish); 
                
            }
        }

        return path;
    }
    /// <summary>
    /// Returns Queue with path node locations, on any viable block around target
    /// </summary>
    /// <param name="start"></param>
    /// <param name="finish"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Queue<Vector3Int> GetPath(Vector3Int start, Vector3Int finish, int distance)
    {
       
        finish = GetClosestViableNeighbor(start, finish, distance);
        Queue<Vector3Int> path = new Queue<Vector3Int>();
        _parents = new Dictionary<Vector3Int, Vector3Int>();
        _open = new Dictionary<Vector3Int, float>();
        _closed = new HashSet<Vector3Int>();

        Vector3Int current = start;
        _open.Add(current, Vector3Int.Distance(current, finish));
        while (_open.Count > 0)
        {
            if (current == finish)
            {
                Vector3Int p = finish;
                path.Enqueue(p);
                while (p != start)
                {
                    p = _parents[p];
                    path.Enqueue(p);                   
                }
               
                path = new Queue<Vector3Int>(path.Reverse());
                return path;

            }
            else
            {
                _closed.Add(current);             
                _open.Remove(current);
                AddNeighbors(current, finish);
                current = GetClosest(current, finish);

            }
        }

        return path;
    }

    private Vector3Int GetClosestViableNeighbor(Vector3Int start, Vector3Int target, int distance)
    {
        Vector3Int closest = target;
        float lastDistance = 99999999;
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                for (int z = -distance; z <= distance; z++)
                {
                    Vector3Int evaluated = target + new Vector3Int(x, y, z);
                    if (IsTraversable(evaluated) && evaluated != target)
                    {
                        float newDistance = Vector3Int.Distance(evaluated, start);
                        if (newDistance < lastDistance)
                        {
                            lastDistance = newDistance;
                            closest = evaluated;
                        }
                    }
                }
            }
        }
        return closest;
    }
    private void AddNeighbors(Vector3Int current, Vector3Int finish)
    {        
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int neigborPosition;
                    if (_onlyAdjacent)
                    {
                        if (x == 0 && y == 0 || y == 0 && z == 0 || z == 0 && x == 0) // only adjacent
                        {
                            neigborPosition = current + new Vector3Int(x, y, z);
                            AddNeighbor(neigborPosition, current, finish);
                        }
                    }else
                    {
                        neigborPosition = current + new Vector3Int(x, y, z);
                        AddNeighbor(neigborPosition, current, finish);
                    }
                }
            }
        }        
    }
   
    private void AddNeighbor(Vector3Int neigborPosition, Vector3Int current, Vector3Int finish)
    {
        
        if (IsTraversable(neigborPosition))
        {
            if (!_closed.Contains(neigborPosition))
            {
                if (!_open.ContainsKey(neigborPosition))
                {
                    float distance = Vector3Int.Distance(neigborPosition, finish);
                    _open.Add(neigborPosition, distance);
                    _parents.Add(neigborPosition, current);
                }
            }
        }
    }
    private Vector3Int GetClosest(Vector3Int current, Vector3Int finish)
    {
        Vector3Int closest = current;
        foreach (var item in _open)
        {
            if (closest == current || item.Value < _open[closest])
            {
                closest = item.Key;
            }
        }
        return closest;
    }
    private bool IsTraversable(Vector3Int position)
    {
        Block block = _voxelManager.World.GetBlock(position);
        Block topBlock = _voxelManager.World.GetBlock(position + Vector3Int.up);
        if (block.data.state == BlockData.State.Solid && topBlock.data.state == BlockData.State.Air) return true;
        else return false;
    }
    
   
}
