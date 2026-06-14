using MC.CoreKit;
using UnityEngine;

namespace CoreKit.Sample
{
    // 유니티 객체(AudioSource)를 '쓰지만' 로직은 순수 C#.
    // 인스펙터/씬 접근은 AudioInstaller(Authoring)가 담당하고, 여기엔 안 들어온다.
    public sealed class AudioService : IInitializable, ITickable
    {
        readonly AudioConfig _config;
        readonly AudioSource _source;
        readonly IGameClock  _clock;   // 의존 — 생성자에 명시 → 순서가 구조에서 파생된다.

        float _elapsed;
        bool  _fading;

        public AudioService(AudioConfig config, AudioSource source, IGameClock clock)
        {
            _config = config;
            _source = source;
            _clock  = clock;
        }

        public void Initialize()
        {
            if (_source != null) _source.volume = 0f;
            _fading = true;
            Debug.Log($"[Audio] 페이드인 시작 (clock t={_clock.TotalTime:F2})");
        }

        public void Tick(float deltaTime)
        {
            if (!_fading) return;

            _elapsed += deltaTime;
            float k = _config.fadeInSeconds <= 0f
                ? 1f
                : Mathf.Clamp01(_elapsed / _config.fadeInSeconds);

            if (_source != null)
                _source.volume = k * _config.masterVolume;

            if (k >= 1f)
            {
                _fading = false;
                Debug.Log($"[Audio] 페이드인 완료 (clock t={_clock.TotalTime:F2})");
            }
        }
    }
}
