using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    private PlayerFSM player;

    public void Enter(PlayerFSM player)
    {
        this.player = player;
    }

    public void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            player.SwitchState(player.moveState);
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.jumps < player.maxJumps)
        {
            player.SwitchState(player.jumpState);
        }

    } 
    
    public void FixedUpdate() { player.HandleCameraMovement(); }

    public void Exit() { }
}