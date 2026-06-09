using UnityEngine;

public class BlockWorld : MonoBehaviour
{
    public GameObject DirtPF;
    public GameObject GrassPF;
    public GameObject StonePF;

    public int chunkSize = 16;
    [Header("チャンク数")]public int worldSize = 4;   //チャンク数
    public int maxheight = 16;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5f;

    private void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {

        for (int cx = 0; cx < worldSize; cx++)
        {
            for (int cz = 0; cz < worldSize; cz++)
            {
                GenerateChunk(cx, cz);
            }
        }

    }

    void GenerateChunk(int chunkX,int chunkZ)
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
    }
}
