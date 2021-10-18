using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoxelEngineModule
{
    public struct Block
    {
        public Vector3Int position;
        public Vector3Int localPosition;
        public BlockData data;
        public Vector3Int rotation;
        public Chunk chunk;
       
    }

    public struct Neighbors
    {
        public Block left;
        public Block right;
        public Block forward;
        public Block back;
        public Block up;
        public Block down;
    }
      
}
