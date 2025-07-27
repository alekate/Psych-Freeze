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
            player.SwitchState(player.idleState);
        }

        ApplyAirControl();
        player.HandleCameraMovement();
    }

    public void FixedUpdate() { }

    public void Exit() { }

    private void ApplyAirControl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = player.cameraPivot.forward * v + player.cameraPivot.right * h;
        inputDir.Normalize();

        if (inputDir.magnitude > 0.01f)
        {
            Vector3 airForce = inputDir * player.airControlForce;
            player.rb.AddForce(airForce, ForceMode.Acceleration);
        }
    }
}