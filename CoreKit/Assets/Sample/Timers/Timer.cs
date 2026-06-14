using System;

namespace CoreKit.Sample
{
    // 순수 C# 타이머 베이스. Unity/Mono/전역 로케이터 비의존 → 테스트에서 Tick 수동 호출 가능.
    //
    // [차용 + 재설계] git-amend의 ImprovedTimers 개념을 차용하되, 원본의 약점이던
    // Timer.Start() 안의 `Core.GetService<TimerManager>()`(전역 로케이터 직접 호출)을 제거.
    // 등록/구동 책임은 TimerService가 가지고, Timer는 자기 상태만 안다 (IoC).
    public abstract class Timer
    {
        protected float initialTime;

        public float CurrentTime { get; protected set; }
        public bool  IsRunning   { get; protected set; }
        public float Progress => initialTime <= 0f ? 1f : Math.Clamp(CurrentTime / initialTime, 0f, 1f);

        public event Action OnStart;
        public event Action OnStop;

        protected Timer(float duration) => initialTime = duration;

        public void Start()
        {
            CurrentTime = initialTime;
            IsRunning   = true;
            OnStart?.Invoke();
        }

        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
            OnStop?.Invoke();
        }

        public void Pause()  => IsRunning = false;
        public void Resume() => IsRunning = true;
        public void Reset()  => CurrentTime = initialTime;

        public abstract void Tick(float deltaTime);
        public abstract bool IsFinished { get; }
    }
}
