using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
namespace VoxelEngineModule
{
    public class TerrainGenerator
    {
        private World _world;
        private System.Random _random;
        public TerrainGenerator(World world)
        {
            _world = world;           
        }      
       
        private int GenerateSurface(float x, float z, float scale, int seed, float height)
        {
            return (int)(_world.Settings.surfaceHeight + Perlin2D(x, z, scale, seed) * height);
        }

        public void GenerateTerrain(Chunk chunk)
        {            
            for (int x = 0; x < _world.Settings.chunkSize; x++)
            {
                for (int y = 0; y < _world.Settings.chunkSize; y++)
                {
                    for (int z = 0; z < _world.Settings.chunkSize; z++)
                    {
                        //assign default amount of bits for each block
                        
                        if (chunk.BlockId[x, y, z] == 0) // assign block only if block is not assigned yet 
                        {
                            Vector3 blockPosition = new Vector3(chunk.Position.x + x, chunk.Position.y + y, chunk.Position.z + z);                           
                            int surface = GenerateSurface(blockPosition.x, blockPosition.z, _world.Settings.perlinModifier, _world.Settings.seed, _world.Settings.perlinMultiplier);
                            
                            if (blockPosition.y > surface) //air
                            {
                                _world.CreateBlock(chunk, x,y,z, Vector3Int.zero, 0);                               
                                chunk.HasAir = true;
                            }
                            else if (blockPosition.y == surface) // surface
                            {                               
                                
                                
                                    _world.CreateBlock(chunk, x, y, z, Vector3Int.zero, 2);
                                    chunk.HasSolid = true;
                                
                            }
                            else if (blockPosition.y < surface) // underground
                            {
                                _world.CreateBlock(chunk, x, y, z, Vector3Int.zero, 3);
                                chunk.HasSolid = true;
                            }

                        }
                    }
                }
            }
            
           
            ApplyOverflow(chunk.overflow);
            
            
        }

        private float Perlin3D(float x, float y, float z, float scale, float offset)
        {
            float ab = Mathf.PerlinNoise((offset + x) * scale, (offset + y) * scale);
            float bc = Mathf.PerlinNoise((offset + y) * scale, (offset + z) * scale);
            float ac = Mathf.PerlinNoise((offset + x) * scale, (offset + z) * scale);

            float ba = Mathf.PerlinNoise((offset + y) * scale, (offset + x) * scale);
            float cb = Mathf.PerlinNoise((offset + z) * scale, (offset + y) * scale);
            float ca = Mathf.PerlinNoise((offset + z) * scale, (offset + x) * scale);

            float abc = ab + bc + ac + ba + cb + ca;
            return abc / 6f;
        }
        private float Perlin2D(float x, float z, float scale, float offset)
        {
            float perlin;
            perlin = Mathf.PerlinNoise((offset + x) * scale, (offset + z) * scale);
            return perlin;
        }

        private void ApplyOverflow(Dictionary<Vector3, int> overflow)
        {
            Chunk chunk;
            foreach (var item in overflow)
            {
                chunk = _world.FindChunk(item.Key.x, item.Key.y, item.Key.z);
                if (chunk == null)
                {
                    chunk = _world.FindVirtualChunk(item.Key.x, item.Key.y, item.Key.z);
                    if (chunk == null)
                    {
                        Vector3Int chunkPoint = new Vector3Int(Mathf.FloorToInt(item.Key.x / _world.Settings.chunkSize) * _world.Settings.chunkSize, Mathf.FloorToInt(item.Key.y / _world.Settings.chunkSize) * _world.Settings.chunkSize, Mathf.FloorToInt(item.Key.z / _world.Settings.chunkSize) * _world.Settings.chunkSize);
                        chunk = _world.CreateVirtualChunk(chunkPoint);
                        chunk.HasAir = true;
                        chunk.HasSolid = true;
                    }
                }
                _world.CreateBlock(chunk, (int)(item.Key.x - chunk.Position.x), (int)(item.Key.y - chunk.Position.y), (int)(item.Key.z - chunk.Position.z), Vector3Int.zero, item.Value);                
            }
        }

        private float GetRandomValue(int seed, int x, int y, int z)
        {
            float value = Mathf.PerlinNoise(seed * x + z, seed * y + z);
            return value;
        }
    }
}