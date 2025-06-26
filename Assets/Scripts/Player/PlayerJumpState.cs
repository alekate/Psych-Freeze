using UnityEngine;

public class PlayerJumpState : IPlayerState
{
    private PlayerFSM player;
    private bool jumped;
    private bool instantJump;

    public PlayerJumpState(bool jumpNow = false) //Semi hardcodeo para que el salto no se retrase
    {
        instantJump = jumpNow;
    }

    public void Enter(PlayerFSM player)
    {
        this.player = player;
        jumped = false;

        if (instantJump)
        {
            Jump();
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
        if (!instantJump && !jumped)
        {
            Jump();
            jumped = true;
            player.SwitchState(new PlayerAirMoveState());
        }

        CheckGrounded();
    }

    public void Exit() { }

    private void Jump()
    {
        Vector3 vel = player.rb.velocity;
        vel.y = 0f;
        player.rb.velocity = vel;

        // aplicar impulso de salto
        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);
        player.jumps++;
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
