using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject rockPrefab;
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

            GameObject tree = Instantiate(treePrefab, transform);
            tree.transform.position = new Vector3(x, y - yFactor, z);
            tree.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            tree.transform.localScale = Vector3.one * Random.Range(2.8f, 3.2f);
        }

        for (int i = 0; i < rockCnt; i++)
        {
            float randX = Random.value;
            float randZ = Random.value;
            float x = randX * terrain.terrainData.size.x;
            float z = randZ * terrain.terrainData.size.z;
            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            GameObject rock = Instantiate(rockPrefab, transform);
            rock.transform.position = new Vector3(x, y - yFactor, z);
            rock.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            rock.transform.localScale = Vector3.one * Random.Range(2.8f, 3.2f);
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
