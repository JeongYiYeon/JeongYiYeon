
public enum ErrorCode
{
    Succeeded = 0,

    // ����
    Unauthorized = 100,
    InvalidToken = 101, // �߸��� ��ū
    InvalidUid = 102, // �߸��� uid
    SuspendedUser = 103, // ������ ����
    WithdrawnUser = 104, // Ż���� ����
    InvalidParameter = 105, // �߸��� �Ķ�����̴�
    InvalidSocialUser = 106, // �߸��� �Ҽ� �����̴�
    NotFoundUser = 107, // ������ ��ã�Ҵ�
    InvalidUserDeviceId = 108, // �߸��� ���� ����̽� ���̵��̴�

    // ���� ����
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

    // ����
    AlreadyReceivedReward = 601, // ���� �̹� �޾���
    WaitForNextInterval = 602, // ���� ���� �����
    NothingMoreToReceive = 603, // �� ������ ����
    EnergyValueIsMaximum = 604, // ������ ���� �ִ�ġ�̴�

    // ����
    NotFoundMailItem = 701, // ������ ��ã�Ҵ�
    IsReceivedMailItem = 702, // ���� �����̴�

    // ĳ����
    NotFoundCharacter = 801, // ĳ���͸� ��ã�Ҵ�
    IsHeroTypeCharacter = 802, // ���� ĳ���ʹ�
    IsNotHeroTypeCharacter = 803, // ���� ĳ���Ͱ� �ƴϴ�
    LowerCharacterLevel = 804, // ���� ĳ���� ������ ����

    // ������
    LackOfItem = 901, // �������� ������
    NotFoundEquipItem = 902, // ���� �������� ã�� �� ����
    NotFoundSellItem = 903, // �� �������� ã�� �� ����
    IsNotForSale = 904, // �̰� ���Ĵ� �������̴�
    IsEquippedItem = 905, // �̰� ������ �������̴�

    // ��������
    IsNotClearedBeforeStage = 1001, // �� ���������� �Ȳ���
    IsNotEnteredeStage = 1002, // �� ���������� �Ȳ���
    StageEnterIsExpire = 1003, // �������� ������ ���� ��

    // ��
    IsNotBuyCharacter = 1101, // ����� ���� �� ���� ĳ���ʹ�
    AlreadyHaveCharacter = 1102, // �̹� ������ ĳ���� �̴�
    IsNotBuyEnergy = 1103, // ���� �� �� ���� ������

    // �����н�
    IsNotReceiveBeforeLevel = 1201, // ���� ������ �ȹ��� ���� �̴�
    LackOfPoint = 1202, // ����Ʈ�� �����ϴ�
}