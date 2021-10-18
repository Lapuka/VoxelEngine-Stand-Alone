using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngineModule
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Misc/Settings")]
    public class WorldSettings : ScriptableObject
    {
        
        public int seed = 42;

        [Tooltip("How many blocks make up 1 chunk.")]
        public int chunkSize = 16;

        [Tooltip("How many chunks will be generated to every direction.")]
        public int generationRange = 6;

        [Tooltip("Generates mesh colliders for chunks (Might reduce FPS).")]
        public bool generateMeshColliders = false;

        [Tooltip("Hill width.")]
        public float perlinModifier = 0.1f;

        [Tooltip("Hill height.")]
        public float perlinMultiplier = 5f;

        [Tooltip("Average Y coordinate of surface blocks.")]
        public int surfaceHeight = 11;

        [Tooltip("Material with block texture atlas.")]
        public Material textureMaterial;

        [Tooltip("Size of 1 texture relative to whole texture atlas.")]
        public float textureSize = 0.03125f;

        [Tooltip("Path to a folder in Resources containing all block scriptable objects.")]
        public string blockDataFolder = "Blocks";
    }

}