using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("Balls")]
    public GameObject ball;
    public List<GameObject> balls;

    private void Start()
    {
        SpawnBalls(1);
    }

    private void Update()
    {

    }

    void SpawnBalls(int count)
    {
        for (int i = 0; i < count; i++)
            Instantiate(ball, this.gameObject.transform);
    }
}
