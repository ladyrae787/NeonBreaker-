using UnityEngine;
using System.Collections.Generic;

public class BrickManager : MonoBehaviour
{
    [Header("Brick Settings")]
    public GameObject brickPrefab;
    public int columns = 7;
    public int rowsPerLevel = 1;
    public float brickWidth = 1.2f;
    public float brickHeight = 0.6f;
    public float spacing = 0.1f;

    [Header("Grid Settings")]
    public Vector2 gridStartPosition = new Vector2(-4f, 4f);
    public float rowMoveDistance = 0.6f;
    public int maxRows = 10;

    [Header("Colors")]
    public Color[] brickColors = new Color[]
    {
        new Color(0.3f, 0.7f, 1f),    // Light Blue
        new Color(0.4f, 0.8f, 0.4f),  // Green
        new Color(1f, 0.8f, 0.2f),    // Yellow
        new Color(1f, 0.5f, 0.2f),    // Orange
        new Color(1f, 0.3f, 0.3f),    // Red
        new Color(0.8f, 0.3f, 0.8f)   // Purple
    };

    private List<Brick> activeBricks = new List<Brick>();
    private int totalBricksInLevel;
    private int bricksDestroyed;

    public void SpawnBricks(int level)
    {
        ClearBricks();

        totalBricksInLevel = 0;
        bricksDestroyed = 0;

        // Spawn rows based on level (1 new row per level, up to maxRows)
        int rowsToSpawn = Mathf.Min(level, maxRows);

        for (int row = 0; row < rowsToSpawn; row++)
        {
            SpawnBrickRow(row, level);
        }
    }

    private void SpawnBrickRow(int rowIndex, int level)
    {
        for (int col = 0; col < columns; col++)
        {
            // Calculate position
            float xPos = gridStartPosition.x + col * (brickWidth + spacing);
            float yPos = gridStartPosition.y - rowIndex * (brickHeight + spacing);

            Vector3 position = new Vector3(xPos, yPos, 0);

            // Instantiate brick
            GameObject brickObj = Instantiate(brickPrefab, position, Quaternion.identity, transform);
            Brick brick = brickObj.GetComponent<Brick>();

            if (brick != null)
            {
                brick.Initialize(rowIndex, col, level);

                // Set color
                SpriteRenderer sr = brick.GetComponent<SpriteRenderer>();
                if (sr != null && brickColors.Length > 0)
                {
                    int colorIndex = rowIndex % brickColors.Length;
                    sr.color = brickColors[colorIndex];
                }

                // Assign color array for health visualization
                brick.healthColors = brickColors;

                activeBricks.Add(brick);
                totalBricksInLevel++;
            }
        }
    }

    public void OnBrickDestroyed(Brick brick)
    {
        activeBricks.Remove(brick);
        bricksDestroyed++;

        // Check if level complete
        if (activeBricks.Count == 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LevelComplete();
            }
        }
    }

    public void MoveBricksDown()
    {
        foreach (Brick brick in activeBricks)
        {
            brick.MoveDown(rowMoveDistance);
        }

        // Check if bricks reached bottom
        if (GetLowestBrickRow() <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void MoveBricksUp()
    {
        // Used when player watches rewarded ad
        foreach (Brick brick in activeBricks)
        {
            brick.MoveUp(rowMoveDistance);
        }
    }

    public int GetLowestBrickRow()
    {
        int lowestRow = int.MaxValue;

        foreach (Brick brick in activeBricks)
        {
            if (brick.GetRow() < lowestRow)
            {
                lowestRow = brick.GetRow();
            }
        }

        return lowestRow == int.MaxValue ? 0 : lowestRow;
    }

    private void ClearBricks()
    {
        foreach (Brick brick in activeBricks)
        {
            if (brick != null)
            {
                Destroy(brick.gameObject);
            }
        }

        activeBricks.Clear();
    }

    public int GetActiveBrickCount()
    {
        return activeBricks.Count;
    }
}
