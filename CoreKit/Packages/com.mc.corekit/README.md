# CoreKit

경량 서비스 / 부트스트랩 프레임워크. 상태·흐름 서비스는 `MonoBehaviour`가 아닌 일반 객체로 두고,
Unity 의존은 커널·Installer·View 같은 경계에 모은다. **모듈러 Installer + 라이프사이클 커널**로
진입점과 서비스 제공을 정리한다.

## 푸는 세 가지 관심사

| 관심사 | 질문 | CoreKit의 해법 |
|--------|------|----------------|
| 의존성 해결 | 내 의존성 누가 주냐 | `ServiceContainer` (Installer에서만 사용) |
| 라이프사이클 | 초기화/Tick/해제 순서 | `ServiceKernel`이 `IInitializable`/`ITickable`/`IDisposable` 펌프 |
| 통신 토폴로지 | 시스템끼리 어떻게 말하냐 | 필수 의존 → 생성자 주입 / 부수·브로드캐스트 → `IEventBus`(주입형) |

## 핵심 원칙

- **서비스는 일반 객체로** — `MonoBehaviour`가 아니어도 커널이 `Tick`을 펌프한다.
  (단 `AudioService`처럼 Unity API를 직접 쓰는 서비스도 있다. Unity 비의존을 보장하지는 않는다.)
- **Mono는 경계에** — 커널·Installer·View에 두고, Unity 객체는 Installer에서 주입한다.
- **순서는 보인다** — 커널 인스펙터의 Installer 리스트 순서가 곧 실행 순서. (주석으로 관리 ❌)
- **추상화는 통증이 증명될 때만** — 같은 문제를 반복해서 겪은 뒤에만 넣는다.
  config 추상화·토폴로지 정렬은 아직 보류 중이다.

## 다른 프로젝트에서 가져다 쓰기

대상 프로젝트 `Packages/manifest.json` 에 한 줄:

```json
"com.mc.corekit": "file:../../PROJECT_CoreKit/CoreKit/Packages/com.mc.corekit"
```

(또는 폴더 복사 / Git URL — 셋 다 가능)

## 사용법

1. 씬에 빈 GameObject → `ServiceKernel` 추가
2. 도메인별 `MonoInstaller` 를 같은(또는 자식) GameObject에 추가
3. 커널의 `Installers` 리스트에 Installer들을 **의존 순서대로** 드래그
4. Play — 커널이 Install → Initialize → Tick 자동 진행

샘플은 `Assets/Sample/` 참고.

## 에디터 툴링

- **부트 씬 강제** (`BootSceneEnforcer`) — 어느 씬에서 Play를 눌러도 Build Settings의 첫 enabled 씬부터
  시작. `Tools/CoreKit/Force Boot Scene On Play`로 on/off. "Core 씬에서만 시작" 제약 제거.

## 상태

설계 실험 겸 기초 구현이다. 태그·릴리스·레지스트리 배포와 자동화 테스트는 없고,
Unity 프로젝트 안에 임베디드 UPM 패키지로 분리해 두는 형태로 쓴다.
UI 모듈(`com.mc.corekit.ui`)은 정적 와이어링까지 확인했고 런타임 스모크는 기록하지 않았다.
