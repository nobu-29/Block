using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Dictionary<Vector3Int, int> modifiedBlocks = new Dictionary<Vector3Int, int>();

    public HashSet<Vector3Int> waterSources = new HashSet<Vector3Int>();
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

    [Header("木の生成率")]
    public float treeRate = 0.5f;

    [Header("地形関係のもの")]
    public float noiseScale = 0.1f;
    public float noiseScale_mini = 0.15f;
    public float heightMultiplier = 5f;
    public float heightMultiplier_mini = 4f;
    public Dictionary<Vector2Int, ChunkMeshWorld> chunks = new Dictionary<Vector2Int, ChunkMeshWorld>();

    private Dictionary<Vector2Int, ChunkData> savedChunks = new Dictionary<Vector2Int, ChunkData>();
    private HashSet<Vector2Int> generatedChunks = new HashSet<Vector2Int>();

    private HashSet<Vector3Int> activeWater = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> waterSources = new HashSet<Vector3Int>();
    private HashSet<ChunkMeshWorld> dirtyChunks = new HashSet<ChunkMeshWorld>();

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

        foreach (var chunk in dirtyChunks)
        {
            chunk.SetDirty();
        }
        dirtyChunks.Clear();
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
                Vector2Int pos = chunk.Key;

                List<Vector3Int> removeWater = new List<Vector3Int>();

                foreach(var w in activeWater)
                {
                    int cx = Mathf.FloorToInt((float)w.x / chunkSize);
                    int cz = Mathf.FloorToInt((float)w.z / chunkSize); 
                    if (cx == pos.x && cz == pos.y) { removeWater.Add(w);}
                }

                foreach (var w in removeWater) 
                {
                    activeWater.Remove(w);
                }
/*
                removeWater.Clear();
                foreach (var w in waterSources) { int cx = Mathf.FloorToInt((float)w.x / chunkSize);
                    int cz = Mathf.FloorToInt((float)w.z / chunkSize);
                    if (cx == pos.x && cz == pos.y) {removeWater.Add(w);}
                }
                foreach (var w in removeWater)
                {
                    waterSources.Remove(w); 
                }*/

                var chunkMesh = chunk.Value.GetComponent<ChunkMeshWorld>();

                //chunkMesh.ClearChunk();

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
        HashSet<Vector3Int> newWater = new HashSet<Vector3Int>();

        foreach (var waterchunk in activeWater)
        {
            Vector3 playerPos = _player.position;
            if((waterchunk - Vector3Int.FloorToInt(playerPos)).sqrMagnitude > 625)
            {
                continue;
            }

            Vector3Int below = waterchunk + Vector3Int.down;

            if(GetBlock(below) == 0)
            {
                ModifyBlock(below, 800);

                newWater.Add(below);
            }
            else
            {
                int level = GetWaterLevel(GetBlock(waterchunk));

                SpreadWater(waterchunk + Vector3Int.right, level - 1, newWater);
                SpreadWater(waterchunk + Vector3Int.left, level - 1, newWater);
                SpreadWater(waterchunk + Vector3Int.forward, level - 1, newWater);
                SpreadWater(waterchunk + Vector3Int.back, level - 1, newWater);
            }
        }

        foreach (var source in waterSources)
        {
            newWater.Add(source);
        }

        activeWater = newWater;
    }

    Vector2Int GetPlayerChunk()
    {
        int x = Mathf.FloorToInt(_player.position.x / chunkSize);
        int z = Mathf.FloorToInt(_player.position.z / chunkSize);

        return new Vector2Int(x, z);
    }

    void GenerateChunk(Vector2Int chunkPos)
    {
        /*bool firstGenerate = !generatedChunks.Contains(chunkPos);

        if (firstGenerate)
        {
            generatedChunks.Add(chunkPos);
        }

        if (chunks.ContainsKey(chunkPos))
        {
            chunks[chunkPos].gameObject.SetActive(true);
            return;
        }
        else
        {
            
        }
*/

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

        //chunkMesh.blocks = new int[chunkMesh._chunkSize, chunkMesh.height, chunkMesh._chunkSize];
        if(chunkMesh.blocks == null)
        {
            chunkMesh.blocks = new int[chunkMesh._chunkSize, chunkMesh.height, chunkMesh._chunkSize];
        }
        else
        {
            System.Array.Clear(chunkMesh.blocks, 0, chunkMesh.blocks.Length);
        }

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int worldX = x + chunkPos.x * chunkSize;
                int worldZ = z + chunkPos.y * chunkSize;

                float height = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale) * heightMultiplier + Mathf.PerlinNoise(worldX * noiseScale_mini, worldZ * noiseScale_mini) * heightMultiplier_mini;

                //バイオーム生成用コード
                float biomeNoise = Mathf.PerlinNoise(worldX * 0.01f, worldZ * 0.01f);

                float forestNoise = Mathf.PerlinNoise(worldX * 0.04f, worldZ * 0.04f);

                //砂漠にするかどうかの判定
                bool isDesert = biomeNoise > 0.7f;

                bool isPlains = biomeNoise >= 0.4f && biomeNoise < 0.7f;

                bool isForest = biomeNoise >= 0.2f && biomeNoise < 0.4f;

                bool isSnow = biomeNoise < 0.2f;

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

                        if (isForest && forestNoise > 0.4f) 
                            TrySpawnTree(chunkMesh, x, y, z, 15f);

                        else if (isPlains && forestNoise > 0.8f) 
                            TrySpawnTree(chunkMesh, x, y, z, treeRate);
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

                    Vector3Int waterPos = new Vector3Int(worldX, waterY, worldZ);

                    GetChunkData(chunkPos).waterSources.Add(waterPos);

                    waterSources.Add(waterPos);
                    activeWater.Add(waterPos);
                }
            }
        }

        if(savedChunks.TryGetValue(chunkPos, out ChunkData data))
        {
            foreach (var block in data.modifiedBlocks)
            {
                Vector3Int worldPos = block.Key;

                int blockID = block.Value;
                int bx = ((worldPos.x % chunkSize) + chunkSize) % chunkSize;
                int bz = ((worldPos.z % chunkSize) + chunkSize) % chunkSize;
                int localY = chunkMesh.WorldYToLocalY( worldPos.y);
                if (localY >= 0 && localY < chunkMesh.height)
                {
                    chunkMesh.blocks[ bx, localY, bz] = blockID;         
                }
            }

            foreach(var waterPos in data.waterSources)
            {
                waterSources.Add(waterPos);
                activeWater.Add(waterPos);
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

    void TrySpawnTree(ChunkMeshWorld chunk, int x, int groundY, int z, float chance)
    {
        if (groundY < 2) return;

        if (Random.Range(0f, 100f) > chance)
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

/*    void TrySpread(ChunkMeshWorld chunk,int x,int y,int z)
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

    */

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
        else if (localY < 0 || localY >= chunkMesh.height) return;

        int z = ((worldPos.z % chunkSize) + chunkSize) % chunkSize;

        //下の1行は不要なメッシュ再生成を減らすため
        if (chunkMesh.blocks[x, localY, z] == blockID) return;

        chunkMesh.blocks[x, localY, z] = blockID;

        if (!savedChunks.ContainsKey(chunkPos))
        {
            savedChunks[chunkPos] = new ChunkData();
        }

        savedChunks[chunkPos].modifiedBlocks[worldPos] = blockID;

        dirtyChunks.Add(chunkMesh);
    }

    public int GetBlock(Vector3Int worldPos)
    {
        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt((float)worldPos.x / chunkSize),Mathf.FloorToInt((float)worldPos.z / chunkSize));

        if (!chunks.ContainsKey(chunkPos)) return -1;

        var chunkMesh = chunks[chunkPos];

        int x = ((worldPos.x % chunkSize) + chunkSize) % chunkSize;

        int y = worldPos.y;
        int localY = chunkMesh.WorldYToLocalY(y);

        if (localY < 0 || localY >= chunkMesh.height) return -1;

        int z = ((worldPos.z % chunkSize) + chunkSize) % chunkSize;

        if (y < worldMinY || y >= worldMaxY) return 0;

        return chunkMesh.blocks[x, localY, z];

    }

    int GetWaterLevel(int id)
    {
        if (id == 8) return 8;

        if(id >= 800 && id <= 806)
            return 807 - id;

        return 0;
    }

    public bool IsSolid(int blockID)
    {
        switch (blockID)
        {
            //空気
            case 0:
            // 水
            case 8:
            // 水流
            case 800:
            case 801:
            case 802:
            case 803:
            case 804:
            case 805:
            case 806:
                return false;
        }

        return true;
    }

    public void ActivateWater(Vector3Int waterPos)
    {
        activeWater.Add(waterPos);

        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt((float)waterPos.x / chunkSize), Mathf.FloorToInt((float)waterPos.z / chunkSize));
        GetChunkData(chunkPos).waterSources.Add(waterPos);
    }

    void SpreadWater(Vector3Int pos, int level, HashSet<Vector3Int> newWater)
    {
        if (level <= 0) 
            return;

        if (GetBlock(pos) != 0)
            return;

        ModifyBlock(pos, 807 - level);
        newWater.Add(pos);
    }

    ChunkData GetChunkData (Vector2Int chunkPos)
    {
        if (!savedChunks.ContainsKey(chunkPos))
        {
            savedChunks[chunkPos] = new ChunkData();
        }

        return savedChunks[chunkPos];
    }
}
