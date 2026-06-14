---
type: DECISION
date: 2026-06-11
supersedes: "2026-06-11_project-start.md #7 (EventBus 의도적 미포함)"
---

# EventBus를 코어에 추가 (보류 → 채택)

**배경:** 프로젝트 시작 시 EventBus를 의도적으로 뺐다(결정 #7) — "진짜 브로드캐스트 필요가
생기면 그때 추가한다". 통증 없는 선제 추상화를 피하기 위함.

**무엇이 바뀌었나:** 4개 프로젝트 스캔(`ref/repeated-infra-matrix.md`)에서
통신 인프라가 3/4 프로젝트(Hearth EventBus / steampack EventManager / MelodyDrop EventBus)에서
반복 확인됨. 보류 조건("증거가 생기면")이 충족됨.

**결정:** `IEventBus` + `EventBus`를 코어(`com.mc.corekit`)의 **세 번째 기둥(통신)**으로 추가.

**핵심 제약 (Hearth 교훈 반영):**
- **static 전역 허브 ❌ → 주입받는 서비스 ✅.** 컨테이너에 등록하고 생성자로 주입 →
  이벤트버스 과용/암묵적 결합을 구조로 억제.
- **목적으로 통신 수단을 가른다:** 필수·구조적 의존 = 생성자 직접 주입 / 부수·브로드캐스트 = EventBus.
- struct 이벤트 유지(GC 압력 최소), 역순 순회(핸들러 내 Unsubscribe 안전) — Hearth 설계 계승.

**검증 샘플:** `GameClockService`가 매초 `SecondElapsedEvent` 발행 → `HudService`가 구독.
둘은 서로 직접 참조 없음(= 디커플링). 반면 `AudioService`는 `IGameClock`을 직접 주입받음
(= 필수 의존). 한 샘플 안에서 "직접 주입 vs 버스"를 목적별로 대비.

**여전히 보류 중:** config 추상화(IConfigSource), 토폴로지 자동 정렬 — 아직 증거 없음.
