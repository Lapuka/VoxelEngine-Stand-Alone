using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngineModule
{
    [CreateAssetMenu(fileName = "Block_", menuName = "Blocks/New Block")]
    public class BlockData : ScriptableObject
    {       
        public int id;
        public new string name;
        public string description;
        public State state; //0 air, 1 solid, 2 transparent, 3 solid and transparent 

        public Size size = new Size {
            xMax = 1,
            yMax = 1,
            zMax = 1
        };

        // Texture coordinates for each side from texture sheet
        public Vector2 texture_up;
        public Vector2 texture_down;
        public Vector2 texture_left;
        public Vector2 texture_right;
        public Vector2 texture_forward;
        public Vector2 texture_back;


        public enum State
        {
            Air, //empty
            Solid, //regular opaque blocks aka "Dirt"
            Transparent, //Transparent blocks like grass, smoke etc 
            TransparentSolid //Transparent but solid blocks like ice, glass etc
        }
        
       
        /// <summary>
        /// Size of a block wall, 1 is full block, 0.5f half size, 0 wont be rendered
        /// </summary>
        [System.Serializable]
        public struct Size
        {
            public float xMin;
            public float xMax;

            public float yMin;
            public float yMax;

            public float zMin;
            public float zMax;
        }

       
            
            public Vector2 GetTexture(Vector3 direction)
            {
            Vector2 texture = Vector2.zero;
           
            if (direction == Vector3.left)
            {

                texture = texture_left;
                return texture;
            }
            else if (direction == Vector3.right)
            {
                texture = texture_right;
                return texture;
            }
            else if (direction == Vector3.forward)
            {

                texture = texture_forward;
                return texture;
            }
            else if (direction == Vector3.back)
            {

                texture = texture_back;
                return texture;
            }
            else if (direction == Vector3.up)
            {

                texture = texture_up;
                return texture;
            }
            else if (direction == Vector3.down)
            {

                texture = texture_down;
                return texture;
            }
            return texture;
        }
        
    }
}