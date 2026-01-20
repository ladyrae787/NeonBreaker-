using UnityEngine;
using System.Collections.Generic;

public class SlingshotController : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform launchPoint;
    public float maxLaunchForce = 20f;
    public float minLaunchForce = 5f;
    public float dragMultiplier = 2f;
    public int ballsToLaunch = 1;

    [Header("Visual Feedback")]
    public LineRenderer trajectoryLine;
    public int trajectoryPoints = 20;
    public float trajectoryPointSpacing = 0.1f;
    public Color trajectoryColor = Color.white;

    [Header("Launch Settings")]
    public float ballLaunchDelay = 0.1f;
    public float maxDragDistance = 3f;

    private Vector2 startDragPosition;
    private Vector2 currentDragPosition;
    private bool isDragging = false;
    private Vector3 originalPosition;
    private List<GameObject> activeBalls = new List<GameObject>();
    private int ballsLaunched = 0;
    private bool hasLaunched = false;

    private void Start()
    {
        originalPosition = launchPoint.position;

        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
            trajectoryLine.startColor = trajectoryColor;
            trajectoryLine.endColor = trajectoryColor;
            trajectoryLine.startWidth = 0.1f;
            trajectoryLine.endWidth = 0.05f;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        // Handle touch and mouse input
        if (Input.GetMouseButtonDown(0) && !hasLaunched)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if touching near launch point
            if (Vector2.Distance(mousePos, launchPoint.position) < 2f)
            {
                startDragPosition = mousePos;
                isDragging = true;

                if (trajectoryLine != null)
                    trajectoryLine.enabled = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            currentDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Clamp drag distance
            Vector2 dragVector = startDragPosition - currentDragPosition;
            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
                currentDragPosition = startDragPosition - dragVector;
            }

            UpdateTrajectoryLine();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            if (trajectoryLine != null)
                trajectoryLine.enabled = false;

            LaunchBalls();
        }
    }

    private void UpdateTrajectoryLine()
    {
        if (trajectoryLine == null) return;

        Vector2 dragVector = startDragPosition - currentDragPosition;
        Vector2 launchVelocity = dragVector * dragMultiplier;

        // Clamp launch force
        float forceMagnitude = Mathf.Clamp(launchVelocity.magnitude, minLaunchForce, maxLaunchForce);
        launchVelocity = launchVelocity.normalized * forceMagnitude;

        trajectoryLine.positionCount = trajectoryPoints;

        Vector2 startPos = launchPoint.position;
        Vector2 velocity = launchVelocity;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * trajectoryPointSpacing;
            Vector2 point = startPos + velocity * t + 0.5f * Physics2D.gravity * t * t;
            trajectoryLine.SetPosition(i, point);
        }
    }

    private void LaunchBalls()
    {
        Vector2 dragVector = startDragPosition - currentDragPosition;
        Vector2 launchVelocity = dragVector * dragMultiplier;

        float forceMagnitude = Mathf.Clamp(launchVelocity.magnitude, minLaunchForce, maxLaunchForce);
        launchVelocity = launchVelocity.normalized * forceMagnitude;

        ballsToLaunch = GameManager.Instance.ballsCollected;
        ballsLaunched = 0;
        hasLaunched = true;

        StartCoroutine(LaunchBallsSequentially(launchVelocity));
    }

    private System.Collections.IEnumerator LaunchBallsSequentially(Vector2 velocity)
    {
        for (int i = 0; i < ballsToLaunch; i++)
        {
            GameObject ball = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = velocity;
            }

            Ball ballScript = ball.GetComponent<Ball>();
            if (ballScript != null)
            {
                ballScript.SetSlingshotController(this);
            }

            activeBalls.Add(ball);
            ballsLaunched++;

            yield return new WaitForSeconds(ballLaunchDelay);
        }
    }

    public void OnBallReturned(GameObject ball)
    {
        activeBalls.Remove(ball);

        if (activeBalls.Count == 0 && hasLaunched)
        {
            // All balls returned
            hasLaunched = false;
            GameManager.Instance.OnBallLost();
        }
    }

    public void ResetPosition()
    {
        launchPoint.position = originalPosition;
        hasLaunched = false;
        activeBalls.Clear();
    }

    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }
}
