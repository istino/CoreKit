namespace CoreKit.Sample
{
    // 추상화에 의존시키기 위한 인터페이스 — AudioService는 구현체가 아니라 이걸 본다.
    public interface IGameClock
    {
        float TotalTime { get; }
        int FrameCount { get; }
    }
}
