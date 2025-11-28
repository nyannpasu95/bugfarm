using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    // Camera movement boundaries
    public float minX, maxX, minY, maxY;

    void LateUpdate()
    {
        if (target == null) return;

        // Target position
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Clamp within boundaries
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
