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
        // Dirección horizontal según la entrada del jugador
        Vector3 inputDirection = player.cameraPivot.forward * Input.GetAxis("Vertical") +
                                 player.cameraPivot.right * Input.GetAxis("Horizontal");
        inputDirection.y = 0;
        inputDirection.Normalize();

        // Calculamos nueva velocidad horizontal (respetando momentum, pero más controlado)
        Vector3 currentHorizontalVelocity = player.rb.velocity;
        currentHorizontalVelocity.y = 0;

        Vector3 desiredDirection = inputDirection * player.moveSpeed;

        // Combinamos la dirección deseada con el momentum actual (puede ajustarse)
        Vector3 combinedHorizontal = Vector3.Lerp(currentHorizontalVelocity, desiredDirection, 0.5f);

        // Mantenemos la velocidad horizontal y agregamos salto vertical
        Vector3 finalVelocity = combinedHorizontal;
        finalVelocity.y = 0f; // reseteamos solo el eje Y para aplicar el salto con fuerza limpia

        player.rb.velocity = finalVelocity;
        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);

        player.jumps++;
    }
}
