---
type: DECISION
date: 2026-06-11
---

# 씬 로더 — 코어/모듈 미추출 (검토 후 기각)

**검토 배경:** 씬 로딩/전환은 3-4/4 프로젝트에서 반복(`ref/repeated-infra-matrix.md`).
반복 횟수만 보면 Timer보다 강한 추출 후보처럼 보였다.

**3개 구현 비교:**
- MelodyDrop `SceneChannelManager` — SingletonMono, UniTask, Additive 채널 + Addressable, EventBus 진행도
- steampack `SceneLoadManager` — 순수 C#, Coroutine, 중간 LoadingScene + 씬별 OnEnter/OnExit/OnLoadAssets 라이프사이클 + 팩토리
- Rolice `RcSceneLoader` — SingletonMono, Coroutine, 단일 씬 + DOTween Fade + 캔버스 숨김 + minimumLoadTime

**공통분모:** `LoadSceneAsync` → progress 폴링 → 0.9에서 `allowSceneActivation` 게이트. 약 10줄뿐.
나머지(채널/Addressable, LoadingScene/씬 라이프사이클, Fade/최소시간)는 전부 **게임별 정책이며 상호 이식 불가**.

**결정:** 코어에도, 옵션 모듈에도, 샘플에도 **추출하지 않는다.**

**근거 — 반복된 건 "코드"가 아니라 "필요"였다 (repeated need ≠ reusable code):**
- Timer: 로직(카운트다운)이 게임 불문 → 이식됨 → 추출 정답.
- SceneLoader: 로직이 전부 게임별 정책 → 이식 안 됨. 추출 시 (a) 10줄짜리 과소 추상화이거나
  (b) 한 게임 정책을 박아 다음 게임에 안 맞음. 둘 다 손해.
- 프로젝트 시작 결정 #5(씬은 코어 밖, 게임별 정책)와 일치. 사용자가 초기에 느낀
  "씬매니저는 게임마다 달라서 애매"의 정체가 바로 이것.

**대신 권장하는 사용 패턴:** 씬 전환이 필요한 *각 게임*에서 `ISceneLoader` 심(seam)을 정의하고
게임별 구현체를 Installer로 등록. CoreKit은 이에 대해 아무것도 제공하지 않는다(의도적).

**일반 원칙(추출 판정 기준 확정):** "반복 횟수"가 아니라 "로직이 게임 불문으로 이식되는가"가
추출 여부를 가른다. 정책(policy)은 추출하지 않고 심(seam)만 남긴다.
