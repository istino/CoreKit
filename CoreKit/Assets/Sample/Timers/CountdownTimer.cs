using System;

namespace CoreKit.Sample
{
    // 지정 시간 후 1회 완료되는 카운트다운. 완료 통보 = OnComplete '직접 콜백'.
    // (요청자 본인이 원하는 결과이므로 브로드캐스트(EventBus)가 아니라 직접 콜백이 맞다.)
    public sealed class CountdownTimer : Timer
    {
        public event Action OnComplete;

        public CountdownTimer(float duration) : base(duration) { }

        public override bool IsFinished => CurrentTime <= 0f;

        public override void Tick(float deltaTime)
        {
            if (!IsRunning || CurrentTime <= 0f) return;

            CurrentTime -= deltaTime;
            if (CurrentTime <= 0f)
            {
                CurrentTime = 0f;
                IsRunning   = false;
                OnComplete?.Invoke();
            }
        }
    }
}
