using UnityEngine;

public class MarkerController : MonoBehaviour
{
    public float moveSpeed = 10f;

    // Limits to keep the marker on the pitch
    public float xLimit = 1.5f;
    public float zMin = -5f;
    public float zMax = 15f;

    private Transform mainCameraTransform;

    void Start()
    {
        // Find the main camera automatically when the game starts
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("No camera tagged as 'MainCamera' found in the scene!");
        }
    }

    void Update()
    {
        // Capture WASD input
        float moveX = Input.GetAxis("Horizontal"); // A, D
        float moveZ = Input.GetAxis("Vertical");   // W, S

        if (mainCameraTransform != null)
        {
            // 1. Get the camera's forward and right directions
            Vector3 camForward = mainCameraTransform.forward;
            Vector3 camRight = mainCameraTransform.right;

            // 2. Flatten the vectors on the Y axis. 
            // If the camera is looking down, we don't want the marker to move into the ground!
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 3. Calculate the movement direction relative to the camera
            Vector3 moveDirection = (camForward * moveZ) + (camRight * moveX);

            // 4. Update position based on that new direction
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // Clamp the position so it doesn't fly off the pitch
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -xLimit, xLimit);
        pos.z = Mathf.Clamp(pos.z, zMin, zMax);
        transform.position = pos;
    }
}