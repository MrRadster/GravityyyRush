using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityShifter : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;

    [Header("Settings")]
    public float gravityStrength = 20f;
    public float floatRotationSpeed = 5f;
    public float raycastDistance = 1.5f;
    public LayerMask walkableLayers;

    private Rigidbody rb;
    private bool isFloating = false;
    private Vector3 currentGravityDir = Vector3.down;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        HandleInput();

        if (isFloating)
        {
            Vector3 targetForward = playerCamera.forward;
            if (targetForward != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(targetForward, -currentGravityDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, floatRotationSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        if (!isFloating)
        {
            rb.AddForce(currentGravityDir * gravityStrength, ForceMode.Acceleration);
            CheckForSurfaceAlignment();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isFloating = !isFloating;

            if (isFloating)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                currentGravityDir = -playerCamera.forward;
                transform.rotation = Quaternion.LookRotation(transform.forward, -currentGravityDir);
            }
        }
    }

    void CheckForSurfaceAlignment()
    {
        if (Physics.Raycast(transform.position, currentGravityDir, out RaycastHit hit, raycastDistance, walkableLayers))
        {
            Vector3 surfaceNormal = hit.normal;
            currentGravityDir = -surfaceNormal;

            Quaternion targetRot = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, floatRotationSpeed * Time.fixedDeltaTime);
        }
    }
}