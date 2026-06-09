using UnityEngine;

public class BlockWorld : MonoBehaviour
{
    public GameObject DirtPF;
    public GameObject GrassPF;
    public GameObject StonePF;

    public int chunkSize = 16;
    public int maxheight = 16;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 5f;

    private void Start()
    {
        GenerateWorld();

        
    }

    void GenerateWorld()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float Yheight = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightMultiplier;

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

                    Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
                }

            }
        }
    }
}
