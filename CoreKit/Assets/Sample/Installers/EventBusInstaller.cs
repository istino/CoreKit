using MC.CoreKit;

namespace CoreKit.Sample
{
    // 통신 인프라 등록 — 커널 리스트 '맨 위'에 둔다(여러 서비스가 IEventBus를 의존하므로).
    public sealed class EventBusInstaller : MonoInstaller
    {
        public override void Install(ServiceContainer container)
        {
            container.Register<IEventBus>(new EventBus());
        }
    }
}
