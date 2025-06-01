using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] GameObject tree;
    [SerializeField] GameObject rock;
    [SerializeField] int treeCnt;
    [SerializeField] int rockCnt;

    [SerializeField] float yFactor;

    public void SpawnResources()
    {
        for(int i = 0; i < treeCnt; i++)
        {
            float randX = Random.value;
            float randZ = Random.value;
            float x = randX * terrain.terrainData.size.x;
            float z = randZ * terrain.terrainData.size.z;
            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            Instantiate(tree, new Vector3(x, y - yFactor, z), Quaternion.identity, transform);
        }

        for (int i = 0; i < rockCnt; i++)
        {
            float randX = Random.value;
            float randZ = Random.value;
            float x = randX * terrain.terrainData.size.x;
            float z = randZ * terrain.terrainData.size.z;
            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            Instantiate(rock, new Vector3(x, y - yFactor, z), Quaternion.identity, transform);
        }
    }

    public void RemoveResources()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }
    }
}
