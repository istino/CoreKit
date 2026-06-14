using MC.CoreKit;
using UnityEngine;   // Debug.Log(데모 가시성)용 — 로직 자체는 Unity 비의존

namespace CoreKit.Sample
{
    // 순수 C# 서비스. MonoBehaviour가 아닌데도 커널 덕분에 매 프레임 Tick을 받는다.
    // → "Update가 필요한 서비스조차 Mono일 필요 없다"를 증명.
    // 또한 매초 SecondElapsedEvent를 버스로 발행(브로드캐스트) → 구독자(HUD)와 디커플링.
    public sealed class GameClockService : IGameClock, IInitializable, ITickable
    {
        readonly IEventBus _bus;
        int _lastWholeSecond;

        public float TotalTime { get; private set; }
        public int   FrameCount { get; private set; }

        public GameClockService(IEventBus bus)
        {
            _bus = bus;
        }

        public void Initialize()
        {
            Debug.Log("[Clock] Initialize — Mono 아님. 커널이 Tick을 펌프한다.");
        }

        public void Tick(float deltaTime)
        {
            TotalTime += deltaTime;
            FrameCount++;

            int whole = (int)TotalTime;
            if (whole > _lastWholeSecond)
            {
                _lastWholeSecond = whole;
                _bus.Publish(new SecondElapsedEvent { Second = whole });
            }
        }
    }
}
