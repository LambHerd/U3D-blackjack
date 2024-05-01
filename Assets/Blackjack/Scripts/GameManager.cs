using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public TMP_InputField BetNumberInput;
    public GameObject BetPanel;

    public GameObject Chip_1, Chip_5, Chip_10;
    public GameObject Chip_pos,Bet_Chip_pos;

    List<GameObject> coins = new List<GameObject>();


    private GameState CurrentState {
        get {
            return _currentState;
        }
        set {
            if (_currentState != value) {
                _currentState = value;

                if (OnGameStateChanged != null) {
                    OnGameStateChanged(_currentState);
                }
            }
        }
    }

    private GameAction CurrentAction {
        get {
            return _currentAction;
        }
        set {
            if (_currentAction != value) {
                _currentAction = value;

                if (OnGameActionChanged != null) {
                    OnGameActionChanged(_currentAction);
                }
            }
        }
    }

    private void Start()
    {
        Subscriptions();

        OnNewGameEvent();

        BetBtn.onClick.AddListener(OnBetBtnClick);
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
        if (_computerTurnCoroutine != null) {
            StopCoroutine(_computerTurnCoroutine);
        }

        SceneManager.LoadScene("Menu");
    }

    private IEnumerator OnComputerTurn()
    {
        yield return _computer.TurnWaitForSeconds;

        _computer.UpdateBehaviour(_human.Hand);

        if (_computer.IsHitting) {
            _dealer.GiveCard(_computer);
        }

        EvaluateHands(GameState.HumanTurn);
    }

    private void EvaluateHands(GameState nextState)
    {
        bool moveToNextState = false;

        int computerTotalValue = _computer.Hand.TotalValue;
        int humanTotalValue = _human.Hand.TotalValue;

        if (_computer.IsHitting || _human.IsHitting) {
            if (humanTotalValue == 21 && computerTotalValue == 21) {
                CurrentState = GameState.Draw;
            }
            else if (computerTotalValue > 21 || humanTotalValue == 21 || (humanTotalValue > computerTotalValue && !_computer.IsHitting)) {
                CurrentState = GameState.HumanWon;
            }
            else if (humanTotalValue > 21 || computerTotalValue == 21 || (computerTotalValue > humanTotalValue && !_human.IsHitting)) {
                CurrentState = GameState.ComputerWon;
            }
            else {
                moveToNextState = true;
            }
        }
        else if (!_computer.IsHitting && !_human.IsHitting) {
            if (computerTotalValue > humanTotalValue) {
                CurrentState = GameState.ComputerWon;
            }
            else if (humanTotalValue > computerTotalValue) {
                CurrentState = GameState.HumanWon;
            }
            else {
                CurrentState = GameState.Draw;
            }
        }

        if (moveToNextState) {
            if ((nextState == GameState.HumanTurn && _human.IsHitting) || (nextState == GameState.ComputerTurn && !_computer.IsHitting)) {
                CurrentState = GameState.HumanTurn;
                CurrentAction = GameAction.HitAndStand;
            }
            else {
                CurrentState = GameState.ComputerTurn;
                CurrentAction = GameAction.None;

                _computerTurnCoroutine = StartCoroutine(OnComputerTurn());
            }
        }
        else {
            EndGame();
        }
    }

    private void EndGame()//modify
    {
        _computer.Hand.Show();

        if (CurrentState == GameState.HumanWon) {
            _human.Score++;
            human_health += chips;
            devil_health -= chips;
        }
        else if (CurrentState == GameState.ComputerWon) {
            _computer.Score++;
            human_health -= chips;
            devil_health += chips;
        }
        else
        {

        }

        if (human_health <= 0)
        {
            //end:human lose
        }
        else if (devil_health <= 0)
        {
            //end:devil lose
        }
        else
        {
            if (human_health > 100)
            {
                human_health = 100;
            }
            if(devil_health > 100)
            {
                devil_health = 100;
            }
            
            human_health_txt.text = human_health+"";
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
        //int bet = int.Parse(BetNumberInput.text);

        //if (bet > human_health-chips)
        //{
            //chips = human_health;
        //}
        //else if(bet>0)
        //{
            //chips += bet;
        //}
        //human_bet_txt.text = "bet: " + chips;
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
            GameObject go = Instantiate(Chip_10, Chip_pos.transform.position+new Vector3(x_pos+110f*i, y_pos,0), Quaternion.identity);
            go.transform.parent=BetPanel.transform;
            go.GetComponent<Button>().onClick.AddListener(() => OnCoinClick(go,10));
            coins.Add(go);
        }

        for (int i = 0; i < fives; i++)
        {
            GameObject go = Instantiate(Chip_5, Chip_pos.transform.position + new Vector3(x_pos + 110f * i, y_pos-120f, 0), Quaternion.identity);
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

    void ClearCoins()
    {
        for (int i = 0;i < coins.Count; i++)
        {
            Destroy(coins[i]);
        }
        coins.Clear();
    }

    void OnCoinClick(GameObject go,int coin)
    {
        go.transform.parent= BetPanel.transform;
        go.GetComponent<MoveToBet>().StartBet(Bet_Chip_pos.transform.position);
        Bet_Chip_pos.transform.position += new Vector3(50f, 0, 0);
        chips += coin;
        human_bet_txt.text = "bet: " + chips;
    }
}