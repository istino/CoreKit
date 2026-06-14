---
type: DECISION
date: 2026-06-14
system: UI
---

# CoreKit UI 프레임워크 아키텍처

**배경:** UI의 호출/사용 플로우를 엔진화하고 싶다. 게임마다 UI *구조*는 다르지만(Rolice=SO 레지스트리 등록 / Hearth=씬 배치 + Awake 자가등록 / Resources 추출 등), "어떻게 호출하고 쓰느냐"는 공통화 가능하다. 반복 증거: Rolice `RcUIManager`, Hearth `UIManager` — 둘 다 **`UIManager.Instance` 싱글톤**을 쓰고, Rolice는 패널마다 SO 수동 등록이 관리 통증. CoreKit이 서비스를 싱글톤→부팅 단일 합성지점으로 풀어낸 것처럼, UI도 같은 도약을 적용한다.

**원칙 정합성:** UI는 코어(`com.mc.corekit`)를 건드리지 않고 **별도 옵션 asmdef**로 분리한다. 코어 의존성 0 유지 → "통증 없는 선제 추상화 금지" 원칙과 충돌 없음(쓰는 프로젝트만 가져감).

---

## 결정

### 1. 싱글톤 제거 → `IUIService` 부팅 서비스
`UIManager.Instance` 전역 싱글톤 대신 `IUIService`를 `UIInstaller`가 등록하고 **생성자 주입**으로 사용. `IInitializable`로 캔버스 레이어 세팅, `IDisposable`로 정리 → 기존 `ServiceKernel`이 펌핑, 새 Mono 진입점 0.

### 2. 소스/시스템 분리 → `IUIPanelProvider` 주입
"어디서 가져오냐"(Provider) ⟂ "어떻게 쓰냐"(UIService) 분리. Provider 교체로 프로젝트별 대응:
- `ResourcesProvider` — 기본 동봉. 컨벤션(`typeof(T).Name` → 경로). **등록 0건.**
- `AddressablesProvider` — 별도 asmdef 격리. 옵션.
- `SceneProvider` — 씬 배치 참조(Hearth식).

레이어는 SO 대신 `[UIPanel(Layer=...)]` 어트리뷰트.

### 3. 타입 키 호출
`OpenAsync<ShopPanel>()` / `OpenAsync<ResultPanel, ResultData>(data)`. `typeof(T)`가 식별자 → 오타=컴파일에러, 리네임=IDE추적, 데이터 타입=컴파일 강제. 단점(호출부가 패널 타입에 컴파일 의존)은 `UIRouter`(이벤트→패널 매핑)로 **모듈 경계에서만** 우회.

### 4. 패널 = 멍청한 뷰
MonoBehaviour는 리플렉션 없이 생성자 주입 불가 → 패널엔 의존성 주입 안 함. `Open<T,TData>(data)`로 데이터 푸시. 똑똑한 로직은 주입받는 서비스/라우터 쪽에. (제약이 아니라 올바른 방향 강제)

### 5. 비동기 우선 (UniTask)
`Resources`(동기)→`Addressables`(비동기) 전환의 진짜 비용은 **비동기 전염**. 처음부터 `IUIPanelProvider.GetAsync<T>()`/`Release`, `IUIService.OpenAsync<T>()`를 UniTask로 설계. 동기→비동기 후행 전환은 지옥이라 처음부터 비동기.

### 6. 내로우 주입(최소권한) → `UIHandle<T,TData>`, 컨테이너 확장 없음
`IUIService` 통째 주입 = 아무 패널이나 여는 과한 권한 + service-locator 냄새. 대신 `IUIService`를 감싸는 얇은 struct `UIHandle<T,TData>`를 주입 → 소비자는 선언한 패널만 접근, 의존성이 생성자에 드러남, 테스트 용이.

> **핵심:** CoreKit은 순수 DI(수동 와이어링, 리플렉션 자동주입 없음)다. 따라서 오픈제네릭 컨테이너 해석이 **불필요**. 핸들은 인스톨러에서 인라인 `new UIHandle<T,TData>(ui)`로 생성. 컨테이너는 손대지 않음.

---

## 어셈블리 계층

| 패키지 | 내용 | 의존성 |
|--------|------|--------|
| `com.mc.corekit` | 코어(Kernel/Container/Lifecycle) | **0** |
| `com.mc.corekit.ui` | `IUIService`, `UIPanel`, `IUIPanelProvider`, `ResourcesProvider`, `CanvasLayer` | UniTask |
| `com.mc.corekit.ui.addressables` | `AddressablesProvider`만 | UniTask + Addressables (옵션) |

## 선행 작업
- UniTask 설치
- Addressables 설치

## 검토했던 대안
- **`Func<TData,UniTask>` 직접 주입:** 가장 느슨하지만 패널마다 등록 부담 최악 → 모듈 경계 특수지점에만 한정.
- **컨테이너 오픈제네릭 자동해석:** 자동주입 컨테이너에서만 의미. CoreKit은 수동 DI라 불필요 + 무리플렉션 철학 위반 → **드롭** (코드 검토 단계에서 전제 오류 발견하여 철회).
- **string/enum 키:** 런타임 에러 위험 → 기각, 타입 키 채택.
- **UIManager 싱글톤(Rolice/Hearth식):** 전역 접근점 = CoreKit이 버린 그 패턴 → 기각, 서비스 주입 채택.

## 관련 결정
- `2026-06-11_project-start.md` — 핵심 아키텍처 7건(싱글톤 거부 / 순수DI / 생성자주입)
- `2026-06-11_eventbus-into-core.md` — UIRouter가 의존하는 주입형 EventBus
