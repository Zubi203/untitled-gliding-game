using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float maxPitchRange = 90f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private Rigidbody playerRigidbody;

    [System.NonSerialized] public Vector3 rotationInput;
    [System.NonSerialized] public float inputX, inputY;
    [System.NonSerialized] public bool specialActionInput;

    private float rotationInputX, rotationInputY;

    void Awake()
    {
        rotationInput = transform.rotation.eulerAngles;
    }
    void FixedUpdate()
    {
        GetRotationInput();
        //AlignRotationInputToVelocity();
    }

    void Update()
    {
        specialActionInput = Input.GetKeyDown(KeyCode.Space);
    }

    void GetRotationInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        rotationInputX += inputY * sensitivity;
        rotationInputY += inputX * sensitivity;
        rotationInputX = Mathf.Clamp(rotationInputX, -maxPitchRange, maxPitchRange);
        rotationInput = new Vector3(rotationInputX, rotationInputY, transform.eulerAngles.z);
    }

    void AlignRotationInputToVelocity()
    {
        if (playerRigidbody == null) return;
        if (inputY > 0f) return;

        Quaternion targetRotation = Quaternion.LookRotation(rotationInput, playerRigidbody.linearVelocity);
        rotationInput = new Vector3(targetRotation.x, rotationInput.y, rotationInput.z);
    }
}
