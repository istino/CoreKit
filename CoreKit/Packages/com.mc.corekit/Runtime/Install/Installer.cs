using UnityEngine;

namespace MC.CoreKit
{
    // 한 도메인의 서비스 등록/조립을 책임지는 모듈.
    // Hearth의 '갓 와이어링'을 도메인 단위로 쪼갠 것 — 시스템 추가 = 새 Installer 1개.
    public interface IInstaller
    {
        void Install(ServiceContainer container);
    }

    // 인스펙터 직렬화 / 씬 참조가 필요한 Installer. (Authoring 레이어)
    // 서비스 자체는 순수 C#일 수 있고, 이 클래스는 그걸 '조립'만 한다 — Unity가 로직에 안 샌다.
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        public abstract void Install(ServiceContainer container);
    }
}
