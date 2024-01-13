using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningProjectile : MonoBehaviour
{
    [SerializeField] protected float spawnInterval;
    [SerializeField] protected List<Projectile> spawnPrefabs;
    [SerializeField] protected List<Vector3> spawnOffsets;
    [SerializeField] protected List<Vector3> spawnRotations;
    protected float spawnTimer = 0;
    protected Projectile projectile;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnPrefabs.Count != spawnOffsets.Count)
            Debug.LogError("Both lists must be parallel!");

        projectile = GetComponent<Projectile>();
        if (projectile == null)
            Debug.Log("Missing Projectile!");

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnInterval > 0)
        {
            if (spawnTimer > spawnInterval)
            {
                Spawn();
                spawnTimer -= spawnInterval;
            }
            spawnTimer += Time.deltaTime;
        }
    }

    public void Spawn()
    {
        for (int i = 0; i < spawnPrefabs.Count; i++)
        {
            Projectile spawn = Instantiate(spawnPrefabs[i], transform.position, transform.rotation);

            spawn.transform.position += transform.right * spawnOffsets[i].x;
            spawn.transform.position += transform.up * spawnOffsets[i].y;
            spawn.transform.position += transform.forward * spawnOffsets[i].z;

            spawn.transform.Rotate(spawnRotations[i]);

            spawn.SetSource(projectile.GetSource());
        }
    }

    public void OnDestroy()
    {
        if (gameObject.scene.isLoaded) Spawn();
    }
}
