using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float maxPitchRange = 90f;
    [SerializeField] private float rotationSpeed = 500f;

    [System.NonSerialized] public Vector3 rotationInput;
    [System.NonSerialized] public float inputX, inputY;
    [System.NonSerialized] public bool specialActionInput;

    private float rotationInputX, rotationInputY;

    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        specialActionInput = Input.GetKeyDown(KeyCode.Space);

        rotationInputX += inputY * sensitivity;
        rotationInputY += inputX * sensitivity;
        rotationInputX = Mathf.Clamp(rotationInputX, -maxPitchRange, maxPitchRange);
        rotationInput = new Vector3(rotationInputX, rotationInputY, transform.eulerAngles.z);
    }
}
