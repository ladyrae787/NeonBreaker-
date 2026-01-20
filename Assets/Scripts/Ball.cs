using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    [Header("Ball Settings")]
    public float minSpeed = 5f;
    public float maxSpeed = 15f;
    public int bounceCount = 0;

    [Header("Visual Settings")]
    public TrailRenderer trailRenderer;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private SlingshotController slingshotController;
    private Vector3 lastPosition;
    private bool hasHitGround = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        // Configure physics
        rb.gravityScale = 0; // No gravity for ball
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (circleCollider != null)
        {
            circleCollider.radius = 0.15f;
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        // Ensure ball maintains minimum speed
        if (rb.velocity.magnitude < minSpeed && rb.velocity.magnitude > 0.1f)
        {
            rb.velocity = rb.velocity.normalized * minSpeed;
        }

        // Cap maximum speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Check if ball went off screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.x < -100 || screenPos.x > Screen.width + 100 ||
            screenPos.y < -100 || screenPos.y > Screen.height + 100)
        {
            ReturnBall();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceCount++;

        // Handle brick collision
        if (collision.gameObject.CompareTag("Brick"))
        {
            Brick brick = collision.gameObject.GetComponent<Brick>();
            if (brick != null)
            {
                brick.TakeDamage(1);
            }

            // Add slight random angle to prevent infinite loops
            Vector2 currentVelocity = rb.velocity;
            float randomAngle = Random.Range(-5f, 5f);
            rb.velocity = Rotate(currentVelocity, randomAngle);
        }

        // Handle wall collision
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Ensure proper bounce
            Vector2 currentVelocity = rb.velocity;
            float randomAngle = Random.Range(-2f, 2f);
            rb.velocity = Rotate(currentVelocity, randomAngle);
        }

        // Handle ground collision
        if (collision.gameObject.CompareTag("Ground"))
        {
            hasHitGround = true;
            ReturnBall();
        }

        // Maintain speed after collision
        if (rb.velocity.magnitude < minSpeed)
        {
            rb.velocity = rb.velocity.normalized * minSpeed;
        }
    }

    private Vector2 Rotate(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            cos * vector.x - sin * vector.y,
            sin * vector.x + cos * vector.y
        );
    }

    public void SetSlingshotController(SlingshotController controller)
    {
        slingshotController = controller;
    }

    private void ReturnBall()
    {
        if (slingshotController != null)
        {
            slingshotController.OnBallReturned(gameObject);
        }

        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        // Safety check if ball leaves screen
        Invoke(nameof(CheckIfStillInvisible), 2f);
    }

    private void CheckIfStillInvisible()
    {
        if (!GetComponent<Renderer>().isVisible)
        {
            ReturnBall();
        }
    }
}
