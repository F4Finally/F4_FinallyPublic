public interface IState
{
    public void Enter();
    public void Exit();
    public void Update();
    public void PhysicsUpdate();

}
public abstract class StateMachine
{
    protected IState curState;

    public void ChangeState(IState state)
    {
        // 상태가 있다면 먼저 나가고
        curState?.Exit();
        // 상태 넣기
        curState = state;
        // 실행
        curState.Enter();
    }

    public void Update()
    {
        curState?.Update();
    }

    public void PhysicsUpdate()
    {
        curState?.PhysicsUpdate();
    }
}
