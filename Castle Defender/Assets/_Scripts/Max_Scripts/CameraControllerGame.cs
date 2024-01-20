using UnityEngine;

public class CameraControllerGame : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float deceleration = 5f;
    public float rotationSpeed = 50f;
    public float rotationVelocity = 0;
    public float rotationDeceleration = 2;

    public float zoomSpeed = 2f;

    public Vector2 maxBounds = new Vector2(100f, 100f);
    public float minZoom = 5f;
    public float maxZoom = 40f;

    private Vector3 velocity = Vector3.zero;
    public float zoomVelocity = 0;
    public float zoomDeceleration = 5f;
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
        transform.Translate(velocity * Time.deltaTime * currentZoom * movementSpeed);
        

        // Set the camera's angle based on height
        Camera.main.transform.localRotation = Quaternion.Euler(currentZoom, transform.rotation.y, transform.rotation.z);


        // Rotate the camera continuously with left/right arrow keys
        if (Input.GetKey(KeyCode.LeftArrow) || (Input.mouseScrollDelta.y > 0 && Input.GetMouseButton(3)))
            rotationVelocity = -1;
            
        else if (Input.GetKey(KeyCode.RightArrow) || (Input.mouseScrollDelta.y < 0 && Input.GetMouseButton(3)))
            rotationVelocity = 1;

        rotationVelocity = rotationVelocity*(1-rotationDeceleration * Time.deltaTime);
        transform.Rotate(Vector3.up, rotationVelocity * rotationSpeed * Time.deltaTime);

        // Zoom in/out vertically with up/down arrow keys
        if (Input.GetKey(KeyCode.UpArrow) || (Input.mouseScrollDelta.y < 0 && !Input.GetMouseButton(3)))
        {
            zoomVelocity=1+currentZoom/5;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || (Input.mouseScrollDelta.y > 0 && !Input.GetMouseButton(3)))
        {
            zoomVelocity=-(1+currentZoom/5);
        }
        zoomVelocity = zoomVelocity * (1 - Time.deltaTime * zoomDeceleration);
        currentZoom += Time.deltaTime * zoomSpeed * zoomVelocity;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Constrain camera position within bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -maxBounds.x / 2, maxBounds.x / 2),
            Mathf.Clamp(currentZoom, minZoom, maxZoom),
            Mathf.Clamp(transform.position.z, -maxBounds.y / 2, maxBounds.y / 2)
        );
    }
}
