using MC.CoreKit;

namespace CoreKit.Sample
{
    public sealed class HudInstaller : MonoInstaller
    {
        public override void Install(ServiceContainer container)
        {
            var bus    = container.Resolve<IEventBus>();      // 부수 통신 = 버스 주입
            var timers = container.Resolve<ITimerService>();  // 타이머 서비스 주입
            container.Register(new HudService(bus, timers));
        }
    }
}
