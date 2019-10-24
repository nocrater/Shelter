using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RtsCamera : MonoBehaviour
{
    [Header("Screen Edge")] [SerializeField]
    private bool useScreenEdgeInput = true;

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

    private Transform cameraTransform;
    private Camera mainCamera;
    private float orthographicSize;
    private float panningVelocity;

    private bool FollowingTarget => targetFollow != null;

    private Vector2 KeyboardInput =>
        useKeyboardInput
            ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis))
            : Vector2.zero;

    private Vector2 MouseInput => Input.mousePosition;

    private void Start()
    {
        cameraTransform = transform;
        mainCamera = Camera.main;
        if (mainCamera != null) 
            orthographicSize = mainCamera.orthographicSize;
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
        if (useKeyboardInput)
        {
            Vector3 desiredMove = new Vector3(KeyboardInput.x, KeyboardInput.y, 0);

            desiredMove *= keyboardMovementSpeed;
            desiredMove *= Time.deltaTime;
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

            desiredMove *= screenEdgeMovementSpeed;
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

        cameraTransform.position = new Vector3(
            Mathf.Clamp(cameraTransform.position.x, -limitX, limitX),
            Mathf.Clamp(cameraTransform.position.y, -limitY, limitY),
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