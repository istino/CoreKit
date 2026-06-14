using System;
using System.Collections.Generic;

namespace MC.CoreKit
{
    // [관심사 1: 의존성 해결]
    // 타입 → 인스턴스 등록/조회. 컴포지션 루트(Installer)에서만 쓰는 '조립 도구'이지,
    // 게임 코드 전역에서 꺼내 쓰는 서비스 로케이터가 아니다.
    public interface IServiceContainer
    {
        T Resolve<T>();
        bool TryResolve<T>(out T service);
    }

    public sealed class ServiceContainer : IServiceContainer
    {
        readonly Dictionary<Type, object> _byType = new();
        readonly List<object> _ordered = new();   // 등록 순서 보존 — 라이프사이클 순서의 기준

        // 인터페이스 타입으로 등록 → 꺼낼 때 캐스팅 불필요(원하는 타입이 바로 나옴).
        public ServiceContainer Register<T>(T service)
        {
            var key = typeof(T);
            if (_byType.ContainsKey(key))
                throw new InvalidOperationException($"[CoreKit] 이미 등록된 서비스: {key.Name}");

            _byType[key] = service;
            _ordered.Add(service);
            return this;   // 체이닝 등록 가능
        }

        public T Resolve<T>()
        {
            if (_byType.TryGetValue(typeof(T), out var s))
                return (T)s;

            throw new InvalidOperationException(
                $"[CoreKit] 등록되지 않은 서비스: {typeof(T).Name} — Installer 등록 여부 / 커널 리스트 순서를 확인하세요.");
        }

        public bool TryResolve<T>(out T service)
        {
            if (_byType.TryGetValue(typeof(T), out var s))
            {
                service = (T)s;
                return true;
            }
            service = default;
            return false;
        }

        // 커널이 라이프사이클 인터페이스를 수집할 때 사용 (등록 순서 유지).
        public IReadOnlyList<object> All => _ordered;
    }
}
