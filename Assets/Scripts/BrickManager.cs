using UnityEngine;
using System.Collections.Generic;

public class BrickManager : MonoBehaviour
{
    public GameObject brickPrefab;
    public int columns = 8;
    public float rowHeight = 0.5f;
    public float moveSpeed = 0.1f;
    public float spawnY = 5f;
    public float gameOverY = -5f;

    private List<GameObject> activeBricks = new List<GameObject>();

    void Start()
    {
        SpawnRow();
    }

    void Update()
    {
        MoveBricksDown();

        // Spawn new row when top-most row has moved down one step
        if (activeBricks.Count > 0)
        {
            float highestY = activeBricks[activeBricks.Count - 1].transform.position.y;
            if (highestY <= spawnY)
                SpawnRow();
        }

        // Check for Game Over
        foreach (var brick in activeBricks)
        {
            if (brick.transform.position.y <= gameOverY)
            {
                GameOver();
                break;
            }
        }
    }

    void SpawnRow()
    {
        for (int i = 0; i < columns; i++)
        {
            Vector3 pos = new Vector3(-3.5f + i, spawnY, 0);
            var b = Instantiate(brickPrefab, pos, Quaternion.identity, transform);
            activeBricks.Add(b);
        }
    }

    void MoveBricksDown()
    {
        for (int i = 0; i < activeBricks.Count; i++)
        {
            activeBricks[i].transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }
    }

    void GameOver()
    {
        Time.timeScale = 0;
        // TODO: trigger your “Watch Ad to Continue” UI here
    }
}
