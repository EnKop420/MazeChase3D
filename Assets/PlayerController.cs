using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public Rigidbody rb;

    private Vector3 movementInput;
    private float mouseX;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false; // Disable script for non-owners
            return;
        }

        // Attach the main camera to the player's head
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 1.5f, -2.5f); // Adjust for your game
        Camera.main.transform.localRotation = Quaternion.identity;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.isKinematic = false; // Ensure movement works for the owner

        networkPosition.Value = transform.position;
        networkRotation.Value = transform.rotation;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            transform.position = networkPosition.Value;
            transform.rotation = networkRotation.Value;
            return;
        }

        HandleInput();
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            MovePlayer();
            RotatePlayer();

            UpdateNetworkPositionAndRotationServerRpc(transform.position, transform.rotation);
        }
        else
        {
            transform.position = networkPosition.Value;
            transform.rotation = networkRotation.Value;
        }
    }

    void HandleInput()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D for strafing
        float moveZ = Input.GetAxis("Vertical");     // W/S for forward-backward

        movementInput = transform.forward * moveZ + transform.right * moveX;

        mouseX = Input.GetAxis("Mouse X") * rotationSpeed; // Get mouse movement
    }

    void MovePlayer()
    {
        rb.linearVelocity = new Vector3(movementInput.x * moveSpeed, rb.linearVelocity.y, movementInput.z * moveSpeed);
    }

    void RotatePlayer()
    {
        transform.Rotate(Vector3.up * mouseX);
    }

    [ServerRpc]
    private void UpdateNetworkPositionAndRotationServerRpc(Vector3 position, Quaternion rotation)
    {
        networkPosition.Value = position;
        networkRotation.Value = rotation;
    }

    //public float moveSpeed = 5f;
    //public float rotationSpeed = 5f;
    //public Rigidbody rb;

    //private Vector3 movementInput;
    //private float mouseX;

    //private void Start()
    //{
    //    if (!IsOwner)
    //    {
    //        enabled = false; // Disable script for non-owners
    //        return;
    //    }

    //    // Attach the main camera to the player's head
    //    Camera.main.transform.SetParent(transform);
    //    Camera.main.transform.localPosition = new Vector3(0, 1.5f, -2.5f); // Adjust for your game
    //    Camera.main.transform.localRotation = Quaternion.identity;

    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //    rb.isKinematic = false; // Ensure movement works for the owner
    //}

    //private void Update()
    //{
    //    if (!IsOwner) return;

    //    HandleInput();
    //}

    //private void FixedUpdate()
    //{
    //    if (!IsOwner) return;

    //    MovePlayer();
    //    RotatePlayer();
    //}

    //void HandleInput()
    //{
    //    float moveX = Input.GetAxis("Horizontal"); // A/D for strafing
    //    float moveZ = Input.GetAxis("Vertical");   // W/S for forward-backward

    //    movementInput = transform.forward * moveZ + transform.right * moveX;

    //    mouseX = Input.GetAxis("Mouse X") * rotationSpeed; // Get mouse movement
    //}

    //void MovePlayer()
    //{
    //    rb.linearVelocity = new Vector3(movementInput.x * moveSpeed, rb.linearVelocity.y, movementInput.z * moveSpeed);
    //}

    //void RotatePlayer()
    //{
    //    transform.Rotate(Vector3.up * mouseX);
    //}
}
