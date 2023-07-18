using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float rotationSpeed = 100f;
    public float zoomSpeed = 5f;

    private float yaw = 0f;
    private float pitch = 0f;

    private void Start() {
        Cursor.visible=false;
        Cursor.lockState=CursorLockMode.Locked;
    }
    private void Update()
    {
        
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

       
        transform.Translate(new Vector3(horizontalMovement, 0, verticalMovement) * movementSpeed * Time.deltaTime);

        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

       
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.forward * scrollWheel * zoomSpeed, Space.Self);
    }
}
