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
            player.SwitchState(player.airMoveState);
        }
    }

    public void Update()
    {
        if (player.isGrounded)
        {
            player.SwitchState(player.idleState);
        }

        if (!instantJump)
        {
            PerformJump();
            player.SwitchState(player.airMoveState);
        }
    }
    public void FixedUpdate() { player.HandleCameraMovement(); }

    public void Exit() { }

    private void PerformJump()
    {
        Vector3 inputDirection = player.cameraPivot.forward * Input.GetAxis("Vertical") +
                                 player.cameraPivot.right * Input.GetAxis("Horizontal");
        inputDirection.y = 0;
        inputDirection.Normalize();

        Vector3 currentHorizontalVelocity = player.rb.velocity;
        currentHorizontalVelocity.y = 0;

        Vector3 desiredDirection = inputDirection * player.moveSpeed;

        Vector3 combinedHorizontal = Vector3.Lerp(currentHorizontalVelocity, desiredDirection, 0.5f);

        Vector3 finalVelocity = combinedHorizontal;
        finalVelocity.y = 0f; 

        player.rb.velocity = finalVelocity;
        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);

        player.jumps++;
    }
}
