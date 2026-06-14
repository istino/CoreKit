using MC.CoreKit;

namespace CoreKit.Sample
{
    public sealed class TimerInstaller : MonoInstaller
    {
        public override void Install(ServiceContainer container)
        {
            container.Register<ITimerService>(new TimerService());
        }
    }
}
