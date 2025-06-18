using UnityEngine;
using System.Collections;
using System;

public class camera_movement : MonoBehaviour
{
    public float panSpeed = 20.0F; // Speed for panning (XZ movement)
    public float zoomSpeed = 200.0F; // Speed for zooming (distance change)
    public float minZoomDistance = 10.0F; // Minimum distance from the ground
    public float maxZoomDistance = 50.0F; // Maximum distance from the ground

    // The target point on the ground plane that the camera is "looking at" or orbiting around
    private Vector3 zoomTarget = Vector3.zero; // Default to origin

    // Store the camera's fixed angle (isometric rotation)
    private Quaternion fixedRotation;

    private Vector3? isAutoMovingTowards = null;
    private float currentAutoMoveSpeed = 0;
    private float maxAutoMoveSpeed = 100f;
    private float startDeceleratingAt = 50f;

    void Start()
    {
        // Store the camera's initial rotation as the fixed isometric angle
        fixedRotation = transform.rotation;

        // Initialize the zoomTarget based on the camera's initial position
        // This assumes the camera is initially positioned to look at a point on the ground
        // A raycast from the screen center to the ground is a robust way to find this
        UpdateZoomTarget();

        // Optional: Adjust initial position to be at a valid zoom distance if needed
        float currentDistance = Vector3.Distance(transform.position, zoomTarget);
        if (currentDistance < minZoomDistance || currentDistance > maxZoomDistance)
        {
            // If the initial distance is outside bounds, place the camera at the average distance
            float initialDistance = (minZoomDistance + maxZoomDistance) / 2f;
            Vector3 directionFromTarget = (transform.position - zoomTarget).normalized;
            transform.position = zoomTarget + directionFromTarget * initialDistance;
        }

        // Ensure the camera maintains the fixed rotation
        transform.rotation = fixedRotation;
    }

    void Update()
    {
        bool cancelAutoMove = false;

        // Используем клавиши со стрелками для движения камеры
        float panHorizontal = Input.GetAxis("HorizontalArrow"); // Горизонтальные стрелки
        float panVertical = Input.GetAxis("VerticalArrow"); // Вертикальные стрелки
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        // Handle Panning (XZ movement)
        Vector3 panMovement = new Vector3(panHorizontal, 0, panVertical).normalized * panSpeed * Time.deltaTime;
        transform.position += panMovement;

        // Handle Zoom (changing distance from the ground)
        // We need to move the camera along the ray from the zoomTarget
        float currentDistance = Vector3.Distance(transform.position, zoomTarget);
        float zoomAmount = mouseScroll * zoomSpeed * Time.deltaTime;
        float newDistance = currentDistance - zoomAmount;

        // Clamp the new distance within the min/max bounds
        newDistance = Mathf.Clamp(newDistance, minZoomDistance, maxZoomDistance);

        // Calculate the new camera position along the ray from the zoomTarget
        Vector3 directionFromTarget = (transform.position - zoomTarget).normalized;
        // If the current position is exactly at the target, we need a default direction
        if (directionFromTarget == Vector3.zero)
        {
            // Use the direction implied by the fixed rotation if at the target
            directionFromTarget = (fixedRotation * Vector3.forward).normalized;
        }

        transform.position = zoomTarget + directionFromTarget * newDistance;

        // Ensure the camera always maintains the fixed rotation
        transform.rotation = fixedRotation;

        // Update the zoomTarget based on the camera's current position and rotation
        // This is important for the camera to orbit around the point it's looking at
        UpdateZoomTarget();

        // cancel the auto movement if the player pans the camera
        cancelAutoMove = cancelAutoMove || (panHorizontal != 0 || panVertical != 0);
        if (cancelAutoMove || (isAutoMovingTowards.HasValue && Vector3.Distance(transform.position, isAutoMovingTowards.Value) < 0.1f)) // Use a small tolerance for comparison
        {
            isAutoMovingTowards = null;
            currentAutoMoveSpeed = 0;
        }

        // Auto Movement (FlyToTarget)
        if (isAutoMovingTowards.HasValue)
        {
             // Move the camera's XZ position towards the target's XZ position
             Vector3 currentXZ = new Vector3(transform.position.x, 0, transform.position.z);
             Vector3 targetXZ = new Vector3(isAutoMovingTowards.Value.x, 0, isAutoMovingTowards.Value.z);

             var tgtDist = (targetXZ - currentXZ).sqrMagnitude;

            if (tgtDist > startDeceleratingAt * startDeceleratingAt) // Compare squared distances
            {
                var speedIncrease = (maxAutoMoveSpeed - currentAutoMoveSpeed) * 15f * Time.deltaTime;
                currentAutoMoveSpeed += speedIncrease;
            }
            else
            {
                var nearGoalSlow = (1f - (startDeceleratingAt - Mathf.Sqrt(tgtDist)) / startDeceleratingAt) * 13f * Time.deltaTime + 0.5f;
                currentAutoMoveSpeed = Mathf.Max(maxAutoMoveSpeed * nearGoalSlow, 4f);
            }

            // Apply auto movement only to the XZ plane
            Vector3 newXZPosition = Vector3.MoveTowards(currentXZ, targetXZ, currentAutoMoveSpeed * Time.deltaTime);
            transform.position = new Vector3(newXZPosition.x, transform.position.y, newXZPosition.z);

            // During auto-movement, the zoomTarget should follow the target point
            zoomTarget = isAutoMovingTowards.Value;
        }
    }

    // Helper function to update the point on the ground the camera is looking at
    void UpdateZoomTarget()
    {
        // Raycast from the center of the screen to find the point on the ground plane
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // Assuming your ground is a flat plane at Y=0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float hitDistance;

        if (groundPlane.Raycast(ray, out hitDistance))
        {
            zoomTarget = ray.GetPoint(hitDistance);
        }
        else
        {
            // Fallback if the ray doesn't hit the ground (e.g., camera too high or looking away)
            // Project the camera's XZ position onto the ground plane
            zoomTarget = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    public void FlyToTarget(Vector3 target)
    {
        isAutoMovingTowards = target;
        currentAutoMoveSpeed = 0; // Reset speed at the start of a new auto-move
    }
}