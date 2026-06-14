using MC.CoreKit;
using UnityEngine;

namespace CoreKit.Sample
{
    // [EXPERIMENTAL] 채널 스파이크 데모.
    // 부팅 시 Content/UI 채널을 additive 로드 → 3초 후 Content만 언로드해
    // "한 채널을 건드려도 다른 채널(UI)은 멀쩡하다"는 채널 독립성을 증명.
    // (Core는 부트 씬이라 여기서 로드하지 않음 — 커널이 거기 산다.)
    public sealed class ChannelBootService : IInitializable
    {
        readonly ISceneChannelService _scenes;
        readonly ITimerService        _timers;

        public ChannelBootService(ISceneChannelService scenes, ITimerService timers)
        {
            _scenes = scenes;
            _timers = timers;
        }

        public void Initialize()
        {
            _scenes.Load(SceneChannel.Content, "Content", () => Debug.Log("[Channel] Content 로드 (Content 채널)"));
            _scenes.Load(SceneChannel.UI,      "UI",      () => Debug.Log("[Channel] UI 로드 (UI 채널)"));

            // 3초 후 Content 채널만 언로드 → UI는 그대로여야 함 (Timer 서비스 dogfood)
            _timers.Countdown(3f).OnComplete += () =>
                _scenes.Unload(SceneChannel.Content, () =>
                    Debug.Log($"[Channel] Content 언로드 완료. UI 여전히 로드됨? {_scenes.IsLoaded(SceneChannel.UI)}"));
        }
    }
}
