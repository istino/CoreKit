using MC.CoreKit;

namespace CoreKit.Sample
{
    // [EXPERIMENTAL] 채널 도메인 등록 — 서비스 + 부트 데모를 함께(같은 도메인이므로).
    // TimerInstaller보다 아래에 둔다(부트 데모가 ITimerService를 의존).
    public sealed class SceneChannelInstaller : MonoInstaller
    {
        public override void Install(ServiceContainer container)
        {
            var scenes = new SceneChannelService();
            container.Register<ISceneChannelService>(scenes);

            var timers = container.Resolve<ITimerService>();
            container.Register(new ChannelBootService(scenes, timers));
        }
    }
}
