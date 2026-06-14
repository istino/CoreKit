# CoreKit

경량 서비스 / 부트스트랩 프레임워크. **순수 C# 로직**과 **Mono 경계**를 분리하고,
**모듈러 Installer + 라이프사이클 커널**로 진입점·서비스 제공을 구조화한다.

## 푸는 세 가지 관심사

| 관심사 | 질문 | CoreKit의 해법 |
|--------|------|----------------|
| 의존성 해결 | 내 의존성 누가 주냐 | `ServiceContainer` (Installer에서만 사용) |
| 라이프사이클 | 초기화/Tick/해제 순서 | `ServiceKernel`이 `IInitializable`/`ITickable`/`IDisposable` 펌프 |
| 통신 토폴로지 | 시스템끼리 어떻게 말하냐 | 필수 의존 → 생성자 주입 / 부수·브로드캐스트 → `IEventBus`(주입형) |

## 핵심 원칙

- **로직은 순수 C#** — `MonoBehaviour`가 아니어도 커널이 `Tick`을 펌프한다.
- **Mono는 경계에서만** — 인스펙터 툴링/씬 참조가 필요한 곳(=Installer)에서만 Unity가 등장한다.
- **순서는 보인다** — 커널 인스펙터의 Installer 리스트 순서가 곧 실행 순서. (주석으로 관리 ❌)
- **추상화는 통증에 매핑** — 안 쓰는 추상화는 넣지 않는다. (`IEventBus`는 4개 프로젝트 반복 증거 확인 후 추가. config 추상화·토폴로지 정렬은 아직 보류.)

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

- **부트 씬 강제** (`BootSceneEnforcer`) — 어느 씬에서 Play를 눌러도 Build Settings 인덱스 0 씬부터
  시작. `Tools/CoreKit/Force Boot Scene On Play`로 on/off. "Core 씬에서만 시작" 제약 제거.
