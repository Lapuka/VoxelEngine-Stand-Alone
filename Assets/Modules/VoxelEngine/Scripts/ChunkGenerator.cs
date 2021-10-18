using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace VoxelEngineModule
{
    public class ChunkGenerator
    {
        
        private World _world;    
        public ChunkGenerator(World world)
        {
            _world = world;            
        }

        
        public Chunk CreateVirtualChunk(Vector3Int point)
        {
            Chunk chunk = new Chunk(_world.Settings.chunkSize, point);
            _world.VirtualChunks.Add(chunk.Position, chunk);
            return chunk;
        }
        public Chunk CreateChunk(Vector3Int point) // create new chunk
        {            
            Chunk chunk;
            if (!_world.VirtualChunks.TryGetValue(point, out chunk)) // checks if there is virtual chunk for this point
            { 
                chunk = new Chunk(_world.Settings.chunkSize, point);
                
            }else
            {
                _world.VirtualChunks.Remove(chunk.Position);
                
            }

            SetNeighbourChunks(chunk);           
            return chunk;
        }

        private void SetNeighbourChunks(Chunk thisChunk)
        {
            Vector3Int neigborPosition = new Vector3Int(thisChunk.Position.x + _world.Settings.chunkSize, thisChunk.Position.y, thisChunk.Position.z); // calculate position of neighbour chunk
            Chunk neigborChunk;

            if (_world.Chunks.TryGetValue(neigborPosition, out neigborChunk)) // check if there is neighbour chunk and if found update the neighbour status
            {
                thisChunk.Neighbors[1] = neigborChunk;
                neigborChunk.Neighbors[3] = thisChunk;

            }
            else
            {
                thisChunk.Neighbors[1] = null; // no neighbour chunk found, this chunk is on the edge of rendered terrain
            }


            neigborPosition = new Vector3Int(thisChunk.Position.x - _world.Settings.chunkSize, thisChunk.Position.y, thisChunk.Position.z); // calculate position of neighbour chunk

            if (_world.Chunks.TryGetValue(neigborPosition, out neigborChunk)) // check if there is neighbour chunk and if found update the neighbour status
            {
                thisChunk.Neighbors[3] = neigborChunk;
                neigborChunk.Neighbors[1] = thisChunk;

            }
            else
            {
                thisChunk.Neighbors[3] = null; // no neighbour chunk found, this chunk is on the edge of rendered terrain
            }


            neigborPosition = new Vector3Int(thisChunk.Position.x, thisChunk.Position.y, thisChunk.Position.z + _world.Settings.chunkSize); // calculate position of neighbour chunk

            if (_world.Chunks.TryGetValue(neigborPosition, out neigborChunk)) // check if there is neighbour chunk and if found update the neighbour status
            {
                thisChunk.Neighbors[2] = neigborChunk;
                neigborChunk.Neighbors[0] = thisChunk;

            }
            else
            {
                thisChunk.Neighbors[2] = null; // no neighbour chunk found, this chunk is on the edge of rendered terrain
            }


            neigborPosition = new Vector3Int(thisChunk.Position.x, thisChunk.Position.y, thisChunk.Position.z - _world.Settings.chunkSize); // calculate position of neighbour chunk

            if (_world.Chunks.TryGetValue(neigborPosition, out neigborChunk)) // check if there is neighbour chunk and if found update the neighbour status
            {
                thisChunk.Neighbors[0] = neigborChunk;
                neigborChunk.Neighbors[2] = thisChunk;

            }
            else
            {
                thisChunk.Neighbors[0] = null; // no neighbour chunk found, this chunk is on the edge of rendered terrain
            }


            neigborPosition = new Vector3Int(thisChunk.Position.x, thisChunk.Position.y + _world.Settings.chunkSize, thisChunk.Position.z); // calculate position of neighbour chunk

            if (_world.Chunks.TryGetValue(neigborPosition, out neigborChunk)) // check if there is neighbour chunk and if found update the neighbour status
            {
                thisChunk.Neighbors[4] = neigborChunk;
                neigborChunk.Neighbors[5] = thisChunk;

            }
            else
            {
                thisChunk.Neighbors[4] = null; // no neighbour chunk found, this chunk is on the edge of rendered terrain
            }


            neigborPosition = new Vector3Int(thisChunk.Position.x, thisChunk.Position.y - _world.Settings.chunkSize, thisChunk.Position.z); // calculate position of neighbour chunk

            if (_world.Chunks.TryGetValue(neigborPosition, out neigborChunk)) // check if there is neighbour chunk and if found update the neighbour status
            {
                thisChunk.Neighbors[5] = neigborChunk;
                neigborChunk.Neighbors[4] = thisChunk;

            }
            else
            {
                thisChunk.Neighbors[5] = null; // no neighbour chunk found, this chunk is on the edge of rendered terrain
            }


        }

        public void Rotate(Vector3 point, Vector3Int rotation)
        {
            
            Chunk chunk = _world.FindChunk(point.x, point.y, point.z);
            chunk.Rotation[(int)(point.x - chunk.Position.x), (int)(point.y - chunk.Position.y), (int)(point.z - chunk.Position.z)] += rotation;
           
            if (chunk.Rotation[(int)(point.x - chunk.Position.x), (int)(point.y - chunk.Position.y), (int)(point.z - chunk.Position.z)].x >= 4) chunk.Rotation[(int)(point.x - chunk.Position.x), (int)(point.y - chunk.Position.y), (int)(point.z - chunk.Position.z)].x = 0;
            if (chunk.Rotation[(int)(point.x - chunk.Position.x), (int)(point.y - chunk.Position.y), (int)(point.z - chunk.Position.z)].y >= 4) chunk.Rotation[(int)(point.x - chunk.Position.x), (int)(point.y - chunk.Position.y), (int)(point.z - chunk.Position.z)].y = 0;
            if (chunk.Rotation[(int)(point.x - chunk.Position.x), (int)(point.y - chunk.Position.y), (int)(point.z - chunk.Position.z)].z >= 4) chunk.Rotation[(int)(point.x - chunk.Position.x), (int)(point.y - chunk.Position.y), (int)(point.z - chunk.Position.z)].z = 0;
          
        }      
        
    }
}