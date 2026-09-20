using UnityEngine;

/// <summary>
/// Keeps the camera trailing the player as they move right through a level. It follows X (with a
/// small look-ahead so you can see what's coming) and, by default, holds Y fixed at the level's
/// height so jumps don't bob the view — a flat side-scroller feel. A left clamp stops the camera
/// panning past the start of the level. Single responsibility: camera framing only; it reads the
/// player by tag if no target is wired.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float lookAhead = 2.5f;
    [SerializeField] private float smoothTime = 0.15f;
    [Tooltip("Track the player horizontally (a side-scroller). Off = hold a fixed X for a narrow vertical shaft.")]
    [SerializeField] private bool followX = true;
    [SerializeField] private float fixedX = 0f;
    [Tooltip("Track the player vertically (a climb). Off = hold a fixed Y for a flat side-scroller.")]
    [SerializeField] private bool followY = false;
    [SerializeField] private float fixedY = 0f;
    [SerializeField] private bool clampLeft = true;
    [SerializeField] private float minX = 0f;

    private Vector3 _velocity;

    private void Awake()
    {
        if (target == null)
        {
            var player = GameObject.FindWithTag(targetTag);
            if (player != null)
                target = player.transform;
        }

        if (fixedY == 0f)
            fixedY = transform.position.y; // default to wherever the camera starts
        if (fixedX == 0f)
            fixedX = transform.position.x;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        float x;
        if (followX)
        {
            x = target.position.x + lookAhead;
            if (clampLeft)
                x = Mathf.Max(x, minX);
        }
        else
        {
            x = fixedX;
        }

        float y = followY ? target.position.y : fixedY;

        Vector3 desired = new Vector3(x, y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
    }
}
