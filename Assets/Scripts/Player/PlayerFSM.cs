using UnityEngine;
public class PlayerFSM : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float airControlForce = 5f;
    public int maxJumps = 2;
    public Transform cameraPivot;
    public LayerMask groundLayer;

    public Vector2 turn;

    public Rigidbody rb;
    public bool isGrounded;
    public int jumps;

    public Terrain terrain;
    public float maxSlopeAngle;
    public float currentSlopeAngle;



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

    private void Start()
    {
        SwitchState(new PlayerIdleState());
    }

    private void Update()
    {
        currentState?.Update();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void SwitchState(IPlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentStateName = newState.GetType().Name;
        currentState.Enter(this);
    }

    public void SetRigidbody(Rigidbody rigid)
    {
        rb = rigid;
    }


}