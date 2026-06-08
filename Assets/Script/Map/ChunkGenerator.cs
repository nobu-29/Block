using UnityEngine;
using System.Collections.Generic;

public class ChunkGenerator : MonoBehaviour
{
    public int chunkSize = 16;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5f;
    public int maxHeight = 16;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private int[,,] blocks;

    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        // ★ ブロック配列を初期化
        blocks = new int[chunkSize, maxHeight, chunkSize];

        // ★ 初期地形を blocks に書き込む
        GenerateInitialBlocks();

        GenerateChunk();
    }

    void GenerateChunk()
    {
        mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (blocks[x, y, z] == 0) continue;

                    bool up = y + 1 >= maxHeight || blocks[x, y + 1, z] == 0;
                    bool down = y - 1 < 0 || blocks[x, y - 1, z] == 0;
                    bool north = z + 1 >= chunkSize || blocks[x, y, z + 1] == 0;
                    bool south = z - 1 < 0 || blocks[x, y, z - 1] == 0;
                    bool east = x + 1 >= chunkSize || blocks[x + 1, y, z] == 0;
                    bool west = x - 1 < 0 || blocks[x - 1, y, z] == 0;

                    int blockID = blocks[x, y, z];
                    AddCube(vertices, triangles, new Vector3(x, y, z),blockID,
                        up, down, north, south, east, west);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void GenerateInitialBlocks()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float height = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightMultiplier;
                int h = Mathf.FloorToInt(height);

                for (int y = 0; y <= h; y++)
                {
                    if(y == h)
                        blocks[x, y, z] = 1;
                    else if(y > h -3)
                        blocks[x, y, z] = 2;
                    else
                        blocks[x, y, z] = 3;
                }
            }
        }
    }

    void AddCube(
    List<Vector3> v,
    List<int> t,
    Vector3 pos,
    int blockID,
    bool up, bool down, bool north, bool south, bool east, bool west)
    {
        if (up) AddFace(v, t, pos + new Vector3(0, 1, 0), Vector3.up);
        if (down) AddFace(v, t, pos + new Vector3(0, 0, 0), Vector3.down);
        if (north) AddFace(v, t, pos + new Vector3(0, 0, 1), Vector3.forward);
        if (south) AddFace(v, t, pos + new Vector3(0, 0, 0), Vector3.back);
        if (east) AddFace(v, t, pos + new Vector3(1, 0, 0), Vector3.right);
        if (west) AddFace(v, t, pos + new Vector3(0, 0, 0), Vector3.left);
    }

    void AddFace(List<Vector3> v, List<int> t, Vector3 pos, Vector3 normal)
    {
        int start = v.Count;

        // 面の4頂点
        v.Add(pos + new Vector3(0, 0, 0));
        v.Add(pos + new Vector3(1, 0, 0));
        v.Add(pos + new Vector3(1, 1, 0));
        v.Add(pos + new Vector3(0, 1, 0));

        // 三角形
        t.Add(start + 0);
        t.Add(start + 2);
        t.Add(start + 1);

        t.Add(start + 0);
        t.Add(start + 3);
        t.Add(start + 2);
    }

/*    bool HasBlock(int x, int y, int z, int[,] heightMap)
    {
        if (x < 0 || x >= chunkSize || z < 0 || z >= chunkSize) return false;
        return y <= heightMap[x, z];

    }*/

    public void BreakBlock(Vector3Int pos)
    {
        if (!InRange(pos)) return;

        blocks[pos.x, pos.y, pos.z] = 0;
        GenerateChunk();
    }

    public void PlaceBlock(Vector3Int pos, int blockID)
    {
        if (!InRange(pos)) return;

        blocks[pos.x, pos.y, pos.z] = blockID;
        GenerateChunk();
    }

    bool InRange(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < chunkSize &&
               pos.z >= 0 && pos.z < chunkSize &&
               pos.y >= 0 && pos.y < maxHeight;
    }
}
