using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    private PlayerFSM player;

    public void Enter(PlayerFSM player)
    {
        this.player = player;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.jumps < player.maxJumps)
        {
            player.SwitchState(new PlayerJumpState(true));
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h == 0 && v == 0)
        {
            player.SwitchState(new PlayerIdleState());
        }
    }

    public void FixedUpdate()
    {
        Move();
        CheckGrounded();
        CameraMovement();
    }

    public void Exit() { }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = (player.cameraPivot.forward * v + player.cameraPivot.right * h);
        dir.y = 0;
        dir.Normalize();

        Vector3 move = dir * player.moveSpeed * Time.fixedDeltaTime;
        player.rb.MovePosition(player.rb.position + move);
    }

    private void CameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * 7f;
        float mouseY = Input.GetAxis("Mouse Y") * 7f;

        player.turn.x += mouseX;
        player.turn.y -= mouseY;
        player.turn.y = Mathf.Clamp(player.turn.y, -35f, 35f);

        player.transform.localRotation = Quaternion.Euler(0f, player.turn.x, 0f);
        if (player.cameraPivot != null)
            player.cameraPivot.localRotation = Quaternion.Euler(player.turn.y, 0f, 0f);

    }

    private void CheckGrounded()
    {
        bool wasGrounded = player.isGrounded;
        player.isGrounded = Physics.Raycast(player.transform.position, Vector3.down, 1.1f, player.groundLayer);
        if (player.isGrounded && !wasGrounded)
        {
            player.jumps = 0;
        }
    }
}