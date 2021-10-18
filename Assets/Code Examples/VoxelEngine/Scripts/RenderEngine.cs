using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VoxelEngineModule
{
    public class RenderEngine: MonoBehaviour
    {
        private Queue<Chunk> _renderQueue;
        private int _id;
        public WorldSettings settings;       
        private void Awake()
        {
            _renderQueue = new Queue<Chunk>();
          
        }
        private void Update()
        {
           if( Monitor.TryEnter(VoxelEngineModuleManager.Instance.locker))
            {
                try
                {
                    if (_renderQueue.Count > 0)
                    {                       
                        Render(_renderQueue.Dequeue());
                    }
                }
                finally
                {
                    Monitor.Exit(VoxelEngineModuleManager.Instance.locker);
                }
            }
        }
        /// <summary>
        /// Add chunk to render queue
        /// </summary>
        /// <param name="chunk"></param>
        public void AddToRenderQueue(Chunk chunk)
        {          
            _renderQueue.Enqueue(chunk);
        }
        private void Render(Chunk chunk)
        {
           
            Mesh mesh = new Mesh();
            mesh.vertices = chunk.vertices.ToArray();
            mesh.uv = chunk.uvs.ToArray();
            mesh.triangles = chunk.triangles.ToArray();

            mesh.Optimize();
            mesh.RecalculateNormals();
            GetGameObject(chunk).GetComponent<MeshFilter>().mesh = mesh;
            if (settings.generateMeshColliders) GetGameObject(chunk).GetComponent<MeshCollider>().sharedMesh = mesh;
            chunk.SetClean();
           
        }

        

        private GameObject GetGameObject(Chunk chunk)
        {
            GameObject g;
            if (chunk.root == null)
            {
                g = new GameObject(_id + " Chunk [" + chunk.Position.x + "] [" + chunk.Position.y + "] [" + chunk.Position.z + "] - v: [" + chunk.vertices.Count + "] t: [" + chunk.triangles.Count + "]");
                g.AddComponent<MeshFilter>();
                g.AddComponent<MeshRenderer>().material = settings.textureMaterial;
                if(settings.generateMeshColliders) g.AddComponent<MeshCollider>();
                chunk.root = g;
                g.transform.SetParent(transform);
                _id++;
            }
            else
            {
                g = chunk.root;
            }
            return g;
        }
    }
}
