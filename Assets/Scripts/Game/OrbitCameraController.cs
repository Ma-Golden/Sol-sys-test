using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    public Transform target; // Central body to orbit around
    public Camera cam;

    public float panSpeed = 10f;
    public float zoomSpeed = 5f;
    public float rotationSpeed = 3f;
    public float minZoom = 1.5f;
    public float maxZoom = 60f;
    private Vector3 _lastMousePos;
    private Vector3 _currentRotation;
    private bool isDragging;
    //private bool _isOrbiting = false;

    private float oldPos;
    private float panOrigin;


    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("OrbitCamController: No target assigned, defaulting to world origin");
            target = new GameObject("Camera Target").transform;
        }
        if (cam == null)
            cam = Camera.main;

        _currentRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) Debug.Log("Right-click detected"); // N.B: DEBUG

        HandleCameraDrag();
        HandleCameraZoom();
        HandleOrbit();
    }

    private void HandleCameraDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {

            _lastMousePos = Input.mousePosition;
        }

        if(Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - _lastMousePos;


            // Convert mouse movement to world movement
            Vector3 move = transform.right * -delta.x + transform.up * -delta.y;

            transform.position += move * panSpeed * Time.deltaTime;

            _lastMousePos = Input.mousePosition;
        }

    }

    private void HandleCameraZoom() 
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta == 0) return; // return if not zooming

        float distance = (transform.position - target.position).magnitude;
        float zoomAmount = scrollDelta * zoomSpeed;
        float newDistance = Mathf.Clamp(distance - zoomAmount, minZoom, maxZoom);

        transform.position = target.position + (transform.position - target.position).normalized * newDistance;
    }

    private void HandleOrbit()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(0)) // Ctrl + Left Click to orbit
        {
//            _isOrbiting = true;
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            _currentRotation.x -= mouseY;
            _currentRotation.y += mouseX;
            _currentRotation.x = Mathf.Clamp(_currentRotation.x, 10f, 80f); // Prevent flipping

            Quaternion rotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
            Vector3 direction = rotation * Vector3.back * (transform.position - target.position).magnitude;

            transform.position = target.position + direction;
            transform.LookAt(target.position);
        }
        else
        {
//            _isOrbiting = false;
        }
    }

    // Resets the camera to top down view (of system)
    public void ResetToTopDown()
    {
        _currentRotation = new Vector3(90f, 0f, 0f);
        transform.position = target.position - new Vector3(0, cam.orthographicSize * 2, 0);
        transform.LookAt(target.position);
    }

}
