using UnityEngine;

public class PlayerAirMoveState : IPlayerState
{
    private PlayerFSM player;

    public void Enter(PlayerFSM player)
    {
        this.player = player;
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
        ApplyAirControl();
        CheckGrounded();
    }

    public void Exit() { }

    private void ApplyAirControl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = player.cameraPivot.forward * v + player.cameraPivot.right * h;
        inputDir.y = 0f;
        inputDir.Normalize();

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Vector3 airForce = inputDir * player.airControlForce;
            player.rb.AddForce(airForce, ForceMode.Acceleration);
        }
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