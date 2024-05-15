# 🎮Dungreed_Copycat_In_Unity🎮
**Unity 2D**를 이용해 만든 던그리드 모작입니다.
<img src= "https://github.com/Seoptank/DungreedCopy/assets/126733224/e9690d19-d333-4212-aef3-c1541f25e9dd">
<img src= "https://github.com/Seoptank/DungreedCopy/assets/126733224/47698492-305a-4daa-a05b-6dc9d8016eed">
## 🧙개발자 소개
1.**최유섭**: 팀장/클라이언트(플레이어 컨트롤러,보스/몬스터 AI, Effect, UI, NPC)

2.**김형수**: 클라이언트(미니맵, 인벤토리, 몬스터AI, UI, 무기 컨트롤러)
## 📈프로젝트 개요
<p align="center">
1. 개발 기간 
  
  6주
  
2. 개발 환경
     
<img src="https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white"/><img src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white"/><img src="https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white"/>

## 🧪사용 기술
|**기술**|**설명**|
|:---:|:---|
|SingleTone<dr>(싱글톤)|싱글톤 패턴을 통해 Manager관리하여 메모리 낭비 방지|
|FSM<dr>(상태 패턴)|상태 패턴을 통해 플레이어의 애니메이션,보스 및 몬스터의 AI통제|
|Strategy<dr>(전략 패턴)|전략 패턴을 통해 공통으로 쓰이는 알고리즘을 정의하고 각각 캡술화 하여 수정의 용이성과 객체간 결합도 최소화|
|ObjectPooling<dr>(오브젝트 풀링)|오브젝트 풀링을 통한 GC 호출 최소화|
|ScriptableObject|아이템을 생성,사용 및 인벤토리 관리|
## 💊구현 기능
* GameOgject
 * 플레이어
 * 땅 몬스터
   * 스켈리독
   * 미노타우루스
   * 스켈레톤(활,검)
   * 정예 스켈레톤
 * 공중 몬스터
   * 붉은 박쥐
   * 큰 박쥐
   * 큰 붉은 박쥐
   * 밴시
   * 꼬마유령   
 * 보스 몬스터
   * 벨리알(탄막 공격/ 레이저 공격/ 검 공격)
 * NPC
   * 상점 NPC
   * 업그레이드 NPC
   * 던전내 이벤트 NPC
 * 아이템
   * 무기(원거리, 창, 검)
   * 회복 아이템
* UI
  * Player UI
    * 상태 창(체력, 레벨,대쉬 카운트 ,골드, 현재무기), 인벤토리
  * NPC UI
    * 업그레이드 UI
    * 상점 UI
  * 기타 UI
    * 미니맵, 던전 맵, 데미지 텍스트, 설정 메뉴창 
## [🚩기술서 링크](https://docs.google.com/presentation/d/1jHJAIKg0ex0KCO2hozneaqXTghVdsbdJ29tOFAVNDkg/edit?usp=sharing)
## 🎬 영상링크
