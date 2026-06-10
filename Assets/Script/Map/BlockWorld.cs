using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Dictionary<Vector3Int, int> modifiedBlocks = new Dictionary<Vector3Int, int>();
}

public class BlockWorld : MonoBehaviour
{
    public GameObject DirtPF;
    public GameObject GrassPF;
    public GameObject StonePF;

    public Transform _player;
    public int chunkSize = 16;
    public int viewDistance = 2;
    [Header("チャンク数")]public int worldSize = 4;   //チャンク数
    public int maxheight = 16;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5f;

    private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, ChunkData> _chunkDatas = new Dictionary<Vector2Int, ChunkData>();
    private Vector2Int currentPlayerChunk;

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

        foreach (var chunk in chunks)
        {
            if (!neededChunks.Contains(chunk.Key))
            {
                Destroy(chunk.Value);
                toRemove.Add(chunk.Key);
            }
        }

        foreach (var pos in toRemove)
        {
            chunks.Remove(pos);
        }
    }

/*
    void GenerateWorld()
    {

        for (int cx = 0; cx < worldSize; cx++)
        {
            for (int cz = 0; cz < worldSize; cz++)
            {
                GenerateChunk(cx, cz);
            }
        }

    }*/


    Vector2Int GetPlayerChunk()
    {
        int x = Mathf.FloorToInt(_player.position.x / chunkSize);
        int z = Mathf.FloorToInt(_player.position.z / chunkSize);

        return new Vector2Int(x, z);
    }


   /* void GenerateChunk(int chunkX,int chunkZ)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int worldX = x + chunkX * chunkSize;
                int worldZ = z + chunkZ * chunkSize;

                float Yheight = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale) * heightMultiplier;

                int height = Mathf.FloorToInt(Yheight);

                for (int y = 0; y <= height; y++)
                {
                    GameObject prefab;
                    if (y == height)
                        prefab = GrassPF;
                    else if (y > height - 3)
                        prefab = DirtPF;
                    else
                        prefab = StonePF;

                    Instantiate(prefab, new Vector3(worldX, y, worldZ), Quaternion.identity);
                }

            }
        }
    }*/

    void GenerateChunk(Vector2Int chunkPos)
    {
        GameObject chunkObj = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");

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
                    Vector3Int blockpos = new Vector3Int(worldX, y, worldZ);

                    int ID;

                    if (_chunkDatas.ContainsKey(chunkPos) && 
                        _chunkDatas[chunkPos].modifiedBlocks.ContainsKey(blockpos))
                    {
                        ID = _chunkDatas[chunkPos].modifiedBlocks[blockpos];
                    }
                    else
                    {
                        if (y == h)
                            ID = 1;
                        else if (y > h - 3)
                            ID = 2;
                        else
                            ID = 3;

                    }

                    //壊されている
                    if (ID == 0) continue;

                    GameObject prefab = GetPrefabID(ID);
                    GameObject block = Instantiate(prefab, blockpos, Quaternion.identity);
                    block.transform.parent = chunkObj.transform;
                    block.isStatic = true;
                }
            }
        }

        chunks.Add(chunkPos, chunkObj);
    }

    public void ModifyBlock(Vector3Int worldPos,int blockID)
    {

        Vector2Int chunkPos = new Vector2Int(
                Mathf.FloorToInt((float)worldPos.x / chunkSize),
                Mathf.FloorToInt((float)worldPos.z / chunkSize)
            );

        if (!_chunkDatas.ContainsKey(chunkPos))
        {
            _chunkDatas[chunkPos] = new ChunkData();
        }

        _chunkDatas[chunkPos].modifiedBlocks[worldPos] = blockID;

        if (chunks.ContainsKey(chunkPos))
        {
            Destroy(chunks[chunkPos]);
            chunks.Remove(chunkPos);
            GenerateChunk(chunkPos);
        }
    }

    GameObject GetPrefabID(int ID)
    {
        switch (ID)
        {
            case 1: return GrassPF;
            case 2: return DirtPF;
            case 3: return StonePF;
            default:
                Debug.LogError("Unknown Block ID:" + ID);
                return null;

        }
    }
}
