using MC.CoreKit;

namespace CoreKit.Sample
{
    // 순수 C# 서비스를 인터페이스로 등록. EventBus를 의존하므로 커널 리스트에서 그 아래에 둔다.
    public sealed class ClockInstaller : MonoInstaller
    {
        public override void Install(ServiceContainer container)
        {
            var bus = container.Resolve<IEventBus>();
            container.Register<IGameClock>(new GameClockService(bus));
        }
    }
}
