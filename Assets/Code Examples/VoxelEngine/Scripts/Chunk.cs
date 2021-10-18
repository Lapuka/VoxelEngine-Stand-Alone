using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace VoxelEngineModule
{
    [Serializable]
    public class Chunk
    {
       // public bool inEdit; // true when thread is changing data
       // [field: NonSerialized] public World World { get; set; }
       
        //Block Data
        
        public int[,,] BlockId { get; set; }
        public Vector3Int[,,] Rotation { get; set; }       
        
        //Chunk Data 
        
        [NonSerialized] public GameObject root; // the root game object of the chunk

        [NonSerialized] public List<Vector3> vertices;
        [NonSerialized] public List<Vector2> uvs;
        [NonSerialized] public List<int> triangles;

        public Dictionary<Vector3, int> overflow;

        public Chunk[] Neighbors { set; get; }
        public bool HasAir { get; set; }
        public bool HasSolid { get; set; }
       

        private int positionx;
        private int positiony;
        private int positionz;

        private bool _dirty;

        private int _size; 

       
        public Chunk(int size, Vector3Int position)
        {    
            BlockId = new int[size, size, size];
            Rotation = new Vector3Int[size, size, size];           

            overflow = new Dictionary<Vector3, int>();
            Neighbors = new Chunk[6];           
            Position = position;
            _size = size;
        }
        public Vector3Int Position
        {
            get
            {
                return new Vector3Int(positionx, positiony, positionz);
            }

            set
            {
                this.positionx = value.x;
                this.positiony = value.y;
                this.positionz = value.z;
            }
        }

        public void SetDirty()
        {
            _dirty = true;
        }
        public void SetClean()
        {
            _dirty = false;
        }
        public bool IsDirty()
        {
            return _dirty;
        }
        public bool HasAllNeighbors()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Neighbors[i] == null)
                {
                    return false;
                }
               
            }
            return true;
        }

        public Vector3Int GetLocalBlockPosition(Vector3 point)
        {
            Vector3Int pos = new Vector3Int((int)(Mathf.Abs(Mathf.Abs(point.x) - Mathf.Abs(Position.x))), (int)(Mathf.Abs(Mathf.Abs(point.y) - Mathf.Abs(Position.y))), (int)(Mathf.Abs(Mathf.Abs(point.z) - Mathf.Abs(Position.z))));
            return pos;
        }
        /// <summary>
        /// Checks conditions if chunk needs to be considered for rendering or not
        /// </summary>
        /// <returns></returns>
        public bool EligibleForRendering(World world)
        {
            if (!HasAllNeighbors()) return false;
            if (!HasSolid) return false;
            if (!HasAir)
            {
                foreach (var item in Neighbors)
                {
                    if(item.HasAir)
                    {
                        
                        for (int x = 0; x < _size; x++)
                        {
                            for (int y = 0; y < _size; y++)
                            {
                                if (world.GetBlockData(item.BlockId[x, 0, y]).state != BlockData.State.Solid)
                                {                                    
                                    return true;
                                }
                            }
                        }                       
                       
                    }
                    
                }
                return false;

            }
            return true;
        }
    }
}