namespace CoreKit.Sample
{
    // 부수적 브로드캐스트 — 매 1초 경과 알림. 구독자는 발행자(Clock)를 직접 참조하지 않는다.
    // struct = 힙 할당 없음(GC 압력 최소).
    public struct SecondElapsedEvent
    {
        public int Second;
    }
}
