using System.Collections.Generic;
using MC.CoreKit;

namespace CoreKit.Sample
{
    public interface ITimerService
    {
        // 타이머를 '소유'하는 팩토리 — 호출자는 전역 레지스트리를 알 필요가 없다.
        CountdownTimer Countdown(float duration, bool autoStart = true);
    }

    // 타이머들을 소유하고 매 프레임 Tick. SingletonMono(원본)가 아니라 ITickable → 커널이 구동.
    // 완료된 타이머는 자동 회수(GC). 'CoreKit 위에 얹은 첫 실제 도메인 서비스'.
    public sealed class TimerService : ITimerService, ITickable
    {
        readonly List<Timer> _timers = new();
        readonly List<Timer> _buffer = new();   // 순회 중 추가/제거 안전용

        public CountdownTimer Countdown(float duration, bool autoStart = true)
        {
            var timer = new CountdownTimer(duration);
            _timers.Add(timer);
            if (autoStart) timer.Start();
            return timer;
        }

        public void Tick(float deltaTime)
        {
            if (_timers.Count == 0) return;

            _buffer.Clear();
            _buffer.AddRange(_timers);

            foreach (var timer in _buffer)
            {
                timer.Tick(deltaTime);
                if (timer.IsFinished)
                    _timers.Remove(timer);   // 완료분 자동 회수
            }
        }
    }
}
