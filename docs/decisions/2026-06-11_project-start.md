---
type: DECISION
date: 2026-06-11
---

# 프로젝트 시작 — CoreKit

**성격:** 게임이 아닌 **경량 서비스/부트스트랩 프레임워크**. 여러 프로젝트에서 반복해 다시 짜던
"진입점 + 서비스 제공" 인프라를 뽑아 쓸 수 있는 라이브러리로 추출.

**동기 (통증):** Hearth의 `AppWiring`가 14도메인을 단일 객체로 배선하며 비대해짐 —
SerializeField 45개, 초기화 순서가 주석(`// 반드시 선행`)에 의존, 부트스트랩에 런타임 로직(UI switch·Provider 팩토리) 혼입.
패턴(Composition Root) 자체는 옳았으나 "단일 갓 객체"로 구현한 게 문제였음.

---

## 초기 결정 (7건)

1. **순수 C# 로직 + Mono 경계 분리 (Authoring/Runtime).**
   로직은 Unity 비의존, 인스펙터·씬 참조가 필요한 경우만 Mono(=Installer)가 담당.

2. **모듈러 Installer + 라이프사이클 커널.**
   갓 와이어링을 도메인별 `MonoInstaller`로 분해. `ServiceKernel`(유일한 Mono)이
   Install → Initialize → Tick → Dispose 를 가로질러 펌프.

3. **컨테이너는 인스톨러 전용.**
   `ServiceContainer`는 조립 단계(인스톨러)에서만 접근. 서비스는 생성자 주입만 받고
   컨테이너를 알지 못함 → 로케이터 안티패턴/숨은 의존 차단.

4. **실행 순서 = 보이는 단일 리스트.**
   커널 인스펙터의 `Installers` 리스트 순서가 곧 실행 순서. 주석 순서 관리 폐기.
   순서 오류는 조용한 버그 대신 명확한 예외로 표면화.

5. **토폴로지 자동 정렬 미도입 (YAGNI).**
   수동 순서가 아파지는 시점(인스톨러 10개+·의존 얽힘)에 추가. 지금은 안 쓰는 복잡도.

6. **블루프린트 = ScriptableObject로 시작, 추상화는 열어둠.**
   두 번째 config 타입이 실제로 나타나기 전까지 `IConfigSource` 같은 추상화 미도입.

7. **EventBus 의도적 미포함.**
   첫 컷은 해결/라이프사이클/순서만 증명. 통신은 필수→생성자 주입, 진짜 브로드캐스트
   필요가 생기면 그때 이벤트버스를 의도적으로 추가.

---

## 구조

```
PROJECT_CoreKit/
├─ docs/                              ← 이 문서
└─ CoreKit/                           ← Unity 프로젝트 (6000.3.10f1)
   ├─ Packages/com.mc.corekit/        ← 프레임워크 (임베디드 UPM)
   │  └─ Runtime/  Container · Lifecycle · Install · Kernel  (본체 4파일)
   └─ Assets/Sample/                  ← 증명 하네스 (GameClock·Audio 서비스)
```

**관통 원칙:** 모든 추상화는 이름 댈 수 있는 통증에 1:1 매핑된다. 매핑 안 되면 넣지 않는다.
