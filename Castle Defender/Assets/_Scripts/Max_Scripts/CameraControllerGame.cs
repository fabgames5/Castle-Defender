using Cinemachine;
using UnityEngine;

public class CameraControllerGame : MonoBehaviour
{
    public Transform rts_camera_Target;
    [Tooltip("Add RTS virtual Camera Here")]
    public CinemachineVirtualCamera rts_virtualCam;
    [Tooltip("Add Player Character here")]
    public Transform playerCharacter;
    [Tooltip("Dynamciaclly  Set, is this camera the priority")]
    public bool useThisCam;
    [Space(5)]
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
    [Space(5)]
    [Tooltip("Dynamically Set, the Current Zoom of RTS Camera")]
    public float currentZoom = 20f;

    private void Start()
    {
        if(rts_camera_Target == null)
        {
            rts_camera_Target = transform;
        }
    }

    private void Update()
    {
        if (useThisCam)
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
            rts_camera_Target.Translate(velocity * Time.deltaTime * currentZoom * movementSpeed);


            // Set the camera's child angle based on height
            rts_camera_Target.GetChild(0).localRotation = Quaternion.Euler(currentZoom, rts_camera_Target.rotation.y, rts_camera_Target.rotation.z);


            // Rotate the camera continuously with left/right arrow keys
            if (Input.GetKey(KeyCode.LeftArrow) || (Input.mouseScrollDelta.y > 0 && Input.GetMouseButton(3)))
                rotationVelocity = -1;

            else if (Input.GetKey(KeyCode.RightArrow) || (Input.mouseScrollDelta.y < 0 && Input.GetMouseButton(3)))
                rotationVelocity = 1;

            rotationVelocity = rotationVelocity * (1 - rotationDeceleration * Time.deltaTime);
            rts_camera_Target.Rotate(Vector3.up, rotationVelocity * rotationSpeed * Time.deltaTime);

            // Constrain camera position within bounds
            rts_camera_Target.position = new Vector3(
                Mathf.Clamp(rts_camera_Target.position.x, -maxBounds.x / 2, maxBounds.x / 2),
                Mathf.Clamp(currentZoom, minZoom, maxZoom),
                Mathf.Clamp(rts_camera_Target.position.z, -maxBounds.y / 2, maxBounds.y / 2)
            );
        }
        else
        {
            rts_camera_Target.position = (playerCharacter.position + (Vector3.up * minZoom));
        }

        // Zoom in/out vertically with up/down arrow keys
        if (Input.GetKey(KeyCode.UpArrow) || (Input.mouseScrollDelta.y < 0 && !Input.GetMouseButton(3)))
        {
            zoomVelocity = 1 + currentZoom / 5;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || (Input.mouseScrollDelta.y > 0 && !Input.GetMouseButton(3)))
        {
            zoomVelocity = -(1 + currentZoom / 5);
        }
        zoomVelocity = zoomVelocity * (1 - Time.deltaTime * zoomDeceleration);
        currentZoom += Time.deltaTime * zoomSpeed * zoomVelocity;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        //changes camera to player character
        if (currentZoom <= minZoom)
        {
            ChangeCameraPriority(false);
        }
        if (currentZoom > minZoom)
        {
            ChangeCameraPriority(true);
        }


    }

    /// <summary>
    /// sets the virtual camera to either 100 or 1 based off of priority
    /// </summary>
    /// <param name="isPriority"></param>
    void ChangeCameraPriority(bool isPriority)
    {
        if (isPriority)
        {
            rts_virtualCam.Priority = 100;
            useThisCam = true;
        }
        else
        {
            rts_virtualCam.Priority = 1;
            useThisCam = false;
        }        
    }
}
