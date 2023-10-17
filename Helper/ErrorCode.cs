
public enum ErrorCode
{
    Succeeded = 0,

    // 유저
    Unauthorized = 100,
    InvalidToken = 101, // 잘못된 토큰
    InvalidUid = 102, // 잘못된 uid
    SuspendedUser = 103, // 정지된 유저
    WithdrawnUser = 104, // 탈퇴한 유저
    InvalidParameter = 105, // 잘못된 파라미터이다
    InvalidSocialUser = 106, // 잘못된 소셜 유저이다
    NotFoundUser = 107, // 유저를 못찾았다
    InvalidUserDeviceId = 108, // 잘못된 유저 디바이스 아이디이다

    // 서버 에러
    InternalError = 300,
    DatabaseFailure = 301,
    TransactionConflict = 302,
    DatabaseConditionError = 303,
    DatabaseLimitExceeded = 304,
    AlreadyCreated = 305,

    CriticalError = 502,
    InvalidDBData = 503,
    InvalidTableData = 504,
    InvalidCharacterItem = 510,
    UnderMaintenance = 520,

    // 보상
    AlreadyReceivedReward = 601, // 보상 이미 받았음
    WaitForNextInterval = 602, // 다음 간격 대기중
    NothingMoreToReceive = 603, // 더 받을게 없다
    EnergyValueIsMaximum = 604, // 에너지 값은 최대치이다

    // 메일
    NotFoundMailItem = 701, // 메일을 못찾았다
    IsReceivedMailItem = 702, // 받은 메일이다

    // 캐릭터
    NotFoundCharacter = 801, // 캐릭터를 못찾았다
    IsHeroTypeCharacter = 802, // 영웅 캐릭터다
    IsNotHeroTypeCharacter = 803, // 영웅 캐릭터가 아니다
    LowerCharacterLevel = 804, // 영웅 캐릭터 레벨이 낮음

    // 아이템
    LackOfItem = 901, // 아이템이 모질람
    NotFoundEquipItem = 902, // 장착 아이템을 찾을 수 없다
    NotFoundSellItem = 903, // 팔 아이템을 찾을 수 없다
    IsNotForSale = 904, // 이거 못파는 아이템이다
    IsEquippedItem = 905, // 이거 장착된 아이템이다

    // 스테이지
    IsNotClearedBeforeStage = 1001, // 전 스테이지를 안깻음
    IsNotEnteredeStage = 1002, // 전 스테이지를 안깻음
    StageEnterIsExpire = 1003, // 스테이지 입장이 만료 됨

    // 샵
    IsNotBuyCharacter = 1101, // 히어로 구매 시 기존 캐릭터다
    AlreadyHaveCharacter = 1102, // 이미 소유한 캐릭터 이다
    IsNotBuyEnergy = 1103, // 구매 할 수 없다 에너지

    // 게임패스
    IsNotReceiveBeforeLevel = 1201, // 이전 레벨을 안받은 상태 이다
    LackOfPoint = 1202, // 포인트가 부족하다
}