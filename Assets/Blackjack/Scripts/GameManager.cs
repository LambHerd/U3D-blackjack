using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    private GameState _currentState;
    private GameAction _currentAction;
    private Coroutine _computerTurnCoroutine;

    [SerializeField]
    private AudioManager _audioManager;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private Dealer _dealer;
    [SerializeField]
    private ComputerPlayer _computer;
    [SerializeField]
    private HumanPlayer _human;

    public delegate void GameStateEvent(GameState state);
    public event GameStateEvent OnGameStateChanged;

    public delegate void GameActionEvent(GameAction actions);
    public event GameActionEvent OnGameActionChanged;


    //modify
    int chips = 0;
    int human_health = 100;
    int devil_health = 100;

    public TMP_Text human_health_txt, devil_health_txt, human_bet_txt;
    public Button BetBtn;
    public GameObject BetPanel;

    public GameObject Chip_1, Chip_5, Chip_10;
    public GameObject Chip_pos, Bet_Chip_pos;

    List<GameObject> coins = new List<GameObject>();

    public Button CardBtn;
    public GameObject CardPanel;

    List<int> skillcards = new List<int>();//0:greed 1:joker 2:life 3:power 4:sight
    List<GameObject> skillcardsObject = new List<GameObject>();
    public List<GameObject> SkillCardProfab;

    bool isGreedyCardActive = false;

    public GameObject FadePanel,ESCPanel,CheatPanel;
    public Button CheatHumanWinBtn, CheatDevilWinBtn,CheatAddCardBtn;
    public TMP_InputField CheatAddCardInput;

    //modify
    public TMP_Text HumanDialogueTxt;
    public TMP_Text DevilDialogueTxt;

    bool DevilSuddenWin = false, HumanSuddenWin = false;

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if (ESCPanel.activeInHierarchy != true)
            {
                ESCPanel.SetActive(true);
            }
            else
            {
                ESCPanel.SetActive(false);
            }
            
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (CheatPanel.activeInHierarchy != true)
            {
                CheatPanel.SetActive(true);
            }
            else
            {
                CheatPanel.SetActive(false);
            }
        }
    }

    private GameState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            if (_currentState != value)
            {
                _currentState = value;

                if (OnGameStateChanged != null)
                {
                    OnGameStateChanged(_currentState);
                }
            }
        }
    }

    private GameAction CurrentAction
    {
        get
        {
            return _currentAction;
        }
        set
        {
            if (_currentAction != value)
            {
                _currentAction = value;

                if (OnGameActionChanged != null)
                {
                    OnGameActionChanged(_currentAction);
                }
            }
        }
    }

    private void Start()
    {
        Subscriptions();

        OnNewGameEvent();


        //modify
        BetBtn.onClick.AddListener(OnBetBtnClick);
        CardBtn.onClick.AddListener(OnCardBtnClick);


        //skillcards.Add(GetARandNum(0, 4));
        //skillcards.Add(GetARandNum(0, 4));
        //skillcards.Add(GetARandNum(0, 4));
        //skillcards.Add(GetARandNum(0, 4));

        skillcards.Add(0);
        skillcards.Add(1);
        skillcards.Add(2);
        skillcards.Add(3);

        CheatHumanWinBtn.onClick.AddListener(OnCheatHumanWinBtnClick); 
        CheatDevilWinBtn.onClick.AddListener(OnCheatDevilWinBtnClick);
        CheatAddCardBtn.onClick.AddListener(OnCheatAddCardBtnClick);
    }

    private void Subscriptions()
    {
        _uiManager.OnDealButtonEvent += OnButtonEvent;
        _uiManager.OnHitButtonEvent += OnButtonEvent;
        _uiManager.OnStandButtonEvent += OnButtonEvent;
        _uiManager.OnNewGameButtonEvent += OnButtonEvent;
        _uiManager.OnExitButtonEvent += OnButtonEvent;

        _uiManager.OnDealButtonEvent += OnCardEvent;
        _uiManager.OnHitButtonEvent += OnCardEvent;

        _uiManager.OnDealButtonEvent += OnDealEvent;
        _uiManager.OnHitButtonEvent += OnHitEvent;
        _uiManager.OnStandButtonEvent += OnStandEvent;
        _uiManager.OnNewGameButtonEvent += OnNewGameEvent;
        _uiManager.OnExitButtonEvent += OnExitEvent;

        OnGameStateChanged += _uiManager.NotificationHandler.OnUpdateNotification;
        OnGameActionChanged += _uiManager.OnUpdateGameplayButtons;
    }

    private void OnButtonEvent()
    {
        _audioManager.PlayButtonClip();
    }

    private void OnCardEvent()
    {
        _audioManager.PlayCardClip();
    }

    private void OnDealEvent()
    {
        _dealer.Deal(_human);
        _dealer.Deal(_computer, false);

        EvaluateHands(GameState.HumanTurn);
    }

    private void OnHitEvent()
    {
        _dealer.GiveCard(_human);

        EvaluateHands(GameState.ComputerTurn);
    }

    private void OnStandEvent()
    {
        _human.IsHitting = false;

        EvaluateHands(GameState.ComputerTurn);
    }

    private void OnNewGameEvent()
    {
        _dealer.Reset(_human, _computer);

        CurrentState = GameState.None;
        CurrentAction = GameAction.Deal;
    }

    private void OnExitEvent()
    {
        if (_computerTurnCoroutine != null)
        {
            StopCoroutine(_computerTurnCoroutine);
        }

        SceneManager.LoadScene("Menu");
    }

    private IEnumerator OnComputerTurn()
    {
        yield return _computer.TurnWaitForSeconds;

        _computer.UpdateBehaviour(_human.Hand);

        if (_computer.IsHitting)
        {
            _dealer.GiveCard(_computer);
        }

        EvaluateHands(GameState.HumanTurn);
    }

    private void EvaluateHands(GameState nextState)
    {
        bool moveToNextState = false;

        int computerTotalValue = _computer.Hand.TotalValue;
        int humanTotalValue = _human.Hand.TotalValue;

        print("computerTotalValue:" + computerTotalValue);
        print("humanTotalValue:" + humanTotalValue);

        if (_computer.IsHitting || _human.IsHitting)
        {
            print("if (_computer.IsHitting || _human.IsHitting)");
            if (humanTotalValue == 21 && computerTotalValue == 21)
            {
                print("if (humanTotalValue == 21 && computerTotalValue == 21)");
                CurrentState = GameState.Draw;
            }
            else if (computerTotalValue > 21 || humanTotalValue == 21 || (humanTotalValue > computerTotalValue && !_computer.IsHitting))
            {
                print("else if (computerTotalValue > 21 || humanTotalValue == 21 || (humanTotalValue > computerTotalValue && !_computer.IsHitting))");
                CurrentState = GameState.HumanWon;
            }
            else if (humanTotalValue > 21 || computerTotalValue == 21 || (computerTotalValue > humanTotalValue && !_human.IsHitting))
            {
                print("else if (humanTotalValue > 21 || computerTotalValue == 21 || (computerTotalValue > humanTotalValue && !_human.IsHitting))");
                CurrentState = GameState.ComputerWon;
            }
            else
            {
                moveToNextState = true;
            }
        }
        else if (!_computer.IsHitting && !_human.IsHitting)
        {
            print("else if (!_computer.IsHitting && !_human.IsHitting)");
            if (computerTotalValue > 21 || humanTotalValue > computerTotalValue)
            {
                print("else if (computerTotalValue > 21 || humanTotalValue > computerTotalValue)");
                CurrentState = GameState.HumanWon;
            }
            else if (humanTotalValue > 21 || computerTotalValue > humanTotalValue)
            {
                print("if (humanTotalValue > 21 || computerTotalValue > humanTotalValue)");
                CurrentState = GameState.ComputerWon;
            }
            else
            {
                CurrentState = GameState.Draw;
            }
        }

        if (moveToNextState)
        {
            if ((nextState == GameState.HumanTurn && _human.IsHitting) || (nextState == GameState.ComputerTurn && !_computer.IsHitting))
            {
                CurrentState = GameState.HumanTurn;
                CurrentAction = GameAction.HitAndStand;
            }
            else
            {
                CurrentState = GameState.ComputerTurn;
                CurrentAction = GameAction.None;

                _computerTurnCoroutine = StartCoroutine(OnComputerTurn());
            }
        }
        else
        {
            EndGame();
        }
    }
    
    private void EndGame()//modify
    {
        _computer.Hand.Show();

        print("CurrentState: " + CurrentState);

        //cheat part
        if (HumanSuddenWin)
        {
            //end:devil lose
            HumanDialogueTxt.text = "What do you say now, Devil?";
            DevilDialogueTxt.text = "Game over now. . . .";
            _audioManager.PlayDialogue("demondie");
            FadePanel.GetComponent<FadeInPanel>().TransToScene = "004";
            FadePanel.SetActive(true);
            return;
        }
        else if (DevilSuddenWin)
        {
            //end:human lose
            HumanDialogueTxt.text = "Oh, Satan, NO!";
            DevilDialogueTxt.text = "See you soon, human...";
            _audioManager.PlayDialogue("humandie");
            FadePanel.GetComponent<FadeInPanel>().TransToScene = "001";
            FadePanel.SetActive(true);
        }
        //cheat part end

        if (CurrentState == GameState.HumanWon)
        {
            _human.Score++;
            human_health += chips;
            devil_health -= chips;
            
            if (isGreedyCardActive)
            {
                human_health += chips;
                devil_health -= chips;
                isGreedyCardActive = false;
                HumanDialogueTxt.text = "Pay twice as much as you bet. You're bleeding, Devil.";
                DevilDialogueTxt.text = "The devil will make you pay more.";
                _audioManager.PlayDialogue("GreedyCardActive");
            }
            else
            {
                HumanDialogueTxt.text = "I won. Devil, don’t you think this is simple?";
                DevilDialogueTxt.text = "You are a lucky man, human.";
                _audioManager.PlayDialogue("HumanWon");
            }
        }
        else if (CurrentState == GameState.ComputerWon)
        {
            _computer.Score++;
            human_health -= chips;
            devil_health += chips;
            HumanDialogueTxt.text = "Satan bless.";
            DevilDialogueTxt.text = "The Devil take the hindmost.";
            _audioManager.PlayDialogue("ComputerWon");

            if (skillcards.Count < 4)
            {
                skillcards.Add(GetARandNum(0, 4));
            }

        }
        else
        {
            HumanDialogueTxt.text = "Almost.";
            DevilDialogueTxt.text = "Lucky.";
        }

        if (human_health <= 0)
        {
            //end:human lose
            HumanDialogueTxt.text = "Oh, Satan, NO!";
            DevilDialogueTxt.text = "See you soon, human...";
            _audioManager.PlayDialogue("humandie");
            FadePanel.GetComponent<FadeInPanel>().TransToScene = "001";
            FadePanel.SetActive(true);
        }
        else if (devil_health <= 0)
        {
            //end:devil lose
            HumanDialogueTxt.text = "What do you say now, Devil?";
            DevilDialogueTxt.text = "Game over now. . . .";
            _audioManager.PlayDialogue("demondie");
            FadePanel.GetComponent<FadeInPanel>().TransToScene = "004";
            FadePanel.SetActive(true);
        }
        else
        {
            if (human_health > 100)
            {
                human_health = 100;
            }
            if (devil_health > 100)
            {
                devil_health = 100;
            }

            human_health_txt.text = human_health + "";
            devil_health_txt.text = devil_health + "";
        }

        chips = 0;
        human_bet_txt.text = "bet: " + chips;

        CurrentAction = GameAction.NewGame;

        _uiManager.UpdateScore(_human.Score, _computer.Score);
    }

    void OnBetBtnClick()//modify
    {
        if (BetPanel.activeInHierarchy != true)
        {
            BetPanel.SetActive(true);
            CalculateCoins(human_health - chips);
        }
        else
        {
            BetPanel.SetActive(false);
            ClearCoins();
        }

    }

    void CalculateCoins(int number)//modify
    {
        int tens = number / 10; // 计算10的数量
        int fives = (number % 10) / 5; // 计算5的数量
        int ones = (number % 10) % 5; // 计算1的数量

        Debug.Log(number + "由" + tens + "个10，" + fives + "个5，" + ones + "个1组成");

        float x_pos = 0;
        float y_pos = 0;

        for (int i = 0; i < tens; i++)
        {
            GameObject go = Instantiate(Chip_10, Chip_pos.transform.position + new Vector3(x_pos + 110f * i, y_pos, 0), Quaternion.identity);
            go.transform.parent = BetPanel.transform;
            go.GetComponent<Button>().onClick.AddListener(() => OnCoinClick(go, 10));
            coins.Add(go);
        }

        for (int i = 0; i < fives; i++)
        {
            GameObject go = Instantiate(Chip_5, Chip_pos.transform.position + new Vector3(x_pos + 110f * i, y_pos - 120f, 0), Quaternion.identity);
            go.transform.parent = BetPanel.transform;
            go.GetComponent<Button>().onClick.AddListener(() => OnCoinClick(go, 5));
            coins.Add(go);
        }
        for (int i = 0; i < ones; i++)
        {
            GameObject go = Instantiate(Chip_1, Chip_pos.transform.position + new Vector3(x_pos + 110f * i, y_pos - 240f, 0), Quaternion.identity);
            go.transform.parent = BetPanel.transform;
            go.GetComponent<Button>().onClick.AddListener(() => OnCoinClick(go, 1));
            coins.Add(go);
        }
    }

    void ClearCoins()//modify
    {
        for (int i = 0; i < coins.Count; i++)
        {
            Destroy(coins[i]);
        }
        coins.Clear();
    }

    void OnCoinClick(GameObject go, int coin)//modify
    {
        go.transform.parent = BetPanel.transform;
        go.GetComponent<MoveToBet>().StartBet(Bet_Chip_pos.transform.position);
        Bet_Chip_pos.transform.position += new Vector3(50f, 0, 0);
        chips += coin;
        human_bet_txt.text = "bet: " + chips;
    }

    void OnCardBtnClick()
    {
        if (CardPanel.activeInHierarchy != true)
        {
            CardPanel.SetActive(true);
            ShowSkillCard();
        }
        else
        {
            CardPanel.SetActive(false);
            ClearSkillCardObject();
        }
    }

    void ShowSkillCard()
    {
        for (int i = 0; i < skillcards.Count; i++)
        {
            GameObject go = Instantiate(SkillCardProfab[skillcards[i]], CardPanel.transform);
            go.GetComponent<Tooltip>().index = i;
            go.GetComponent<Button>().onClick.AddListener(() => OnSkillCardClick(go));
            skillcardsObject.Add(go);
        }
    }

    void ClearSkillCardObject()
    {
        for (int i = 0; i < skillcardsObject.Count; i++)
        {
            Destroy(skillcardsObject[i]);
        }
        skillcardsObject.Clear();
    }
    int GetARandNum(int minRange, int maxRange)
    {
        // 生成一个指定范围内的随机整数
        int randomNumber = Random.Range(minRange, maxRange + 1); // maxRange + 1 用于确保包括最大值
        Debug.Log("Random Number: " + randomNumber);
        return randomNumber;
    }

    void OnSkillCardClick(GameObject go)
    {
        if (go.GetComponent<Tooltip>().used)
        {
            return;
        }
        go.GetComponent<Tooltip>().OnClickToMove();
        int ind = go.GetComponent<Tooltip>().index;
        go.GetComponent<Tooltip>().used = true;
        int type = skillcards[ind];
        skillcards.RemoveAt(ind);
        for (int i = 0; i < skillcardsObject.Count; i++)
        {
            if (skillcardsObject[i].GetComponent<Tooltip>().index > ind)
            {
                skillcardsObject[i].GetComponent<Tooltip>().index -= 1;
            }
        }

        //use card:  type
        //to do
        // 0:greed 1:joker 2:life 3:power 4:sight
        if (type == 0)
        {
            //玩家如果赢了，电脑跟注两倍
            GreedCard();
        }
        else if (type == 1)
        {
            //出3选1，替换你最新那张手牌
            JokerCard();

            //Invoke("CloseCardPanel", 0.2f);

        }
        else if (type == 2)
        {
            //生命值回100
            LifeCard();

        }
        else if (type == 3)
        {
            //强迫对面摸一张牌
            PowerCard();

        }
        else if (type == 4)
        {
            //让对面首张手牌翻开
            SightCard();

        }


    }

    void CloseCardPanel()
    {
        CardPanel.SetActive(false);
        ClearSkillCardObject();
    }

    //强迫对面摸一张牌
    void PowerCard()
    {
        Debug.Log("PowerCard");
        _dealer.GiveCard(_computer);
        _computer.UpdateBehaviour(_human.Hand);

    }


    //让对面首张手牌翻开
    void SightCard()
    {
        Debug.Log("SightCard");
        _computer.Hand.Show();
    }


    //出3选1，替换你最新那张手牌
    void JokerCard()
    {
        Debug.Log("JokerCard");

        //使用dealer生成3张（很多方法都在dealer里面，得注意看）
        _dealer.ThreenewCards(_human);
        //开始协程
        StartCoroutine(DoRaycast());
    }

    //由于3张卡不是按钮，不能添加监听，选择用协程来实现等待点击
    IEnumerator DoRaycast()
    {
        Debug.Log("in coroutine");
        while (true)
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                // 发射射线
                Ray ray;
                //Debug.Log(UnityEngine.Input.mousePosition);

                ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //print("ray");
                    GameObject carddisplay = hit.collider.gameObject;
                    CardDisplay display = carddisplay.GetComponent<CardDisplay>();                   

                    if (display != null)
                    {
                        //print("display");
                        Transform parent = _human.transform;
                        Vector3 position = parent.position;
                        Debug.Log(parent.childCount);

                        position.z -= (parent.childCount - 1) * 0.1f;
                        Debug.Log(position.z);
                        display.transform.position = position;

                        _dealer.Remove3Cards(_human, display);

                        yield break;
                    }

                }

            }
            yield return null;
        }
    }

    void LifeCard()
    {
        human_health = Mathf.Min(human_health + 50, 100);
        human_health_txt.text = human_health.ToString();
    }

    void GreedCard()
    {
        isGreedyCardActive = true;
    }


    void OnCheatHumanWinBtnClick()
    {
        //human_health = 100;
        //devil_health = 0;
        //human_health_txt.text = human_health.ToString();
        //devil_health_txt.text = devil_health.ToString();
        HumanSuddenWin = true;
    }
    void OnCheatDevilWinBtnClick()
    {
        //human_health = 0;
        //devil_health = 100;
        //human_health_txt.text = human_health.ToString();
        //devil_health_txt.text = devil_health.ToString();
        DevilSuddenWin = true;
    }

    void OnCheatAddCardBtnClick()
    {
        int type = int.Parse(CheatAddCardInput.text);
        if (skillcards.Count == 4)
        {
            skillcards[3] = type;
        }
        else if (skillcards.Count < 4)
        {
            skillcards.Add(type);
        }
    }
}