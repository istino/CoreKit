using MC.CoreKit;
using UnityEngine;

namespace CoreKit.Sample
{
    // Authoring — 인스펙터에서 블루프린트(SO)와 씬 참조(AudioSource)를 받아
    // 순수 C# AudioService를 '조립'한다. 서비스 로직엔 Unity가 안 샌다.
    public sealed class AudioInstaller : MonoInstaller
    {
        [SerializeField] AudioConfig _config;
        [SerializeField] AudioSource _source;

        public override void Install(ServiceContainer container)
        {
            // 의존(IGameClock)을 여기서 Resolve → 커널 리스트에서 ClockInstaller가 위에 있어야 함.
            // 순서가 틀리면 명확한 예외가 난다(= 조용한 순서 버그 대신 즉시 발견).
            var clock = container.Resolve<IGameClock>();
            container.Register(new AudioService(_config, _source, clock));
        }
    }
}
