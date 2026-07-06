using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Dictionary<Vector3Int, int> modifiedBlocks = new Dictionary<Vector3Int, int>();
}

public class BlockWorld : MonoBehaviour
{
    public GameObject chunkPrefab;

    public Transform _player;
    public int chunkSize = 16;
    public int viewDistance = 2;

    [Header("チャンク数")]
    public int worldSize = 4;   //チャンク数
    public int maxheight = 16;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5f;
    public Dictionary<Vector2Int, ChunkMeshWorld> chunks = new Dictionary<Vector2Int, ChunkMeshWorld>();

    private Vector2Int currentPlayerChunk;
    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    private void Start()
    {
        UpdateChunks();
        //GenerateWorld();
    }

    void Update()
    {
        Vector2Int newChunk = GetPlayerChunk();

        if (newChunk != currentPlayerChunk)
        {
            currentPlayerChunk = newChunk;
            UpdateChunks();
        }
    }


    void UpdateChunks()
    {
        Vector2Int playerChunk = GetPlayerChunk();

        // 必要なチャンク一覧
        HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                if ((x * x) + (z * z) >( viewDistance * viewDistance)) continue;

                Vector2Int chunkPos = new Vector2Int(playerChunk.x + x, playerChunk.y + z);
                neededChunks.Add(chunkPos);

                if (!chunks.ContainsKey(chunkPos))
                {
                    GenerateChunk(chunkPos);
                }
            }
        }

        // 不要チャンク削除
        List<Vector2Int> toRemove = new List<Vector2Int>();


        foreach(var chunk in chunks)
{
            if (!neededChunks.Contains(chunk.Key))
            {
                var chunkMesh = chunk.Value.GetComponent<ChunkMeshWorld>();

                chunkMesh.ClearChunk();

                chunk.Value.gameObject.SetActive(false);

                chunkPool.Enqueue(chunk.Value.gameObject);

                toRemove.Add(chunk.Key);
            }
        }

        foreach (var pos in toRemove)
        {
            chunks.Remove(pos);
        }

    }

    Vector2Int GetPlayerChunk()
    {
        int x = Mathf.FloorToInt(_player.position.x / chunkSize);
        int z = Mathf.FloorToInt(_player.position.z / chunkSize);

        return new Vector2Int(x, z);
    }

    void GenerateChunk(Vector2Int chunkPos)
    {

        if (chunks.ContainsKey(chunkPos))
        {
            chunks[chunkPos].gameObject.SetActive(true);
            return;
        }

        GameObject chunkObj;

        if (chunkPool.Count > 0)
        {
            chunkObj = chunkPool.Dequeue();

            chunkObj.SetActive(true);
        }
        else
        {
            chunkObj = Instantiate(chunkPrefab);
        }

        chunkObj.name = $"Chunk_{chunkPos.x}_{chunkPos.y}";

        var chunkMesh = chunkObj.GetComponent<ChunkMeshWorld>();

        chunkMesh.blocks = new int[chunkMesh._chunkSize,chunkMesh.height,chunkMesh._chunkSize];


        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int worldX = x + chunkPos.x * chunkSize;
                int worldZ = z + chunkPos.y * chunkSize;

                float height = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale) * heightMultiplier;
                int h = Mathf.FloorToInt(height);

                for (int y = 0; y <= h; y++)
                {

                    if (y == h)
                        chunkMesh.blocks[x, y, z] = 1; // 草
                    else if (y > h - 3)
                        chunkMesh.blocks[x, y, z] = 2; // 土
                    else
                        chunkMesh.blocks[x, y, z] = 3; // 石
                }
            }
        }

        chunkMesh.SetDirty();
        //chunkMesh.BuildMesh();

        chunks.Add(chunkPos, chunkMesh);


        chunkObj.transform.position = new Vector3(
            chunkPos.x * chunkSize,
            0,
            chunkPos.y * chunkSize
        );

    }

    public void ModifyBlock(Vector3Int worldPos,int blockID)
    {

        Vector2Int chunkPos = new Vector2Int(
                Mathf.FloorToInt((float)worldPos.x / chunkSize),
                Mathf.FloorToInt((float)worldPos.z / chunkSize)
        );

        if(!chunks.ContainsKey(chunkPos)) return;

        var chunkMesh = chunks[chunkPos];

        int x = ((worldPos.x % chunkSize) + chunkSize) % chunkSize;
        int y = worldPos.y;
        int z = ((worldPos.z % chunkSize) + chunkSize) % chunkSize;

        chunkMesh.blocks[x, y, z] = blockID;

        chunkMesh.SetDirty();


    }

    public int GetBlock(Vector3Int worldPos)
    {
        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt((float)worldPos.x / chunkSize),Mathf.FloorToInt((float)worldPos.z / chunkSize));

        if (!chunks.ContainsKey(chunkPos)) return 0;

        var chunkMesh = chunks[chunkPos];

        int x = ((worldPos.x % chunkSize) + chunkSize) % chunkSize;
        int y = worldPos.y;
        int z = ((worldPos.z % chunkSize) + chunkSize) % chunkSize;

        if (y < 0 || y >= maxheight) return 0;

        return chunkMesh.blocks[x, y, z];

    }

    /*    public void ReturnBlock(GameObject block)
        {
            block.SetActive(false);
            blockPool.Enqueue(block);
        }*/
}
