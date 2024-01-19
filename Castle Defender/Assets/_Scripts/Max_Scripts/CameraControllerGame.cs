using UnityEngine;

public class CameraControllerGame : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float deceleration = 5f;
    public float rotationSpeed = 50f;
    public float zoomSpeed = 2f;

    public Vector2 maxBounds = new Vector2(100f, 100f);
    public float minZoom = 5f;
    public float maxZoom = 40f;

    private Vector3 velocity = Vector3.zero;
    private float currentZoom = 20f;

    private void Update()
    {
        // Check for WASD key presses and adjust velocity
        if (Input.GetKey(KeyCode.W))
            velocity += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            velocity -= Vector3.forward;
        if (Input.GetKey(KeyCode.A))
            velocity -= Vector3.right;
        if (Input.GetKey(KeyCode.D))
            velocity += Vector3.right;

        // Apply deceleration when no keys are pressed
        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * deceleration);

        // Move the camera based on velocity
        transform.Translate(velocity * Time.deltaTime);

        // Rotate the camera continuously with left/right arrow keys
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Zoom in/out vertically with up/down arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentZoom = Mathf.Min(currentZoom + zoomSpeed * Time.deltaTime, maxZoom);
            transform.Translate(Vector3.up * Time.deltaTime * zoomSpeed);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentZoom = Mathf.Max(currentZoom - zoomSpeed * Time.deltaTime, minZoom);
            transform.Translate(Vector3.down * Time.deltaTime * zoomSpeed);
        }

        // Constrain camera position within bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -maxBounds.x / 2, maxBounds.x / 2),
            currentZoom,
            Mathf.Clamp(transform.position.z, -maxBounds.y / 2, maxBounds.y / 2)
        );
    }
}
