# 반복 인프라 매트릭스 — CoreKit 추출 근거

> 4개 프로젝트(Hearth / steampack / Rolice / MelodyDrop) Assets 코드 스캔(2026-06-11).
> CoreKit이 "발명"이 아니라 "반복된 통증의 증류"임을 증명하는 1차 증거.

## 매트릭스

| 인프라 | Hearth | steampack | Rolice | MelodyDrop | 반복 |
|--------|:---:|:---:|:---:|:---:|:---:|
| 진입점/부트스트랩 | AppWiring | GameManager | Bootstrap ×4 (App/Init/Game/Lobby) | GameManager | **4/4** |
| 서비스 접근 | Services(locator) | Singleton×N | 정적레지스트리+Singleton+Context 3종 혼재 | Singleton×N | **4/4** |
| Singleton 베이스 | (탈피) | Singleton/DontDestroy | RcSingleton/Mono | Singleton/Mono | 3/4 |
| 라이프사이클/순서 제어 | Bootstrapper 수동 | — | 부트스트랩 분리 | ManualLifeCycleManager | 2-3 |
| UI/팝업 스택 | Floating/UIManager | UIManager+Popup | RcUIManager+Dialog | UIManager | **4/4** |
| Save/Data | SO 스토어 | DataManager | IRcSaveSystem+Json+DataTable | DataManager | **4/4** |
| 씬 로딩/전환 | (WindowManager) | SceneLoadManager | RcSceneLoader | SceneChannel+Fade | 3-4 |
| 사운드 | SoundPlayer | Sound+Music | Sound/Bgm/Sfx/Mixer | — | 3/4 |
| 풀링 | — | ObjectPool+IndicatorPool | — | — | **1/4** |

## 결정적 발견

**1. 서비스 접근을 4/4 전부 재발명. Rolice는 혼자 3가지 방식 혼재.**
`RcAppBootstrap.Initialize()` 안에 `RcBackendServices.Register(...)`(정적 레지스트리) +
`RcProgressManager.Instance`(싱글톤) + `RcGameContext`(정적 상태)가 공존.
= "차악을 매번 다르게 짜는" 물증. 또한 `[RuntimeInitializeOnLoadMethod]` + Core 프리팹 로드 +
DontDestroyOnLoad = 가장 흔한 "Core 프리팹 진입점" 패턴.

**2. MelodyDrop은 이 프레임워크를 시도하다 드롭한 프로젝트.**
`Assets/00_Engine/System/`에 `ManualLifeCycleManager`(Queue로 ManualAwake/Start 수동 펌프) 존재.
= CoreKit `ServiceKernel` 라이프사이클 펌프의 원시 버전. Unity Awake/Start 순서를 불신해
직접 통제하려 했으나 Singleton으로 구현해 그 짐을 떠안음.

## 절제 판정 (무엇을 코어에 넣지 않는가)

- **풀링 1/4** → 반복 부족, 코어 제외.
- **UI/Save/씬/사운드** → 4/4~3/4로 반복되나 *도메인 서비스*. 코어(해결/라이프사이클/통신) 위에
  얹히는 것이지 코어가 아님. 추출하더라도 별도 패키지 또는 샘플 서비스로.
- **코어 3기둥(해결/라이프사이클/통신)** → 4/4 전부에서 반복 확인. CoreKit이 정조준한 지점.

## Before / After (서비스 접근)

```csharp
// BEFORE — steampack/Rolice/MelodyDrop 공통
public class SoundManager : Singleton<SoundManager> { ... }
SoundManager.Instance.Play(...);   // 전역 접근, 숨은 의존, 테스트 난해

// AFTER — CoreKit
public sealed class SoundService : ISoundService { ... }            // 순수 로직
container.Register<ISoundService>(new SoundService(...));            // Installer가 등록
public Foo(ISoundService sound) { ... }                             // 생성자 주입, 의존 가시화
```
