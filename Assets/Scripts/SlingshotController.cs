using UnityEngine;

public class SlingshotController : MonoBehaviour
{
    public float maxPullDistance = 3f;
    public float powerMultiplier = 10f;

    private Vector2 startPos;
    private Rigidbody2D rb;
    private bool isDragging;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        rb.isKinematic = true;
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseDrag()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - startPos;
        if (dir.magnitude > maxPullDistance) 
            dir = dir.normalized * maxPullDistance;
        transform.position = startPos + dir;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        Vector2 launchDir = (startPos - (Vector2)transform.position).normalized;
        float power = Vector2.Distance(startPos, transform.position) * powerMultiplier;

        rb.isKinematic = false;
        rb.velocity = launchDir * power;
    }
}
