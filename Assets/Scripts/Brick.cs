using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class Brick : MonoBehaviour
{
    [Header("Brick Settings")]
    public int health;
    public int baseHealth = 1;

    [Header("Visual Settings")]
    public SpriteRenderer spriteRenderer;
    public Text healthText;
    public Color[] healthColors;

    [Header("Effects")]
    public GameObject destroyEffect;

    private int row;
    private int column;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual();
    }

    public void Initialize(int rowIndex, int colIndex, int level)
    {
        row = rowIndex;
        column = colIndex;

        // Health increases with level and row
        health = baseHealth + level + rowIndex;

        UpdateVisual();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DestroyBrick();
        }
        else
        {
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        // Update health text
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }

        // Update color based on health
        if (spriteRenderer != null && healthColors != null && healthColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(health - 1, 0, healthColors.Length - 1);
            spriteRenderer.color = healthColors[colorIndex];
        }
    }

    private void DestroyBrick()
    {
        // Add score
        if (GameManager.Instance != null)
        {
            int scoreValue = baseHealth + row;
            GameManager.Instance.AddScore(scoreValue);
        }

        // Spawn destroy effect
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Notify brick manager
        BrickManager brickManager = FindObjectOfType<BrickManager>();
        if (brickManager != null)
        {
            brickManager.OnBrickDestroyed(this);
        }

        Destroy(gameObject);
    }

    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return column;
    }

    public void SetRow(int newRow)
    {
        row = newRow;
    }

    public void MoveDown(float distance)
    {
        transform.position += Vector3.down * distance;
        row--;
    }

    public void MoveUp(float distance)
    {
        transform.position += Vector3.up * distance;
        row++;
    }
}
