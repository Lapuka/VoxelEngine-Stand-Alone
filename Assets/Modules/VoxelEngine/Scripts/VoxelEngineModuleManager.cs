using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngineModule
{
    public class VoxelEngineModuleManager : MonoBehaviour
    {
        public static VoxelEngineModuleManager Instance { get; private set; }
        public World World { get; private set; }

        [SerializeField] private WorldSettings _settings;

        internal object locker = new object();
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Voxel Engine already exist, there can be only one Instance");
                GameObject.Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            World = new World(_settings);
        }
    }
}
