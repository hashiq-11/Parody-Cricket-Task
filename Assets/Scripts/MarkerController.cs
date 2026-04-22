using UnityEngine;

public class MarkerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    // Limits to keep the marker on the pitch
    public float xLimit = 1.5f;
    public float zMin = -5f;
    public float zMax = 15f;

    void Update()
    {
        // Capture WASD input
        float moveX = Input.GetAxis("Horizontal"); // A, D
        float moveZ = Input.GetAxis("Vertical");   // W, S

        // Calculate movement
        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;

        // Update position 
        transform.Translate(move, Space.World);

        // Clamp the position so it doesn't fly off the pitch
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -xLimit, xLimit);
        pos.z = Mathf.Clamp(pos.z, zMin, zMax);
        transform.position = pos;
    }
}