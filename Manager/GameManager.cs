using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Coffee.UIExtensions;
using System.Linq;
using UnityEngine.U2D;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class GameManager : MonoSingletonInScene<GameManager>
{
    private enum EAuto
    {
        Off,
        On,
    }

    private enum EAlramLabel
    {
        Warning,
        GameOver,
        Reward
    }

    [SerializeField]
    private Canvas[] canvas;

    [SerializeField]
    private Stage stage = null;
    [SerializeField]
    private StageProgress stageProgress = null;

    [SerializeField]
    private Challenge challenge = null;

    [SerializeField]
    private UtilState autoState = null;

    [SerializeField]
    private TMP_Text labelGameSpeed = null;

    [SerializeField]
    private TMP_Text labelTotalCombatPower = null;

    [Header("플레이어블")]
    [SerializeField]
    private FollowTarget heroFollowRootTf = null;

    [SerializeField]
    private HeroCharacter heroCharacter = null;
    [SerializeField]
    private NpcHeroCharacter[] npcHeroCharacters = null;

    [Header("적")]
    [SerializeField]
    private Transform enemyRoot;
    [SerializeField]
    private FollowTarget enemyFollowRootTf = null;

    [SerializeField]
    private BackgroundController backGroundController = null;

    private float playTime = 0f;

    private int waveClearCnt = 0;
    private int waveCnt = 0;
    private int waveIdx = 0;
    private Queue<EnemyCharacter> enemyQueue = new Queue<EnemyCharacter>();
    private List<CharacterData> enemyWaveInfo = new List<CharacterData>();
    private List<float> enemyMultipleInfo = new List<float>();

    private int enemyKillCnt = 0;

    private float gameSpeed = 1f;

    private float multipleDropcoin = 1f;

    private List<DataStageRegen> stageRegenData = new List<DataStageRegen>();

    private bool isInit = false;

    private bool isWaveWait = false;

    private bool isPause = false;

    private bool isChallenge = false;

    private bool isAuto = false;

    public bool IsInit => isInit;

    public bool IsWaveWait => isWaveWait;

    public Transform EnemyRoot => enemyRoot;

    public float GameSpeed => gameSpeed;

    public HeroCharacter HeroCharacter => heroCharacter;

    public NpcHeroCharacter[] NpcHeroCharacters => npcHeroCharacters;

    public int WaveClearCnt => waveClearCnt;
    public int WaveCnt => waveCnt;

    public float MultipleDropcoin => multipleDropcoin;

    public bool IsChallenge => isChallenge;

    public bool IsAuto => isAuto;


    private void OnEnable()
    {
        Messenger.AddListener(ConstHelper.MessengerString.MSG_TOTAL_COMBAT_POWER, SetTotalCombatPower);

        stage.InitStage();
        InitGame();
    }
    private void OnDisable()
    {
        Messenger.RemoveListener(ConstHelper.MessengerString.MSG_TOTAL_COMBAT_POWER, SetTotalCombatPower);
    }

    private void InitGame()
    {
        CameraManager.Instance.SetCanvasCamera(canvas);

        gameSpeed = UtilPlayerPrefs.GetFloat(ConstHelper.MessengerString.MSG_GAMESPEED, 1f);

        isAuto = UtilPlayerPrefs.GetInt(ConstHelper.MessengerString.MSG_GAMEAUTO, 0) == 0 ? false : true;

        autoState.ActiveState(((EAuto)UtilPlayerPrefs.GetInt(ConstHelper.MessengerString.MSG_GAMEAUTO, 0)).ToString());

        labelGameSpeed.text = string.Format("x{0}", gameSpeed);
        backGroundController.SetSpeed(gameSpeed);        

        SetSkillDic();

        SetStage();

        StartCoroutine(IECheckWave());

        StartCoroutine(IEStartTimer());

        SetTotalCombatPower();

        SoundManager.Instance.PlayBGM(SoundManager.Instance.SoundDic["bgm"]);

        Messenger<float>.Broadcast(ConstHelper.MessengerString.MSG_GAMESPEED, gameSpeed, MessengerMode.DONT_REQUIRE_LISTENER);

        isInit = true;
    }

    public void RefreshCharacters()
    {
        HeroCharacter.RefreshCharacter();

        if (UserData.Instance.user.Character.GetFollowCharacters() != null && UserData.Instance.user.Character.GetFollowCharacters().Count > 0)
        {
            for (int i = 0; i < UserData.Instance.user.Character.GetFollowCharacters().Count; i++)
            {
                NpcHeroCharacters[i].RefreshCharacter();
            }
        }
    }

    private void SetCharacter()
    {
        heroCharacter.InitStat(UserData.Instance.user.Character.GetHeroCharacter());
        Messenger<BaseCharacter>.Broadcast(ConstHelper.MessengerString.MSG_STATUP_RESET, heroCharacter, MessengerMode.DONT_REQUIRE_LISTENER);
        SetSkill(UserData.Instance.user.Character.GetHeroCharacter());
        SetFollowCharacter();
        SetSkillIcon();
    }

    public void SetFollowCharacter()
    {
        int idx = 0;

        if (UserData.Instance.user.Character.GetFollowCharacters() != null && UserData.Instance.user.Character.GetFollowCharacters().Count > 0)
        {
            foreach (CharacterData characterData in UserData.Instance.user.Character.GetFollowCharacters())
            {
                SetFollowCharacterSetting(idx, characterData);
                SetSkill(characterData);

                npcHeroCharacters[idx].transform.SetParent(npcHeroCharacters[idx].GetFollowTarget(characterData.PositionIdx));
                npcHeroCharacters[idx].transform.localPosition = Vector3.zero;
                npcHeroCharacters[idx].gameObject.SetActive(true);

                idx++;
            }
        }

    }

    private void SetSkill(CharacterData data)
    {
        if(data.SkillGroupList != null && data.SkillGroupList.Count > 0)
        {
            for (int i = 0; i < data.SkillGroupList.Count; i++)
            {
                SkillManager.Instance.SetHaveSkill(SkillManager.Instance.SkillDic[data.SkillGroupList[i]][0]);
            }
        }       
    }

    public void SetSkillIcon()
    {
        int idx = 0;

        foreach(SkillData data in SkillManager.Instance.HaveSkillDic.Values)
        {
            SkillManager.Instance.SkillItems[idx].SetSkillData(data);
            SkillManager.Instance.SkillItems[idx].InitSkllItem();
            SkillManager.Instance.SkillItems[idx].SetState(SkillItem.EHaveSkill.Skill);
            idx++;
        }

        for(int i = idx; i < SkillManager.Instance.SkillItems.Length; i++)
        {
            SkillManager.Instance.SkillItems[i].SetState(SkillItem.EHaveSkill.Empty);
        }
    }

    public void SetNextStage()
    {
        stage.SetNextStage();

        SetStageData(stage.CurrentStageData.StageRegenGroupUID);
        SetStage();
        StartCoroutine(IECheckWave());
    }

    private void SetStage()
    {
        ResetEnemyQueue();

        isChallenge = false;

        SetCharacter();

        backGroundController.ChangeAtlas(AtlasManager.Instance.Atlas[stage.CurrentStageData.StagePrefabPath]);

        SetStageData(stage.CurrentStageData.StageRegenGroupUID, UserData.Instance.user.Stage.IsEnterBoss);

        if (UserData.Instance.user.Stage.IsEnterBoss == true)
        {
            stageProgress.SetState(StageProgress.EStageFlag.Boss);
        }
        else
        {
            stageProgress.SetState(StageProgress.EStageFlag.Flag);
            stageProgress.SetFlags(canvas[(int)CameraManager.ECamera.UI]);
        }

        stageProgress.SetStageName(stage.CurrentStageData.StageTitle);

    }

    public void SetChallenge()
    {
        ResetEnemyQueue();

        isChallenge = true;

        SetCharacter();

        backGroundController.ChangeAtlas(AtlasManager.Instance.Atlas[challenge.CurrentChallengeData.ChallengePrefabPath]);

        SetStageData(challenge.CurrentChallengeData.EnemyRegenGroupUID);

        stageProgress.SetStageName(challenge.CurrentChallengeData.ChallengeTitle);
        stageProgress.SetState(StageProgress.EStageFlag.Flag);
        stageProgress.SetFlags(canvas[(int)CameraManager.ECamera.UI]);
    }

    private void SetTotalCombatPower()
    {
        labelTotalCombatPower.text = UserData.Instance.user.Goods.GetPriceUnit(heroCharacter.TotalDamage);
    }

    private IEnumerator IEStartTimer()
    {
        while (true)
        {
            playTime += Time.deltaTime * gameSpeed;

            yield return null;
        }
    }

    private IEnumerator IEMoveBackGround()
    {
        while (true)
        {
            backGroundController.MoveBackGround(Vector2.right);

            if (isWaveWait == false)
            {
                backGroundController.MoveBackGround(Vector2.zero);

                break;
            }

            yield return null;
        }
    }


    private void SetStageData(int stageRegenGroupUID, bool isEnterBoss = false)
    {
        if(stageRegenGroupUID == 0)
        {
            Debug.LogError("스테이지 정보 에러");
            return;
        }

        stageRegenData.Clear();
        waveCnt = 0;
        waveClearCnt = 0;
        waveIdx = 0;

        stageRegenData = DataManager.Instance.DataHelper.StageRegen.FindAll(x => x.GROUP_NUMBER == stageRegenGroupUID);

        for (int i = 0; i < stageRegenData.Count; i++)
        {
            if (stageRegenData[i].ENEMY_TYPE != BaseCharacter.CHARACTER_TYPE.BOSS.ToString())
            {
                //보스가 아니면 일반 몹 웨이브 갯수
                waveCnt += stageRegenData[i].ENEMY_BOSS_COUNT;
            }
            else
            {
                //보스는 한마리만 있음
                waveCnt += 1;
            }
        }
    }

    private void SetSkillDic()
    {
        Dictionary<int, List<SkillData>> skillDic = new Dictionary<int, List<SkillData>>();
        List<SkillData> skills = new List<SkillData>();

        if (DataManager.Instance.DataHelper.SkillBase != null && DataManager.Instance.DataHelper.SkillBase.Count > 0)
        {
            for (int i = 0; i < DataManager.Instance.DataHelper.SkillBase.Count; i++)
            {
                SkillData skillData = new SkillData();

                DataOptionSet option = null;

                if (DataManager.Instance.DataHelper.SkillBase[i].OPTION_VALUE_1 > 0)
                {
                    option = DataManager.Instance.DataHelper.OptionSet.Find(x => x.UID == DataManager.Instance.DataHelper.SkillBase[i].OPTION_VALUE_1);

                    skillData.SetSkillOption(option);
                }
                if (DataManager.Instance.DataHelper.SkillBase[i].OPTION_VALUE_2 > 0)
                {
                    option = DataManager.Instance.DataHelper.OptionSet.Find(x => x.UID == DataManager.Instance.DataHelper.SkillBase[i].OPTION_VALUE_2);

                    skillData.SetSkillOption(option);
                }
                if (DataManager.Instance.DataHelper.SkillBase[i].OPTION_VALUE_3 > 0)
                {
                    option = DataManager.Instance.DataHelper.OptionSet.Find(x => x.UID == DataManager.Instance.DataHelper.SkillBase[i].OPTION_VALUE_3);

                    skillData.SetSkillOption(option);
                }

                skillData.SetSkillData(DataManager.Instance.DataHelper.SkillBase[i]);

                if (skillData.SkillOptions.Count > 0)
                {
                    object[] localParam = new object[skillData.SkillOptions.Count];

                    for (int j = 0; j < skillData.SkillOptions.Count; j++)
                    {
                        if (skillData.SkillOptions[j].ValueType == BaseOptionData.EOptionValueType.PER)
                        {
                            localParam[j] = Mathf.Abs(skillData.SkillOptions[j].Value - 100f);
                        }
                        else
                        {
                            localParam[j] = skillData.SkillOptions[j].Value;
                        }
                    }

                    skillData.SetTitle(DataManager.Instance.GetLocalization(DataManager.Instance.DataHelper.SkillBase[i].SKILL_NAME, localParam));
                    skillData.SetDesc(DataManager.Instance.GetLocalization(DataManager.Instance.DataHelper.SkillBase[i].SKILL_DESC, localParam));
                }

                else
                {
                    skillData.SetDesc(DataManager.Instance.GetLocalization(DataManager.Instance.DataHelper.SkillBase[i].SKILL_DESC));
                }

                skillData.SetSkillAtlas(DataManager.Instance.DataHelper.SkillBase[i].SKILL_ICON_ATLAS);
                skillData.SetSkillImgPath(DataManager.Instance.DataHelper.SkillBase[i].SKILL_IMG);
                skillData.SetSkillEffectPath(DataManager.Instance.DataHelper.SkillBase[i].SKILL_EFFECT_FILE);
                skillData.SetSkillGroup(DataManager.Instance.DataHelper.SkillBase[i].SKILL_GROUP);
                skillData.SetSkillLevel(DataManager.Instance.DataHelper.SkillBase[i].SKILL_LEVEL);
                skillData.SetCategory(DataManager.Instance.DataHelper.SkillBase[i].SKILL_CATEGORY);
                skillData.SetType(DataManager.Instance.DataHelper.SkillBase[i].SKILL_TYPE);
                skillData.SetCoolTime((float)DataManager.Instance.DataHelper.SkillBase[i].SKILL_COOLTIME);
                skillData.SetTime((float)DataManager.Instance.DataHelper.SkillBase[i].SKILL_TIME);
                skillData.SetInterval((float)DataManager.Instance.DataHelper.SkillBase[i].SKILL_INTERVALL);
                skillData.SetTarget(DataManager.Instance.DataHelper.SkillBase[i].SKILL_TARGET_1);

                skills.Add(skillData);
            }

            for (int i = 0; i < skills.Count; i++)
            {
                List<SkillData> skillGroup = skills.FindAll(x => x.SkillGroup == skills[i].SkillGroup);

                if (skillGroup != null && skillGroup.Count > 0)
                {
                    if (skillDic.ContainsKey(skills[i].SkillGroup) == false)
                    {
                        skillDic.Add(skills[i].SkillGroup, skillGroup);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        SkillManager.Instance.SetSkillDic(skillDic);
    }


    private void SetFollowCharacterSetting(int idx, CharacterData dataCharacter)
    {
        npcHeroCharacters[idx].InitStat(dataCharacter);
    }

    public void AddKillEnemyCnt()
    {
        enemyKillCnt += 1;
    }

    public void OnClickGameSpeed()
    {
        gameSpeed += 0.5f;

        if (gameSpeed > 2f)
        {
            gameSpeed = 1f;
        }

        labelGameSpeed.text = string.Format("x{0}", gameSpeed);

        Messenger<float>.Broadcast(ConstHelper.MessengerString.MSG_GAMESPEED, gameSpeed);

        backGroundController.SetSpeed(gameSpeed);
        UtilPlayerPrefs.SetFloat(ConstHelper.MessengerString.MSG_GAMESPEED, gameSpeed);
    }

    public void OnClickAuto()
    {
        if(isAuto == false)
        {
            isAuto = true;
            autoState.ActiveState(EAuto.On.ToString());
        }
        else
        {
            isAuto = false;
            autoState.ActiveState(EAuto.Off.ToString());
        }

        UtilPlayerPrefs.SetInt(ConstHelper.MessengerString.MSG_GAMEAUTO, isAuto == false ? 0 : 1);
    }

    public void OnClickEnterBoss()
    {
        if(UserData.Instance.user.Stage.IsEnterBoss == false)
        {
            return;
        }
        else
        {
            ResetEnemyQueue();
            waveClearCnt = waveCnt - 1;
            waveIdx = stageRegenData.Count - 1;

            stageProgress.SetState(StageProgress.EStageFlag.None);
        }        
    }

    public void SetGameSpd(float gameSpd)
    {
        gameSpeed = gameSpd;
    }

    public void SetMultipleDropcoin(float multiple)
    {
        multipleDropcoin = multiple;
    }

    private void SetEnemyWave(DataStageRegen dataStageRegen)
    {
        enemyWaveInfo.Clear();
        enemyMultipleInfo.Clear();

        SetAvailableEnemyData(dataStageRegen.REGEN_CHARACTER_UID_1, (float)dataStageRegen.REGEN_CHA_MULTIPLE_1);
        SetAvailableEnemyData(dataStageRegen.REGEN_CHARACTER_UID_2, (float)dataStageRegen.REGEN_CHA_MULTIPLE_2);
        SetAvailableEnemyData(dataStageRegen.REGEN_CHARACTER_UID_3, (float)dataStageRegen.REGEN_CHA_MULTIPLE_3);
        SetAvailableEnemyData(dataStageRegen.REGEN_CHARACTER_UID_4, (float)dataStageRegen.REGEN_CHA_MULTIPLE_4);
        SetAvailableEnemyData(dataStageRegen.REGEN_CHARACTER_UID_5, (float)dataStageRegen.REGEN_CHA_MULTIPLE_5);
    }

    private void SetAvailableEnemyData(int uid, float multiple = 1f)
    {
        if (uid > 0)
        {
            CharacterData enemyData = new CharacterData();

            DataCharacter data = DataManager.Instance.DataHelper.Character.Find(x => x.UID == uid);

            if(data != null)
            {
                enemyData.SetDataCharacter(data);
            }

            enemyWaveInfo.Add(enemyData);
            enemyMultipleInfo.Add(multiple);
        }
    }

    private void SetEnemy()
    {
        int rndIdx = 0;
        int regenRndEnemy = UnityEngine.Random.Range(stageRegenData[waveIdx].REGEN_CHARACTER_COUNT_MIN, stageRegenData[waveIdx].REGEN_CHARACTER_COUNT_MAX + 1);

        var exclude = new HashSet<int>();

        for (int i = 0; i < regenRndEnemy; i++)
        {
            rndIdx = UnityEngine.Random.Range(0, enemyWaveInfo.Count);

            CharacterData enemyData = enemyWaveInfo[rndIdx];

            EnemyCharacter enemy = enemyRoot.GetChild(0).GetComponent<EnemyCharacter>();

            if (stageRegenData[waveIdx].ENEMY_TYPE == BaseCharacter.CHARACTER_TYPE.BOSS.ToString())
            {
                if (UserData.Instance.user.Stage.IsEnterBoss == false)
                {
                    NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.STAGE_BOSS, stage.CurrentStageData.UID);
                }

                enemy.InitEnemy(enemyData, 8, enemyMultipleInfo[rndIdx]);

                enemy.SetSize(1f);
            }
            else
            {
                var range = Enumerable.Range(1, 9).Where(i => !exclude.Contains(i));
                var rand = new System.Random();
                int index = rand.Next(1, 9 - exclude.Count);
                
                int targetIdx = range.ElementAtOrDefault(index);

                if(targetIdx == 0)
                {
                    //예외값 나왔으면 그냥 맨 앞에 있는거 넣어줘서 보냄
                    targetIdx = range.ElementAt(0);
                }

                enemy.InitEnemy(enemyData, targetIdx, enemyMultipleInfo[rndIdx]);
                enemy.SetSize(1f);
                exclude.Add(targetIdx);
            }
            //enemyFollowRootTf.AddVec2Event(enemy.SetInputVec);
            enemyQueue.Enqueue(enemy);
        }
    }

    public void SetGameOverAlram()
    {
        ResetEnemyQueue();
        heroCharacter.Reset();
        heroCharacter.InitStat(UserData.Instance.user.Character.GetHeroCharacter());
        
        Messenger<BaseCharacter>.Broadcast(ConstHelper.MessengerString.MSG_STATUP_RESET, heroCharacter, MessengerMode.DONT_REQUIRE_LISTENER);

        SetStageData(stage.CurrentStageData.StageRegenGroupUID);
        SetStage();

        ShowDefeatPopup defeatPopup =
                    UIManager.Instance.GetPopup(BasePopup.EPopupType.ShowDefeat) as ShowDefeatPopup;

        defeatPopup.Active();
    }

    private void ResetEnemyQueue()
    {
        if (enemyQueue != null && enemyQueue.Count > 0)
        {
            while (enemyQueue.Count > 0)
            {
                enemyQueue.Dequeue().ReturnEnemy();
            }
        }

        enemyKillCnt = 0;
        enemyQueue.Clear();
    }

    public string GetUnitString(double value)
    {
        //아스키 65 = A 90 = Z
        string[] tmpValueString = value.ToString("E").Split('+');
        string AsciiUnit = "";
        double tmpValue = 0;

        if (tmpValueString.Length < 2)
        {
            return string.Empty;
        }
        else
        {
            if (double.TryParse(tmpValueString[1], out tmpValue) == false)
            {
                return string.Empty;
            }
            else
            {
                if (tmpValue < 3)
                {
                    return string.Format($"{value:#,##0}");
                }

                int tmpAsciiIdx = (int)tmpValue / 3;

                char[] asciiArray = new char[2];

                //27부터는 AA시작 해야됨
                if (tmpAsciiIdx > 26)
                {
                    int tmpFirstAscii = tmpAsciiIdx / 27;
                    int tmpSecondAscii = tmpAsciiIdx % 27;

                    asciiArray[0] = (char)(64 + tmpFirstAscii);
                    asciiArray[1] = (char)(65 + tmpSecondAscii);    // 나머지가 0일때 A부터 표현
                }
                else
                {
                    asciiArray[0] = (char)(64 + tmpAsciiIdx);
                }

                AsciiUnit = new string(asciiArray);

                return string.Format($"{(Math.Truncate(double.Parse(tmpValueString[0].Replace("E", "")) * 100) / 100).ToString("#.#0")}{AsciiUnit}");
            }
        }
    }
       
    private IEnumerator IERespawnEnemy()
    {
        SetEnemyWave(stageRegenData[waveIdx]);
        SetEnemy();

        yield return new WaitForEndOfFrame();
    }

    private IEnumerator IECheckWave()
    {
        while (true)
        {
            if (gameSpeed > 0)
            {
                if (enemyQueue.Count > 0)
                {
                    if (enemyKillCnt >= enemyQueue.Count)
                    {
                        if (waveClearCnt < waveCnt)
                        {
                            waveClearCnt += 1;

                            if (waveClearCnt == waveCnt - 1)
                            {
                                if (UserData.Instance.user?.Stage.IsEnterBoss == true)
                                {
                                    waveClearCnt = 0;
                                    waveIdx = 0;
                                    enemyKillCnt = 0;
                                    enemyQueue.Clear();
                                }
                                else
                                {
                                    waveIdx += 1;
                                }
                            }

                            enemyKillCnt = 0;
                            enemyQueue.Clear();
                        }
                        else
                        {
                            if (isChallenge == false)
                            {
                                NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.STAGE_CLEAR, stage.CurrentStageData.UID);

                                Debug.LogError("스테이지 클리어");

                                yield break;
                            }
                            else
                            {
                                //도전 클리어 정보 서버로 보내고 스테이지로
                                SetStage();
                            }
                        }
                    }
                }
                else
                {
                    if(waveClearCnt == waveCnt)
                    {
                        if (isChallenge == false)
                        {
                            NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.STAGE_CLEAR, stage.CurrentStageData.UID);

                            Debug.LogError("스테이지 클리어");
                            yield break;
                        }
                        else
                        {
                            //도전 클리어 정보 서버로 보내고 스테이지로
                            SetStage();
                        }
                    }

                    yield return new WaitWhile(() => isPause);

                    isWaveWait = true;

                    StartCoroutine(IEMoveBackGround());
                    yield return StartCoroutine(stageProgress.IEWaveProgressBar());
                    yield return StartCoroutine(IERespawnEnemy());

                    isWaveWait = false;
                }
            }

            yield return new WaitForEndOfFrame();
        }

    }

}
