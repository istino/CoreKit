using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace CoreKit.Sample
{
    // [EXPERIMENTAL — Sample 스파이크] 채널 = 이름 붙은 씬 레이어 슬롯.
    // 의도적으로 Packages(축복된 코어)가 아니라 Sample에 산다. 검증 대상 = 메커니즘(슬롯/스왑)뿐.
    // 정책(fade/progress/addressable/memory)은 없음 — 어차피 게임별이라 이식 안 됨(미추출 결정 참조).
    // 프로덕션이면 string 대신 SceneReference(GUID)/Addressables로 갈 자리.
    public enum SceneChannel { Core, Content, UI }

    public interface ISceneChannelService
    {
        void Load(SceneChannel channel, string sceneName, Action onComplete = null);
        void Unload(SceneChannel channel, Action onComplete = null);
        bool IsLoaded(SceneChannel channel);
    }

    // SingletonMono(원본 패턴) 아님 — 그냥 등록되는 서비스. 콜백 기반이라 UniTask/코루틴 의존 없음.
    public sealed class SceneChannelService : ISceneChannelService
    {
        readonly Dictionary<SceneChannel, string> _loaded = new();

        public bool IsLoaded(SceneChannel channel) => _loaded.ContainsKey(channel);

        public void Load(SceneChannel channel, string sceneName, Action onComplete = null)
        {
            if (_loaded.TryGetValue(channel, out var existing))
            {
                if (existing == sceneName) { onComplete?.Invoke(); return; }   // 이미 같은 씬

                // 같은 채널의 기존 씬만 언로드 → 다른 채널은 절대 안 건드림 (= 채널 독립성)
                var unloadOp = SceneManager.UnloadSceneAsync(existing);
                _loaded.Remove(channel);
                if (unloadOp != null)
                {
                    unloadOp.completed += _ => LoadInternal(channel, sceneName, onComplete);
                    return;
                }
            }
            LoadInternal(channel, sceneName, onComplete);
        }

        public void Unload(SceneChannel channel, Action onComplete = null)
        {
            if (!_loaded.TryGetValue(channel, out var sceneName)) { onComplete?.Invoke(); return; }

            var op = SceneManager.UnloadSceneAsync(sceneName);
            _loaded.Remove(channel);
            if (op != null) op.completed += _ => onComplete?.Invoke();
            else onComplete?.Invoke();
        }

        void LoadInternal(SceneChannel channel, string sceneName, Action onComplete)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.completed += _ =>
            {
                _loaded[channel] = sceneName;
                onComplete?.Invoke();
            };
        }
    }
}
