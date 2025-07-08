using UnityEngine;

public class PlayerJumpState : IPlayerState
{
    private PlayerFSM player;
    private readonly bool instantJump;

    public PlayerJumpState(bool jumpNow = false)
    {
        instantJump = jumpNow;
    }

    public void Enter(PlayerFSM player)
    {
        this.player = player;

        if (instantJump)
        {
            PerformJump();
            player.SwitchState(new PlayerAirMoveState());
        }
    }

    public void Update()
    {
        if (player.isGrounded)
        {
            player.SwitchState(new PlayerIdleState());
        }
    }

    public void FixedUpdate()
    {
        if (!instantJump)
        {
            PerformJump();
            player.SwitchState(new PlayerAirMoveState());
        }

        UpdateGroundedStatus();
    }

    public void Exit() { }

    private void PerformJump()
    {
        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);
        player.jumps++;
    }

    private void UpdateGroundedStatus()
    {
        bool wasGrounded = player.isGrounded;
        player.isGrounded = Physics.Raycast(player.transform.position, Vector3.down, 1.1f, player.groundLayer);

        if (player.isGrounded && !wasGrounded)
        {
            player.jumps = 0;
        }
    }
}
