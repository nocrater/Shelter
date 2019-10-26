using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Camera))]
public class RtsCamera : MonoBehaviour
{
    [Header("Mouse")] [SerializeField] private float mouseMovementSpeed = 3f;
    [Space] [SerializeField] private bool useScreenEdgeInput = true;
    [SerializeField] private float screenEdgeMovementSpeed = 3f;
    [SerializeField] private float screenEdgeBorder = 25f;

    [Header("Keyboard")] [SerializeField] private bool useKeyboardInput = true;
    [SerializeField] private float keyboardMovementSpeed = 5f;
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    [Header("Target")] [SerializeField] private Transform targetFollow;
    [SerializeField] private Vector3 targetOffset = new Vector3();
    [SerializeField] private float followingSpeed = 5f;

    [Header("Map")] [SerializeField] private bool limitMap = true;
    [SerializeField] private float limitX = 50f;
    [SerializeField] private float limitY = 50f;

    private Camera mainCamera;
    private Transform cameraTransform;

    private Vector3 mouseDelta = new Vector3();
    private Vector3 mouseLastPosition = new Vector3();

    private float startingOrthographicSize;

    private bool FollowingTarget => targetFollow != null;

    private Vector2 KeyboardInput =>
        useKeyboardInput
            ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis))
            : Vector2.zero;

    private Vector2 MouseInput => Input.mousePosition;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraTransform = transform;
        cameraTransform.position = cameraTransform.position;

        startingOrthographicSize = mainCamera.orthographicSize;
    }

    private void Update()
    {
        if (FollowingTarget)
            FollowTarget();
        else
            Move();

        LimitPosition();
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseLastPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            mouseDelta = mouseLastPosition - Input.mousePosition;

            Vector3 desiredMove = new Vector3(cameraTransform.position.x + mouseDelta.x, cameraTransform.position.y + mouseDelta.y,
                cameraTransform.position.z);

            cameraTransform.position = Vector3.MoveTowards(cameraTransform.position,
                desiredMove, mouseMovementSpeed * (mainCamera.orthographicSize / startingOrthographicSize) * Time.deltaTime);

            mouseLastPosition = Input.mousePosition;
        }

        if (useKeyboardInput)
        {
            Vector3 desiredMove = new Vector3(KeyboardInput.x, KeyboardInput.y, 0);

            desiredMove = Vector3.Lerp(Vector3.zero, desiredMove, Time.deltaTime / (mouseMovementSpeed * (mainCamera.orthographicSize / startingOrthographicSize)));
            desiredMove = cameraTransform.InverseTransformDirection(desiredMove);

            cameraTransform.Translate(desiredMove, Space.Self);
        }

        if (useScreenEdgeInput)
        {
            Vector3 desiredMove = new Vector3();

            Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
            Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
            Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
            Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

            desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
            desiredMove.y = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

            desiredMove *= screenEdgeMovementSpeed * (mainCamera.orthographicSize / startingOrthographicSize);
            desiredMove *= Time.deltaTime;
            desiredMove = cameraTransform.InverseTransformDirection(desiredMove);

            cameraTransform.Translate(desiredMove, Space.Self);
        }
    }

    private void FollowTarget()
    {
        Vector3 targetPos = new Vector3(targetFollow.position.x, cameraTransform.position.y, targetFollow.position.z) +
                            targetOffset;
        cameraTransform.position =
            Vector3.MoveTowards(cameraTransform.position, targetPos, Time.deltaTime * followingSpeed);
    }

    private void LimitPosition()
    {
        if (!limitMap)
            return;

        float camSize = mainCamera.orthographicSize;
        float aspect = mainCamera.aspect;

        cameraTransform.position = new Vector3(
            Mathf.Clamp(cameraTransform.position.x, -limitX + camSize * aspect, limitX - camSize * aspect),
            Mathf.Clamp(cameraTransform.position.y, -limitY + camSize, limitY - camSize),
            cameraTransform.position.z);
    }

    public void SetTarget(Transform target)
    {
        targetFollow = target;
    }

    public void ResetTarget()
    {
        targetFollow = null;
    }
}