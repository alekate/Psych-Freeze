public interface IPlayerState
{
    void Enter(PlayerFSM player);
    void Update();
    void FixedUpdate();
    void Exit();
}