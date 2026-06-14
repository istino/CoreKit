using System;
using System.Collections.Generic;

namespace MC.CoreKit
{
    // [관심사 3: 통신 토폴로지]
    // 부수적/브로드캐스트(fire-and-forget) 통신용. 필수·구조적 의존은 생성자 주입을 쓴다.
    // static 전역 허브가 아니라 '주입받는 서비스' — 과용/암묵적 결합을 구조로 억제한다.
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : struct;
        void Unsubscribe<T>(Action<T> handler) where T : struct;
        void Publish<T>(T evt) where T : struct;
    }

    public sealed class EventBus : IEventBus, IDisposable
    {
        readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            var key = typeof(T);
            if (!_subscribers.TryGetValue(key, out var list))
            {
                list = new List<Delegate>();
                _subscribers[key] = list;
            }
            list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            if (_subscribers.TryGetValue(typeof(T), out var list))
                list.Remove(handler);
        }

        public void Publish<T>(T evt) where T : struct
        {
            if (!_subscribers.TryGetValue(typeof(T), out var list)) return;

            // 역순 순회 — 핸들러 내부에서 Unsubscribe해도 안전 (Hearth 교훈).
            for (int i = list.Count - 1; i >= 0; i--)
                ((Action<T>)list[i])?.Invoke(evt);
        }

        public void Dispose() => _subscribers.Clear();   // 커널이 OnDestroy에서 호출
    }
}
