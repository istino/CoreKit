using System;
using MC.CoreKit;
using UnityEngine;

namespace CoreKit.Sample
{
    // 구독자 — Clock을 '직접 참조하지 않고' 이벤트버스로만 안다 (디커플링).
    // 예전이라면 Singleton.Instance를 잡아 구독했을 자리 → 이제 주입받은 서비스.
    // IDisposable 구현 → 커널이 OnDestroy 시 역순으로 Dispose 호출하며 구독 해제.
    public sealed class HudService : IInitializable, IDisposable
    {
        readonly IEventBus     _bus;
        readonly ITimerService _timers;

        public HudService(IEventBus bus, ITimerService timers)
        {
            _bus    = bus;
            _timers = timers;
        }

        public void Initialize()
        {
            _bus.Subscribe<SecondElapsedEvent>(OnSecond);   // 부수 통신 = 버스 구독

            // 직접 통신 = 타이머 콜백. 내가 만든 타이머의 결과는 나에게만 오면 되므로 버스가 아님.
            var countdown = _timers.Countdown(5f);
            countdown.OnComplete += () => Debug.Log("[HUD] 5초 카운트다운 완료! (타이머 직접 콜백)");
        }

        public void Dispose()
        {
            _bus.Unsubscribe<SecondElapsedEvent>(OnSecond);
        }

        void OnSecond(SecondElapsedEvent e)
        {
            Debug.Log($"[HUD] 가동 {e.Second}초 (Clock 직접 참조 없이 이벤트로 수신)");
        }
    }
}
