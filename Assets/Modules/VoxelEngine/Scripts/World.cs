using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;


namespace VoxelEngineModule
{
    public class World
    {
        public static int Total;

        public static Action OnWorldLoaded; //invoked when world is rendered for the first time
        public WorldSettings Settings { get; }
        public Dictionary<Vector3Int, Chunk> Chunks { get; }       

        internal Dictionary<Vector3Int, Chunk> VirtualChunks { get; set; }

        private List<BlockData> _blockData;
        private ChunkGenerator _chunkGenerator;
        private TerrainGenerator _terrainGenerator;
        private MeshGenerator _meshGenerator;
        private RenderEngine _renderEngine;
        public World(WorldSettings settings)
        {
            Settings = settings;
            Chunks = new Dictionary<Vector3Int, Chunk>();
            VirtualChunks = new Dictionary<Vector3Int, Chunk>();
            _chunkGenerator = new ChunkGenerator(this);
            _terrainGenerator = new TerrainGenerator(this);
            _meshGenerator = new MeshGenerator(this);          
            _blockData = new List<BlockData>(Resources.LoadAll<BlockData>(settings.blockDataFolder));

            GameObject renderEngine = new GameObject("Render Engine");
            _renderEngine = renderEngine.AddComponent<RenderEngine>();
            _renderEngine.settings = settings;

            ThreadPool.QueueUserWorkItem(a => Start(Vector3Int.zero));

        }      
       

      
       
        public void AddBlock(Vector3Int position, int id)
        {
            ThreadPool.QueueUserWorkItem(a => AddBlockThread(position, id));           
        }

        
        public void RemoveBlock(Vector3Int position)
        {
            ThreadPool.QueueUserWorkItem(a => RemoveBlockThread(position));
        }

       
        public void RotateBlock(Vector3Int position, Vector3Int rotation)
        {
            ThreadPool.QueueUserWorkItem(a => RotateBlockThread(position, rotation));
        }

     
        public Block GetBlock(Vector3Int position)
        {
            Block block = new Block();
            Chunk chunk = FindChunk(position.x, position.y, position.z);
            if (chunk != null)
            {
                Vector3Int localPosition = chunk.GetLocalBlockPosition(position);
                block.position = position;
                block.data = GetBlockData(chunk.BlockId[localPosition.x, localPosition.y, localPosition.z]);
                block.rotation = chunk.Rotation[localPosition.x, localPosition.y, localPosition.z];
                block.chunk = chunk;
                block.localPosition = localPosition;
                
            }else
            {
                Debug.Log($"Cant find chunk at: {position} ");
            }
            return block;
        }
      
        public Block FindSurfaceBlock(Vector3Int currentLocation)
        {
            Block block = GetBlock(currentLocation);
            Vector3Int direction = Vector3Int.down;
            if (block.data.state == BlockData.State.Air) direction = Vector3Int.down;
            else if (block.data.state == BlockData.State.Solid) direction = Vector3Int.up;

            bool found = false;
            while(!found)
            {
                if (block.data.state == BlockData.State.Solid && GetBlock(block.position + Vector3Int.up).data.state == BlockData.State.Air) return block;
                block = GetBlock(block.position + direction);
                
            } 

            return block;
        }
       
        public WorldStats GetWorldStatistics()
        {
            WorldStats stats = new WorldStats()
            {
                numberOfChunks = Chunks.Count,
                numberOfVirtualChunks = VirtualChunks.Count,
                numberOfChunksRendered = Chunks.Count(c => c.Value.root != null)
            };
            return stats;
        }

      
        // internal use methods

        internal Chunk FindChunk(float x, float y, float z)
        {
            Chunk chunk = null;
            Vector3Int closestPoint = new Vector3Int(Mathf.FloorToInt(x / Settings.chunkSize) * Settings.chunkSize, Mathf.FloorToInt(y / Settings.chunkSize) * Settings.chunkSize, Mathf.FloorToInt(z / Settings.chunkSize) * Settings.chunkSize);
            if (Chunks.TryGetValue(closestPoint, out chunk))
            {
                return chunk;
            }
            return chunk;
        }

        internal Chunk FindChunk(Vector3Int position)
        {
            Chunk chunk = null;
            int x = Mathf.FloorToInt((float)position.x / Settings.chunkSize) * Settings.chunkSize;
            int y = Mathf.FloorToInt((float)position.y / Settings.chunkSize) * Settings.chunkSize;
            int z = Mathf.FloorToInt((float)position.z / Settings.chunkSize) * Settings.chunkSize;          
            Vector3Int chunkBounds = new Vector3Int(x, y, z);
           
            if (Chunks.TryGetValue(chunkBounds, out chunk))
            {
                return chunk;
            }
            return chunk;
        }
        internal Chunk CreateVirtualChunk(Vector3Int chunkPoint)
        {
            return _chunkGenerator.CreateVirtualChunk(chunkPoint);
        }
        internal void RefreshNeighborChunks(Chunk current, Vector3Int pos)
        {
            Debug.Log($"Refreshing {pos}");
            if (pos.x == 0)
            {
                // Debug.Log($"Left: {pos} - {current.Position} - {pos + current.Position + Vector3Int.left}");
                RefreshChunk(FindChunk(pos + current.Position + Vector3Int.left));
            }
            if (pos.x == Settings.chunkSize - 1)
            {
                // Debug.Log($"Right: {pos} - {current.Position} - {pos + current.Position + Vector3Int.right}");
                RefreshChunk(FindChunk(pos + current.Position + Vector3Int.right));
            }
            if (pos.y == 0)
            {
                // Debug.Log($"Down: {pos} - {current.Position} - {pos + current.Position + Vector3Int.down}");
                RefreshChunk(FindChunk(pos + current.Position + Vector3Int.down));
            }
            if (pos.y == Settings.chunkSize - 1)
            {
                // Debug.Log($"Up: {pos} - {current.Position} - {pos + current.Position + Vector3Int.up}");
                RefreshChunk(FindChunk(pos + current.Position + Vector3Int.up));
            }
            if (pos.z == 0)
            {
                Debug.Log($"Back: {pos} - {current.Position} - {pos + current.Position + Vector3Int.back}");
                RefreshChunk(FindChunk(pos + current.Position + Vector3Int.back));
            }
            if (pos.z == Settings.chunkSize - 1)
            {
                // Debug.Log($"Forward: {pos} - {current.Position} - {pos + current.Position + Vector3Int.forward}");
                RefreshChunk(FindChunk(pos + current.Position + Vector3Int.forward));
            }
        }
        internal void RefreshChunk(Chunk chunk)
        {
            _meshGenerator.SetChunkGeometryNew(chunk);
            _renderEngine.AddToRenderQueue(chunk);
        }
        /// <summary>
        /// Find if there is virtual chunk, used by world generator before creating new chunk. (for internal logic)
        /// </summary>        
        internal Chunk FindVirtualChunk(float x, float y, float z)
        {
            Chunk chunk = null;
            Vector3Int closestPoint = new Vector3Int(Mathf.FloorToInt(x / Settings.chunkSize) * Settings.chunkSize, Mathf.FloorToInt(y / Settings.chunkSize) * Settings.chunkSize, Mathf.FloorToInt(z / Settings.chunkSize) * Settings.chunkSize);
            if (VirtualChunks.TryGetValue(closestPoint, out chunk))
            {
                return chunk;
            }
            return chunk;
        }
       
        /// <summary>
        /// Creates block (for internal logic)
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="localRotation"></param>
        /// <param name="id"></param>
        internal void CreateBlock(Chunk chunk, int x, int y, int z, Vector3Int localRotation, int id)
        {
            chunk.BlockId[x, y, z] = id;
            chunk.Rotation[x, y, z] = localRotation;          
          
        }

        internal BlockData GetBlockData(int id)
        {
            foreach (var data in _blockData)
            {
                if (data.id == id) return data;
            }

            return null;
        }

        private void Start(Vector3Int position)
        {
            Debug.Log($"Rendering from: {position}");
            GenerateChunks(this, position);
            RenderChunks();
            OnWorldLoaded.Invoke();
        }
        private void RenderChunks()
        {
            foreach (var chunk in Chunks)
            {
                if (chunk.Value != null)
                {
                    if (chunk.Value.IsDirty() || chunk.Value.EligibleForRendering(this)) RefreshChunk(chunk.Value);
                }

            }

            //foreach (var chunk in Chunks)
            //{
            //    if (chunk.Value.IsDirty()) _renderEngine.AddToRenderQueue(chunk.Value);
            //}
        }
        private void GenerateChunks(World world, Vector3Int worldCenter)
        {
            foreach (var item in GetGrid(worldCenter, world.Settings.generationRange))
            {
                if (!Chunks.ContainsKey(item))
                {
                   Chunk chunk = _chunkGenerator.CreateChunk(item);
                   Chunks.Add(chunk.Position, chunk);
                   _terrainGenerator.GenerateTerrain(chunk);
                }else
                {
                    Debug.Log("Redundant");
                }
            }
            
        }
        private Vector3Int[] GetGrid(Vector3Int playerPosition, int gridSize)
        {
            Vector3Int playerChunk = ToGridPoint(playerPosition);
            int gs = (gridSize * 2) + 1;
            Vector3Int[] grid = new Vector3Int[gs * gs * gs];
            int index = 0;
            for (int x = -gridSize; x <= gridSize; x++)
            {
                for (int y = -gridSize; y <= gridSize; y++)
                {
                    for (int z = -gridSize; z <= gridSize; z++)
                    {
                        // Debug.Log(index);
                        grid[index] = new Vector3Int(playerChunk.x + (x * Settings.chunkSize), playerChunk.y + (y * Settings.chunkSize), playerChunk.z + (z * Settings.chunkSize));
                        index++;
                    }
                }
            }

            return grid;
        }
        private Vector3Int ToGridPoint(Vector3Int playerPosition) // converts any point in 3d space to coordinates of a chunk it is inside of.
        {
            return new Vector3Int(Mathf.FloorToInt(playerPosition.x / Settings.chunkSize) * Settings.chunkSize, Mathf.FloorToInt(playerPosition.y / Settings.chunkSize) * Settings.chunkSize, Mathf.FloorToInt(playerPosition.z / Settings.chunkSize) * Settings.chunkSize);
        }
        private void RemoveBlockThread(Vector3Int position)
        {
            lock (VoxelEngineModuleManager.Instance.locker)
            {
                Chunk chunk = FindChunk(position.x, position.y, position.z);

                Vector3Int pos = chunk.GetLocalBlockPosition(position);

                //id 0 is Air block
                CreateBlock(chunk, pos.x, pos.y, pos.z, Vector3Int.zero, 0);
                RefreshChunk(chunk);
                RefreshNeighborChunks(chunk, pos);
            }
        }
        private void RotateBlockThread(Vector3Int position, Vector3Int rotation)
        {
            lock (VoxelEngineModuleManager.Instance.locker)
            {
                Chunk chunk = FindChunk(position.x, position.y, position.z);

                Vector3Int pos = chunk.GetLocalBlockPosition(position);

                //id 0 is Air block
                chunk.Rotation[pos.x, pos.y, pos.z] += rotation;
                RefreshChunk(chunk);
                RefreshNeighborChunks(chunk, pos);
            }
        }
        private void AddBlockThread(Vector3Int position, int id)
        {
            lock (VoxelEngineModuleManager.Instance.locker)
            {
                if (GetBlockData(id) != null)
                {
                    Chunk chunk = FindChunk(position.x, position.y, position.z);

                    Vector3Int pos = chunk.GetLocalBlockPosition(position);



                    if (chunk.BlockId[pos.x, pos.y, pos.z] == 0)
                    {
                        CreateBlock(chunk, pos.x, pos.y, pos.z, Vector3Int.zero, id);
                        RefreshChunk(chunk);
                        RefreshNeighborChunks(chunk, pos);
                    }
                    else
                    {
                        Debug.LogError("Block already exists at " + position);
                    }
                }
                else
                {
                    Debug.LogError($"Block with ID {id} was not found!");
                }
            }
        }

        //structures

        public struct WorldStats
        {
            public int numberOfChunks;
            public int numberOfChunksRendered;
            public int numberOfVirtualChunks;
        }

    }

}

