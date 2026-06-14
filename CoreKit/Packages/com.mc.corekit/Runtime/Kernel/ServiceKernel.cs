using System;
using System.Collections.Generic;
using UnityEngine;

namespace MC.CoreKit
{
    // 프레임워크의 '유일한' Mono 진입점.
    // Install → Initialize → Tick → Dispose 라이프사이클을 모든 서비스에 가로질러 펌프한다.
    // (Hearth의 AppWiring + AppBootstrapper가 하던 일을, 도메인 비종속적으로 일반화한 것.)
    [DefaultExecutionOrder(-1000)]
    public sealed class ServiceKernel : MonoBehaviour
    {
        [Tooltip("실행 순서 = 이 리스트 순서. 의존 '대상'이 되는 Installer를 위쪽에 둔다.\n" +
                 "주석으로 순서를 관리하던 Hearth와 달리, 순서가 인스펙터에 '보인다'.")]
        [SerializeField] List<MonoInstaller> _installers = new();

        ServiceContainer _container;
        readonly List<ITickable> _tickables = new();

        public IServiceContainer Container => _container;

        void Awake()
        {
            _container = new ServiceContainer();

            // 1. Install — 도메인별 서비스 등록/조립 (리스트 순서대로).
            //    하위 Installer가 상위 Installer의 서비스를 Resolve하면, 순서가 곧 의존이다.
            foreach (var installer in _installers)
            {
                if (installer == null) continue;
                installer.Install(_container);
            }

            // 2. 라이프사이클 수집 (등록 순서 유지).
            foreach (var service in _container.All)
            {
                if (service is ITickable t)
                    _tickables.Add(t);
            }

            // 3. Initialize — 모든 서비스가 존재하는 시점에서 부작용 초기화.
            foreach (var service in _container.All)
            {
                if (service is IInitializable init)
                    init.Initialize();
            }
        }

        void Update()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < _tickables.Count; i++)
                _tickables[i].Tick(dt);
        }

        void OnDestroy()
        {
            if (_container == null) return;

            // 등록 역순으로 해제 — 의존 '대상'이 더 나중에 죽도록.
            var all = _container.All;
            for (int i = all.Count - 1; i >= 0; i--)
                (all[i] as IDisposable)?.Dispose();
        }
    }
}
