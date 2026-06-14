---
type: DECISION
date: 2026-06-11
---

# 에디터 부트 씬 강제 — 코어 추출 (채택)

**문제:** 멀티 씬 인프라의 고질병 — 디자이너가 Core(부트) 씬이 아닌 곳에서 Play를 누르면
인프라가 부팅 안 돼 다 깨짐. "Core 씬에서만 시작해야 한다"는 암묵 제약이 매번 마찰.

**증거 (2/4, 추출 바 통과):**
- Rolice `RcEditorBootstrapper` — `playModeStateChanged`로 InitScene 강제 + 수동 save/restore. 정상 동작.
- MelodyDrop `Core.cs` — `[RuntimeInitializeOnLoadMethod]`에서 `SceneManager.LoadScene("Core", Additive)`.
  **LoadScene이 지연 로드라 시작 씬 오브젝트 Awake가 Core.Awake보다 먼저 → `Core.GetService` null → 크래시.**
  `ManualLifeCycleManager`(BindAwake/RunAwake)는 그 반창고였으나 "모두가 Awake 대신 ManualAwake 써야"만
  성립해 깨지기 쉬움.

**왜 추출 OK인가 (씬로더·채널과 달리):**
- 메커니즘이지 정책 아님 (에디터 강제는 게임 불문).
- CoreKit #1 정체성(진입점) 그 자체.
- footgun — 틀리기 쉬워(MelodyDrop 증명) 올바른 패턴 박제 가치 높음.

**결정:** `MC.CoreKit.Editor`(에디터 asmdef) + `BootSceneEnforcer` 추가.
- Unity 네이티브 `EditorSceneManager.playModeStartScene` 사용 → 종료 시 원래 씬 복구는 Unity 자동.
  (Rolice 수동 save/restore를 네이티브로 일반화·간소화.)
- 부트 씬 = Build Settings 첫 활성 씬(인덱스 0). 메뉴 `Tools/CoreKit/Force Boot Scene On Play`로 on/off.

**핵심 교훈 (런타임 보장은 미추출, 패턴만 문서화):**
- 런타임에 인프라를 시작 씬과 무관하게 보장하려면 → 훅에서 **동기 `Instantiate(prefab)`** 를 써라.
  **`LoadScene`(지연)은 레이스를 만든다(MelodyDrop 버그).** 단 "Core를 프리팹으로 두냐 씬으로 두냐"는
  게임 모양이라 프레임워크 코드로 추출 안 함 — 패턴으로만 남김.
- 유저 플로우(auth/sync/navigate, Rolice `RcInitBootstrap`)는 순수 정책 → 게임에 남김.

**의의:** 코어 3기둥(해결/라이프사이클/통신) *밖*에서 추출 바를 통과한 첫 항목.
3기둥은 그대로, 이건 진입점 기둥을 떠받치는 에디터 툴링으로 분류.
