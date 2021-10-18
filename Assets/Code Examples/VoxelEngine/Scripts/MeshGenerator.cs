using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngineModule
{ 
public class MeshGenerator
{
    World _world;

       public  MeshGenerator(World world)
        {
            _world = world;
        }

        bool isRendered = false;
        public void SetChunkGeometryNew(Chunk chunk)
        {
            chunk.triangles = new List<int>();
            chunk.uvs = new List<Vector2>();
            chunk.vertices = new List<Vector3>();

            for (int x = 0; x < _world.Settings.chunkSize; x++)
            {
                for (int y = 0; y < _world.Settings.chunkSize; y++)
                {
                    for (int z = 0; z < _world.Settings.chunkSize; z++)
                    {
                        int id = chunk.BlockId[x, y, z];
                        BlockData currentData = _world.GetBlockData(id);
                        isRendered = false;

                        if (currentData.state == BlockData.State.Solid)
                        {
                            Block block;
                            Vector3Int direction;
                            // face render procedure
                            block = _world.GetBlock(new Vector3Int(x + 1, y, z) + chunk.Position); //Finds a block to the right of current block 
                            direction = Vector3Int.right; // sets default face direction if block is not rotated, in this case its right
                            if (chunk.Rotation[x, y, z] != Vector3Int.zero) //checks if there is rotation on current block (optimization)
                            {
                                //calculates where is the side that after rotation will be facing to right side .. each axis is calculated independently
                                direction = Rotate3D(Vector3Int.right, new Vector3Int(chunk.Rotation[x, y, z].x, 0, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, chunk.Rotation[x, y, z].y, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, 0, chunk.Rotation[x, y, z].z) * -1);
                            }
                            //if block to the right is air (empty) render the right side (or if its rotated render that side that will be facing right after rotation is applied)
                            if (block.data.state == BlockData.State.Air) BuildFace(chunk, GetDefaultVertices(currentData.size.xMin, currentData.size.xMax, currentData.size.yMin, currentData.size.yMax, currentData.size.zMin, currentData.size.zMax, direction), currentData.GetTexture(direction), new Vector3Int(x, y, z) + chunk.Position, chunk.Rotation[x, y, z]);

                            //next face, same procedure
                            block = _world.GetBlock(new Vector3Int(x - 1, y, z) + chunk.Position);
                            direction = Vector3Int.left;
                            if (chunk.Rotation[x, y, z] != Vector3Int.zero)
                            {
                                direction = Rotate3D(Vector3Int.left, new Vector3Int(chunk.Rotation[x, y, z].x, 0, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, chunk.Rotation[x, y, z].y, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, 0, chunk.Rotation[x, y, z].z) * -1);
                            }
                            if (block.data.state == BlockData.State.Air) BuildFace(chunk, GetDefaultVertices(currentData.size.xMin, currentData.size.xMax, currentData.size.yMin, currentData.size.yMax, currentData.size.zMin, currentData.size.zMax, direction), currentData.GetTexture(direction), new Vector3Int(x, y, z) + chunk.Position, chunk.Rotation[x, y, z]);

                            block = _world.GetBlock(new Vector3Int(x, y + 1, z) + chunk.Position);
                            direction = Vector3Int.up;
                            if (chunk.Rotation[x, y, z] != Vector3Int.zero)
                            {
                                direction = Rotate3D(Vector3Int.up, new Vector3Int(chunk.Rotation[x, y, z].x, 0, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, chunk.Rotation[x, y, z].y, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, 0, chunk.Rotation[x, y, z].z) * -1);
                            }
                            if (block.data.state == BlockData.State.Air) BuildFace(chunk, GetDefaultVertices(currentData.size.xMin, currentData.size.xMax, currentData.size.yMin, currentData.size.yMax, currentData.size.zMin, currentData.size.zMax, direction), currentData.GetTexture(direction), new Vector3Int(x, y, z) + chunk.Position, chunk.Rotation[x, y, z]);

                            block = _world.GetBlock(new Vector3Int(x, y - 1, z) + chunk.Position);
                            direction = Vector3Int.down;
                            if (chunk.Rotation[x, y, z] != Vector3Int.zero)
                            {
                                direction = Rotate3D(Vector3Int.down, new Vector3Int(chunk.Rotation[x, y, z].x, 0, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, chunk.Rotation[x, y, z].y, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, 0, chunk.Rotation[x, y, z].z) * -1);
                            }
                            if (block.data.state == BlockData.State.Air) BuildFace(chunk, GetDefaultVertices(currentData.size.xMin, currentData.size.xMax, currentData.size.yMin, currentData.size.yMax, currentData.size.zMin, currentData.size.zMax, direction), currentData.GetTexture(direction), new Vector3Int(x, y, z) + chunk.Position, chunk.Rotation[x, y, z]);


                            block = _world.GetBlock(new Vector3Int(x, y, z + 1) + chunk.Position);
                            direction = Vector3Int.forward;
                            if (chunk.Rotation[x, y, z] != Vector3Int.zero)
                            {
                                direction = Rotate3D(Vector3Int.forward, new Vector3Int(chunk.Rotation[x, y, z].x, 0, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, chunk.Rotation[x, y, z].y, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, 0, chunk.Rotation[x, y, z].z) * -1);
                            }
                            if (block.data.state == BlockData.State.Air) BuildFace(chunk, GetDefaultVertices(currentData.size.xMin, currentData.size.xMax, currentData.size.yMin, currentData.size.yMax, currentData.size.zMin, currentData.size.zMax, direction), currentData.GetTexture(direction), new Vector3Int(x, y, z) + chunk.Position, chunk.Rotation[x, y, z]);


                            block = _world.GetBlock(new Vector3Int(x, y, z - 1) + chunk.Position);
                            direction = Vector3Int.back;
                            if (chunk.Rotation[x, y, z] != Vector3Int.zero)
                            {
                                direction = Rotate3D(Vector3Int.back, new Vector3Int(chunk.Rotation[x, y, z].x, 0, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, chunk.Rotation[x, y, z].y, 0) * -1);
                                direction = Rotate3D(direction, new Vector3Int(0, 0, chunk.Rotation[x, y, z].z) * -1);
                            }
                            if (block.data.state == BlockData.State.Air) BuildFace(chunk, GetDefaultVertices(currentData.size.xMin, currentData.size.xMax, currentData.size.yMin, currentData.size.yMax, currentData.size.zMin, currentData.size.zMax, direction), currentData.GetTexture(direction), new Vector3Int(x, y, z) + chunk.Position, chunk.Rotation[x, y, z]);
                        }
                        if (isRendered) World.Total++;
                    }
                }
            }
            chunk.SetDirty();
            
         }

        
       

    private Vector3Int Rotate3D(Vector3Int side, Vector3 rotation)
    {
        Vector2 zRot = RotatePoint(side.x, side.y, 0f, 0f, 90 * rotation.z);
        side = new Vector3Int((int)zRot.x, (int)zRot.y, (int)side.z);

        Vector2 yRot = RotatePoint(side.x, side.z, 0f, 0f, 90 * rotation.y);
        side = new Vector3Int((int)yRot.x, (int)side.y, (int)yRot.y);

        Vector2 xRot = RotatePoint(side.y, side.z, 0f, 0f, 90 * rotation.x);
        side = new Vector3Int((int)side.x, (int)xRot.x, (int)xRot.y);

        return side;
    }
    private Vector2 RotatePoint(float pointX, float pointY, float originX, float originY, float angle) // rotates 2D point around set pivot
    {
        angle = angle * Mathf.PI / 180.0f;

        float x = Mathf.Cos(angle) * (pointX - originX) - Mathf.Sin(angle) * (pointY - originY) + originX;
        float y = Mathf.Sin(angle) * (pointX - originX) + Mathf.Cos(angle) * (pointY - originY) + originY;
        return new Vector2(x, y);
    }
    private void BuildFace(Chunk chunk, Vector3[] side, Vector2 textureCoordinates, Vector3 block, Vector3 rotation)
    {
        isRendered = true;
        GetTris(chunk);
        GetUV(textureCoordinates, chunk);

        if (rotation != Vector3.zero)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 zRot = RotatePoint(side[i].x, side[i].y, 0.5f, 0.5f, 90 * rotation.z);
                side[i] = new Vector3(zRot.x, zRot.y, side[i].z);

                Vector2 yRot = RotatePoint(side[i].x, side[i].z, 0.5f, 0.5f, 90 * rotation.y);
                side[i] = new Vector3(yRot.x, side[i].y, yRot.y);

                Vector2 xRot = RotatePoint(side[i].y, side[i].z, 0.5f, 0.5f, 90 * rotation.x);
                side[i] = new Vector3(side[i].x, xRot.x, xRot.y);

                chunk.vertices.Add(block + side[i]);

            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                chunk.vertices.Add(block + side[i]);

            }
        }
    }

    private void GetUV(Vector2 textureCoorinates, Chunk chunk) // all UVs are the same for each side
    {
        Vector2 c = new Vector2(_world.Settings.textureSize * textureCoorinates.x, _world.Settings.textureSize * textureCoorinates.y);
        float offset = _world.Settings.textureSize / 16f; // leave a border around texture to remove bleeding effect to other textures

        chunk.uvs.Add(new Vector2(c.x + offset, c.y + offset));
        chunk.uvs.Add(new Vector2(c.x + offset, (c.y + _world.Settings.textureSize) - offset));
        chunk.uvs.Add(new Vector2((c.x + _world.Settings.textureSize) - offset, (c.y + _world.Settings.textureSize) - offset));
        chunk.uvs.Add(new Vector2((c.x + _world.Settings.textureSize) - offset, c.y + offset));

    }

    private void GetTris(Chunk chunk)
    {
        chunk.triangles.Add(chunk.vertices.Count + 0);
        chunk.triangles.Add(chunk.vertices.Count + 2);
        chunk.triangles.Add(chunk.vertices.Count + 3);

        chunk.triangles.Add(chunk.vertices.Count + 0);
        chunk.triangles.Add(chunk.vertices.Count + 1);
        chunk.triangles.Add(chunk.vertices.Count + 2);
    }

    private Vector3[] GetDefaultVertices(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax, Vector3 direction)
    {
        Vector3[] side = new Vector3[4];
        if (direction == Vector3.left)
        {

            side[0] = new Vector3(xMin, yMin, zMax);
            side[1] = new Vector3(xMin, yMax, zMax);
            side[2] = new Vector3(xMin, yMax, zMin);
            side[3] = new Vector3(xMin, yMin, zMin);
            return side;
        }
        else if (direction == Vector3.right)
        {

            side[0] = new Vector3(xMax, yMin, zMin);
            side[1] = new Vector3(xMax, yMax, zMin);
            side[2] = new Vector3(xMax, yMax, zMax);
            side[3] = new Vector3(xMax, yMin, zMax);
            return side;
        }
        else if (direction == Vector3.forward)
        {

            side[0] = new Vector3(xMax, yMin, zMax);
            side[1] = new Vector3(xMax, yMax, zMax);
            side[2] = new Vector3(xMin, yMax, zMax);
            side[3] = new Vector3(xMin, yMin, zMax);
            return side;
        }
        else if (direction == Vector3.back)
        {

            side[0] = new Vector3(xMin, yMin, zMin);
            side[1] = new Vector3(xMin, yMax, zMin);
            side[2] = new Vector3(xMax, yMax, zMin);
            side[3] = new Vector3(xMax, yMin, zMin);
            return side;
        }
        else if (direction == Vector3.up)
        {

            side[0] = new Vector3(xMin, yMax, zMin);
            side[1] = new Vector3(xMin, yMax, zMax);
            side[2] = new Vector3(xMax, yMax, zMax);
            side[3] = new Vector3(xMax, yMax, zMin);
            return side;
        }
        else if (direction == Vector3.down)
        {

            side[0] = new Vector3(xMin, yMin, zMax);
            side[1] = new Vector3(xMin, yMin, zMin);
            side[2] = new Vector3(xMax, yMin, zMin);
            side[3] = new Vector3(xMax, yMin, zMax);
            return side;
        }
        return side;
    }

    }
}

