using UnityEngine;

public class BlockWorld : MonoBehaviour
{
    public GameObject BlockPF;

    public int width = 20;
    public int height = 16;
    public int depth = 20;

    private void Start()
    {
        for(int x = 0; x < width; x++)
        {
            for (int z = 0;z < depth; z++)
            {
                int YHeight = Mathf.FloorToInt(
                    Mathf.PerlinNoise(x * 0.1f,z * 0.1f) * height
                    );

                for (int y = 0; y <= YHeight; y++)
                {
                    Instantiate(
                        BlockPF, new Vector3(x, y, z),
                        Quaternion.identity
                        );
                }

            }
        }
    }
}
