namespace MC.CoreKit
{
    // [관심사 2: 라이프사이클]
    // 서비스가 '구현하기만 하면' 커널이 알아서 호출해준다. Mono 상속 불필요.

    // 모든 서비스가 등록된 뒤 1회 호출. 부작용 초기화(상태 로드, 구독)는 여기서.
    public interface IInitializable
    {
        void Initialize();
    }

    // 커널이 매 프레임 펌프. Mono가 아니어도 Update를 받게 해주는 핵심 인터페이스.
    public interface ITickable
    {
        void Tick(float deltaTime);
    }

    // 해제는 System.IDisposable을 그대로 사용 — 커널이 OnDestroy에서 역순 Dispose 호출.
}
