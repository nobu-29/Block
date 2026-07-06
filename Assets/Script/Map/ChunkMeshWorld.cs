using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider))]
public class ChunkMeshWorld : MonoBehaviour
{
    public int _chunkSize = 16;
    public int height = 96;
    public int worldMinY = -32;

    public int[,,] blocks;

    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    List<Vector2> uv = new List<Vector2>();
    public Material _material;

    private bool isDirty;

    private MeshCollider _meshCollider;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private readonly Vector3[] quad = new Vector3[4];

    private void Awake()
    {
        blocks = new int[_chunkSize, height, _chunkSize];
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if(_material == null)
        {
            Debug.LogError("Materialが設定されていないよ！!");
            return;
        }

        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();

        _meshFilter.mesh = mesh;
        _meshRenderer.material = _material;
    }

    private void Update()
    {
        if (!isDirty) return;

        BuildMesh();
        isDirty = false;
    }

    public int WorldYToLocalY (int worldY)
    {
        return worldY - worldMinY;
    }

    public void SetDirty()
    {
        isDirty = true;
    }

    public void ClearChunk()
    {

        System.Array.Clear(blocks, 0, blocks.Length);

        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        mesh.Clear();

        _meshCollider.sharedMesh = null;

    }

    public void BuildMesh()
    {
        vertices.Clear();
        uv.Clear();
        triangles.Clear();

        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < _chunkSize; z++)
                {
                    if (blocks[x, y, z] == 0) continue;

                    CheckFace(x, y, z, Vector3.forward);
                    CheckFace(x, y, z, Vector3.back);
                    CheckFace(x, y, z, Vector3.left);
                    CheckFace(x, y, z, Vector3.right);
                    CheckFace(x, y, z, Vector3.up);
                    CheckFace(x, y, z, Vector3.down);
                }
            }
        }


        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        _meshCollider.sharedMesh = null;
        _meshCollider.sharedMesh = mesh;
    }


    void CheckFace(int x, int y, int z, Vector3 dir)
    {
        int nx = x + (int)dir.x;
        int ny = y + (int)dir.y;
        int nz = z + (int)dir.z;

        if (nx >= 0 && nx < _chunkSize &&
            ny >= 0 && ny < height &&
            nz >= 0 && nz < _chunkSize)
        {
            if (blocks[nx, ny, nz] != 0) return;
        }

        AddFace(new Vector3(x, y + worldMinY, z), dir, blocks[x, y, z]);
    }

    void AddFace(Vector3 pos, Vector3 dir,int blockID)
    {
        int v = vertices.Count;

        if (dir == Vector3.forward)
        {
            quad[0] = pos + new Vector3(0, 0, 1);
            quad[1] = pos + new Vector3(1, 0, 1);
            quad[2] = pos + new Vector3(1, 1, 1);
            quad[3] = pos + new Vector3(0, 1, 1);
        }
        else if (dir == Vector3.back)
        {
            quad[0] = pos + new Vector3(1, 0, 0);
            quad[1] = pos + new Vector3(0, 0, 0);
            quad[2] = pos + new Vector3(0, 1, 0);
            quad[3] = pos + new Vector3(1, 1, 0);
        }
        else if (dir == Vector3.left)
        {
            quad[0] = pos + new Vector3(0, 0, 0);
            quad[1] = pos + new Vector3(0, 0, 1);
            quad[2] = pos + new Vector3(0, 1, 1);
            quad[3] = pos + new Vector3(0, 1, 0);
        }
        else if (dir == Vector3.right)
        {
            quad[0] = pos + new Vector3(1, 0, 1);
            quad[1] = pos + new Vector3(1, 0, 0);
            quad[2] = pos + new Vector3(1, 1, 0);
            quad[3] = pos + new Vector3(1, 1, 1);
        }
        else if (dir == Vector3.up)
        {
            quad[0] = pos + new Vector3(0, 1, 1);
            quad[1] = pos + new Vector3(1, 1, 1);
            quad[2] = pos + new Vector3(1, 1, 0);
            quad[3] = pos + new Vector3(0, 1, 0);
        }
        else if (dir == Vector3.down)
        {
            quad[0] = pos + new Vector3(0, 0, 0);
            quad[1] = pos + new Vector3(1, 0, 0);
            quad[2] = pos + new Vector3(1, 0, 1);
            quad[3] = pos + new Vector3(0, 0, 1);
        }

        vertices.AddRange(quad);

        triangles.Add(v + 0);
        triangles.Add(v + 1);
        triangles.Add(v + 2);

        triangles.Add(v + 0);
        triangles.Add(v + 2);
        triangles.Add(v + 3);

        Vector2 uvOffset = GetUVByFace(blockID,dir);
        float size = 0.25f; // 4x4テクスチャ想定

        uv.Add(uvOffset + new Vector2(0, 0));
        uv.Add(uvOffset + new Vector2(size, 0));
        uv.Add(uvOffset + new Vector2(size, size));
        uv.Add(uvOffset + new Vector2(0, size));
    }

    Vector2 GetUV(int x, int y)
    {
        float size = 0.25f;

        return new Vector2(x * size, (3 - y) * size);
    }

    //↓ブロックに使うメッシュの設定箇所
    Vector2 GetUVByFace(int id, Vector3 dir)
    {
        //float size = 0.25f;

        if (id == 1)
        {

            if (dir == Vector3.up)      // 上
                return GetUV(0, 0);     // 草上
            else if (dir == Vector3.down) // 下
                return GetUV(2, 0);     // 土
            else                        // 横
                return GetUV(1, 2);     // 草側面
        }

        // 土
        else if (id == 2)
            return GetUV(2, 0);

        // 石
        else if (id == 3)
            return GetUV(1, 0);

        // 幹
        else if (id == 4)
            return GetUV(2, 2);

        // 葉
        else if (id == 5)
            return GetUV(3, 2);

        //砂（ケイ素の元素番号14）
        else if (id ==14)
            return GetUV(3, 0);

        // 鉄（元素番号26）
        else if (id == 26)
            return GetUV(3, 1);

        // 金（元素番号79）
        else if (id == 79)
            return GetUV(0, 1);

        // 岩盤
        else if (id == 99)
            return GetUV(0, 2);


        return GetUV(0, 0);

    }
}
