using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float maxPitchRange = 90f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private Rigidbody playerRigidbody;

    [System.NonSerialized] public Vector3 rotationInput;
    [System.NonSerialized] public float inputX, inputY;
    [System.NonSerialized] public bool specialActionInput;
    private InputAction moveAction, specialAction;
    private float rotationInputX, rotationInputY;

    void Awake()
    {
        Assert.IsNotNull(inputActions, "Input Actions Asset has not been assigned");

        rotationInput = transform.rotation.eulerAngles;
    }

    void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        specialAction = InputSystem.actions.FindAction("Special");
    }

    void FixedUpdate()
    {
        GetRotationInput();
        //AlignRotationInputToVelocity();
    }

    void Update()
    {
        specialActionInput = specialAction.WasPressedThisFrame();
    }

    void GetRotationInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        inputX = input.x;
        inputY = input.y;

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
