# CoreKit

경량 서비스 / 부트스트랩 프레임워크 (게임 아님 — 다른 프로젝트가 뽑아 쓰는 라이브러리).
순수 C# 로직과 Mono 경계를 분리하고, 모듈러 Installer + 라이프사이클 커널로 진입점·서비스 제공을 구조화한다.

> 코드 위치: `CoreKit/Packages/com.mc.corekit/` (임베디드 UPM 패키지) · 샘플: `CoreKit/Assets/Sample/`

---

## 문서 읽는 순서

1. `INDEX.md` — 전체 문서 목록 한눈에
2. `specs/_SCOPE_*.md` — 구현 로드맵 / 진행 현황
3. `specs/_*.md` — 현재 ACTIVE 시스템 (언더스코어 prefix)
4. `decisions/` — 결정 맥락이 필요할 때
5. `ref/` — 레퍼런스 (필요할 때만)

## 커맨드

| 커맨드 | 용도 |
|--------|------|
| `/doc-scope` | 구현 로드맵 정의 |
| `/doc-new` | 새 시스템 스펙 문서 |
| `/doc-decision` | 결정 기록 |
| `/impl` | 구현 세션 시작 |
| `/bug` | 버그 수정 세션 시작 |

## 아키텍처 원칙

> 이 프레임워크의 존재 이유 = Hearth에서 겪은 통증(갓 와이어링 비대화 / 주석 순서 / 부트스트랩에 런타임 로직 혼입)의 해소.

- **금지 패턴**
  - 서비스 코드가 `ServiceContainer`를 참조하거나 `Resolve` 호출 (= 로케이터 안티패턴, 숨은 의존)
  - 단일 갓 와이어링 객체 (도메인은 Installer 단위로 분리)
  - 주석/호출 순서로 초기화 순서 관리
  - 통증 없는 선제 추상화 (config 추상화, 토폴로지 정렬 등 — 증거 생기면 그때)
  - `IEventBus`는 4프로젝트 반복 증거로 코어 채택(`decisions/2026-06-11_eventbus-into-core.md`), 단 static 전역 ❌ 주입형으로만
- **핵심 규칙**
  - 로직은 순수 C# / Mono는 경계(인스펙터·씬 참조)에서만
  - 컨테이너를 만지는 건 인스톨러(조립 단계)의 특권, 서비스는 생성자 주입만
  - 의존은 생성자 시그니처에 드러난다
  - 실행 순서 = 커널 인스펙터의 Installer 리스트 (보이는 단일 출처)
- **주요 참고 문서:** `Packages/com.mc.corekit/README.md`, `decisions/2026-06-11_project-start.md`
