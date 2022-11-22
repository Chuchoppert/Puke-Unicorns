using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerCakes : MonoBehaviour
{
    [Header("Vars for spawn")]
    [SerializeField] GameObject[] CakesToSpawn;
    [SerializeField] Vector2 minMaxRandomX;
    [SerializeField] Vector2 minMaxRandomZ;
    [SerializeField] float HighSpawn;
    [SerializeField] float TimeToSpawnCake;
    public bool SpawnerCanWork = true;

    [Space, Header("Vars for Testing")]
    [SerializeField] KeyCode KeyToSpawn;


    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyToSpawn))
        {
            GameObject RandomCake = CakesToSpawn[Random.Range(0, CakesToSpawn.Length)];
            Vector3 randomPosToSpawn = new Vector3(Random.Range(minMaxRandomX.x, minMaxRandomX.y), HighSpawn, Random.Range(minMaxRandomZ.x, minMaxRandomZ.y));

            Instantiate(RandomCake, randomPosToSpawn, Quaternion.identity);
        }
#endif
    }


    private void Start()
    {
        SpawnCakesInRandomPos();
    }

    void SpawnCakesInRandomPos()
    {
        if (SpawnerCanWork)
        {
            GameObject RandomCake = CakesToSpawn[Random.Range(0, CakesToSpawn.Length)];
            Vector3 randomPosToSpawn = new Vector3(Random.Range(minMaxRandomX.x, minMaxRandomX.y), HighSpawn, Random.Range(minMaxRandomZ.x, minMaxRandomZ.y));

            Instantiate(RandomCake, randomPosToSpawn, Quaternion.identity);

            Invoke("SpawnCakesInRandomPos", TimeToSpawnCake);
        }
    }

    public void activateSpawner()
    {
        SpawnerCanWork = true;
    }

    public void deactivateSpawner()
    {
        SpawnerCanWork = false;
    }
}
