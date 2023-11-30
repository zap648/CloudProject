using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RainManager : MonoBehaviour
{
    public float widthX = 100.0f;
    public float heightY = 25.0f;
    public float widthZ = 100.0f;
    public float speed = 10.0f;

    [SerializeField] BallManager ballManager;
    [SerializeField] List<GameObject> drops;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rain();
        Depopulate();
    }

    void Rain()
    {
        // Spawn a "drop of rain" at a random spot within the borders of this box
        float spawnX = Random.Range(-widthX / 2, widthX / 2);
        float spawnY = Random.Range(-heightY / 2, heightY / 2);
        float spawnZ = Random.Range(-widthZ / 2, widthZ / 2);
        Vector3 spawn = new Vector3(spawnX, spawnY, spawnZ);

        GameObject drop = ballManager.SpawnBall(spawn);
        if (!drops.Contains(drop))
            drops.Add(drop);
    }

    void Depopulate()
    {
        foreach(GameObject drop in drops)
        {
            if (drop.transform.position.y < -100.0f)
                ballManager.PoolBall(drop);
        }
    }
}
