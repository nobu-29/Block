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
    [Header("ワールドの高さ設定")]
    public int worldMinY = -32;
    public int worldMaxY = 64;
    public int seaLevel = 10;

    public float noiseScale = 0.1f;
    public float noiseScale_mini = 0.15f;
    public float heightMultiplier = 5f;
    public float heightMultiplier_mini = 4f;
    public Dictionary<Vector2Int, ChunkMeshWorld> chunks = new Dictionary<Vector2Int, ChunkMeshWorld>();

    private Vector2Int currentPlayerChunk;
    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    private float waterTimer;

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

        waterTimer += Time.deltaTime;
        if(waterTimer > 0.25f)
        {
            waterTimer = 0;
            UpdateWater();
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

    void UpdateWater()
    {
        foreach (var chunk in chunks.Values)
        {
            bool changed = false;
            for (int y = 1; y < chunk.height; y++)
            {
                 for (int x = 0; x < chunk._chunkSize; x++)
                    {
                    for (int z = 0; z < chunk._chunkSize; z++)
                    {
                        if (chunk.blocks[x, y, z] != 8)
                            continue;

                        if (chunk.blocks[x, y - 1, z] == 0)
                        {
                            chunk.blocks[x, y - 1, z] = 8;

                            changed = true;
                        }
                        else
                        {

                            TrySpread(chunk, x + 1, y, z);
                            TrySpread(chunk, x - 1, y, z);

                            TrySpread(chunk, x, y, z + 1);
                            TrySpread(chunk, x, y, z - 1);

                            changed = true;

                        }
                    }
                }
            }

            if (changed) chunk.SetDirty();
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

                float height = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale) * heightMultiplier + Mathf.PerlinNoise(worldX * noiseScale_mini, worldZ * noiseScale_mini) * heightMultiplier_mini;

                //バイオーム生成用コード
                float biomeNoise = Mathf.PerlinNoise(worldX * 0.01f, worldZ * 0.01f);

                //砂漠にするかどうかの判定
                bool isDesert = biomeNoise > 0.65f;

                bool isSnow = biomeNoise < 0.25f;

                int h = Mathf.FloorToInt(height);

                if (!isDesert && h < seaLevel) h = seaLevel;

                for (int y = worldMinY; y <= h; y++)
                {
                    float caveNoise = Mathf.PerlinNoise((worldX + 1000) * 0.08f, (worldZ + y) * 0.08f);

                    int localY = chunkMesh.WorldYToLocalY(y);

                    if (y == worldMinY)
                        chunkMesh.blocks[x, localY, z] = 99; // 岩盤
                    else if(y == h)
                    {
                        if (isDesert)
                            chunkMesh.blocks[x, localY, z] = 14; // 砂
                        else if (isSnow)
                            chunkMesh.blocks[x, localY, z] = 9; //雪
                        else
                            chunkMesh.blocks[x, localY, z] = 1; // 草

                        if(!isDesert) TrySpawnTree(chunkMesh, x, y, z);
                    }
                    else if (y > h - 3)
                    {

                        if(isDesert)
                            chunkMesh.blocks[x, localY, z] = 14; // 砂
                        else if (isSnow)
                            chunkMesh.blocks[x, localY, z] = 9; //雪
                        else
                            chunkMesh.blocks[x, localY, z] = 2; // 土
                    }
                    else if(y < h - 5)
                    {

                        float oreNoise = Mathf.PerlinNoise(worldX * 0.2f, (worldZ + y) * 0.2f);

                        if (oreNoise > 0.9f && y < -10)
                            chunkMesh.blocks[x, localY, z] = 79; //金
                        else if (oreNoise > 0.75f)
                            chunkMesh.blocks[x, localY, z] = 26; // 鉄
                        else
                            chunkMesh.blocks[x, localY, z] = 3; // 石
                    }
                    else
                    {
                        if (caveNoise > 0.62f && y < h - 3) continue;

                            chunkMesh.blocks[x, localY, z] = 3; // 石
                    }
                }

                for (int waterY = h + 1; waterY <= seaLevel; waterY++)
                {
                    int localWaterY = chunkMesh.WorldYToLocalY(waterY);

                    chunkMesh.blocks[x, localWaterY, z] = 8;
                }
            }
        }

        chunkMesh.SetDirty();
        chunkMesh.BuildMesh();

        chunks.Add(chunkPos, chunkMesh);


        chunkObj.transform.position = new Vector3(
            chunkPos.x * chunkSize,
            0,
            chunkPos.y * chunkSize
        );

    }

    void TrySpawnTree(ChunkMeshWorld chunk, int x, int groundY, int z)
    {
        if (groundY < 2) return;

        if (Random.Range(0f, 100f) > 2f)
            return;

        int trunkHeight = Random.Range(4, 7);

        for (int i = 1; i <= trunkHeight; i++)
        {
            int ly = chunk.WorldYToLocalY(groundY + i);

            if (ly >= chunk.height) return;

            chunk.blocks[x, ly, z] = 4;
        }

        int leafCenter =
            chunk.WorldYToLocalY(
                groundY + trunkHeight
            );

        for (int lx = -2; lx <= 2; lx++)
        {
            for (int lz = -2; lz <= 2; lz++)
            {
                for (int ly = -2; ly <= 1; ly++)
                {
                    int nx = x + lx;
                    int ny = leafCenter + ly;
                    int nz = z + lz;

                    if (nx < 0 || nx >= chunkSize) continue;
                    if (nz < 0 || nz >= chunkSize) continue;
                    if (ny < 0 || ny >= chunk.height) continue;

                    float distance = 
                        lx * lx + 
                        ly * ly + 
                        lz * lz;

                    if (distance > 6) continue;

                    if (chunk.blocks[nx, ny, nz] == 0)
                        chunk.blocks[nx, ny, nz] = 5;
                }
            }
        }

    }

    void TrySpread(ChunkMeshWorld chunk,int x,int y,int z)
    {
        if (x < 0 || x >= chunk._chunkSize)
            return;

        if (z < 0 || z >= chunk._chunkSize)
            return;

        if (y < 0 || y >= chunk.height)
            return;

        if (chunk.blocks[x, y, z] != 0)
            return;

        chunk.blocks[x, y, z] = 8;
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
        int localY = chunkMesh.WorldYToLocalY(y);

        if (y < worldMinY || y >= worldMaxY) return;

        int z = ((worldPos.z % chunkSize) + chunkSize) % chunkSize;

        chunkMesh.blocks[x, localY, z] = blockID;

        chunkMesh.SetDirty();


    }

    public int GetBlock(Vector3Int worldPos)
    {
        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt((float)worldPos.x / chunkSize),Mathf.FloorToInt((float)worldPos.z / chunkSize));

        if (!chunks.ContainsKey(chunkPos)) return 0;

        var chunkMesh = chunks[chunkPos];

        int x = ((worldPos.x % chunkSize) + chunkSize) % chunkSize;

        int y = worldPos.y;
        int localY = chunkMesh.WorldYToLocalY(y);


        int z = ((worldPos.z % chunkSize) + chunkSize) % chunkSize;

        if (y < worldMinY || y >= worldMaxY) return 0;

        return chunkMesh.blocks[x, localY, z];

    }

    /*    public void ReturnBlock(GameObject block)
        {
            block.SetActive(false);
            blockPool.Enqueue(block);
        }*/
}
