using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float maxRotationRange = 90f;
    [SerializeField] private float rotationSpeed = 500f;
    float inputX, inputY;

    void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        RotateController();
    }

    void RotateController()
    {
        inputX -= Input.GetAxisRaw("Mouse Y") * sensitivity;
        inputY += Input.GetAxisRaw("Mouse X") * sensitivity;
        float clampedX = Mathf.Clamp(inputX, -maxRotationRange, maxRotationRange);

        Quaternion targetRotation = Quaternion.Euler(clampedX, inputY, transform.eulerAngles.z);
        transform.rotation = targetRotation;
    }
}
