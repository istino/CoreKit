# CoreKit

Unity용 경량 **서비스 / 부트스트랩 프레임워크**. 여러 개인 프로젝트에서 매번 다시 만들던
진입점·서비스 제공·수명 관리 코드를 임베디드 UPM 패키지로 추출한 것이다.

세 가지 관심사를 나눈다.

| 관심사 | 해법 |
|---|---|
| 의존성 해결 | `ServiceContainer` |
| 초기화 / Tick / 해제 순서 | `ServiceKernel` |
| 시스템 간 통신 | 생성자 주입 + `IEventBus` |

- 패키지 본체와 자세한 사용법: [`CoreKit/Packages/com.mc.corekit/README.md`](CoreKit/Packages/com.mc.corekit/README.md)
- 샘플 씬: `CoreKit/Assets/Sample/`

## 만든 이유

같은 부트스트랩 코드를 프로젝트마다 다시 쓰고 있었다.
**추상화는 통증이 증명될 때만 넣는다**는 기준으로, 반복이 확인된 것만 패키지에 올렸다.
config 추상화와 토폴로지 정렬은 아직 필요가 증명되지 않아 보류했다.

## 상태

설계 실험 겸 기초 구현이다. 태그·릴리스·레지스트리 배포와 자동화 테스트는 없다.
읽고 판단할 용도로 공개한다.

Unity 6 / C#
