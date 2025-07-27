using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    private PlayerFSM player;

    public void Enter(PlayerFSM player)
    {
        this.player = player;
        if (player.terrain == null)
            player.terrain = Terrain.activeTerrain;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.jumps < player.maxJumps)
        {
            player.SwitchState(player.jumpState);
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h == 0 && v == 0)
        {
            player.SwitchState(player.idleState);
        }

    }

    public void FixedUpdate() { player.HandleCameraMovement(); Move(); }

    public void Exit() { }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = (player.cameraPivot.forward * v + player.cameraPivot.right * h);
        dir.y = 0;
        dir.Normalize();

        if (dir.sqrMagnitude > 0.01f && CanMove(dir))
        {
            Vector3 move = dir * player.moveSpeed * Time.fixedDeltaTime;
            player.rb.MovePosition(player.rb.position + move);
        }
    }

    private bool CanMove(Vector3 direction)
    {
        if (player.terrain == null)
        {
            player.terrain = Terrain.activeTerrain;

            if (player.terrain == null)
            {
                return true;
            }
        }

        Vector3 currentPos = player.rb.position;
        Vector3 futurePos = currentPos + direction * 5f;

        float currentHeight = player.terrain.SampleHeight(currentPos);
        float nextHeight = player.terrain.SampleHeight(futurePos);

        Vector3 normalizedPos = GetNormalizedTerrainPos(currentPos);
        Vector3 normal = player.terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.z);
        float slopeAngle = Vector3.Angle(normal, Vector3.up);

        player.currentSlopeAngle = slopeAngle;

        return slopeAngle <= player.maxSlopeAngle || nextHeight <= currentHeight;
    }


    private Vector3 GetNormalizedTerrainPos(Vector3 worldPos)
    {
        Vector3 terrainPos = player.terrain.transform.position;
        Vector3 size = player.terrain.terrainData.size;

        return new Vector3((worldPos.x - terrainPos.x) / size.x, 0, (worldPos.z - terrainPos.z) / size.z);
    }
}
