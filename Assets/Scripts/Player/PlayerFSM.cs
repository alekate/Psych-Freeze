using UnityEngine;
public class PlayerFSM : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float airControlForce = 5f;
    public int maxJumps = 2;
    public Transform cameraPivot;

    [Header("Ground Check Settings")]
    public LayerMask groundLayer;

    [HideInInspector] public bool isGrounded;


    public Vector2 turn;

    public Rigidbody rb;
    public int jumps;

    public Terrain terrain;
    public float maxSlopeAngle = 45f;
    public float currentSlopeAngle;

    public IPlayerState idleState;
    public IPlayerState moveState;
    public IPlayerState jumpState;
    public IPlayerState airMoveState;

    private IPlayerState currentState;
    [SerializeField] private string currentStateName;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraPivot = transform.Find("Camera Pivot");
        groundLayer = LayerMask.GetMask("Ground");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        idleState = new PlayerIdleState();
        moveState = new PlayerMoveState();
        jumpState = new PlayerJumpState();
        airMoveState = new PlayerAirMoveState();

        currentState = idleState;
        currentState.Enter(this);
    }


    private void Update()
    {
       currentState?.Update();

        currentStateName = currentState.ToString();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void SwitchState(IPlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter(this);
    }


    public void SetRigidbody(Rigidbody rigid)
    {
        rb = rigid;
    }

    public void HandleCameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * 7f;
        float mouseY = Input.GetAxis("Mouse Y") * 7f;

        turn.x += mouseX;
        turn.y -= mouseY;
        turn.y = Mathf.Clamp(turn.y, -35f, 35f);

        transform.localRotation = Quaternion.Euler(0f, turn.x, 0f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(turn.y, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsOnGroundLayer(collision.gameObject))
        {
            isGrounded = true;
            jumps = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsOnGroundLayer(collision.gameObject))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (IsOnGroundLayer(collision.gameObject))
        {
            isGrounded = false;
        }
    }

    private bool IsOnGroundLayer(GameObject obj)
    {
        return (groundLayer.value & (1 << obj.layer)) > 0;
    }


}