using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Dealer : MonoBehaviour
{
    private Deck _deck;
    private Queue<Card> _cardsQueue;
    Card[] cards;
    [SerializeField]
    public PoolingSystem _poolingSystem;
    private void Awake()
    {
        _deck = new Deck();
    }

    public void Reset(params Player[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Reset(_poolingSystem);
        }

        _cardsQueue = new Queue<Card>();

        cards = _deck.GetCards();

        for (int i = 0; i < cards.Length; i++)
        {
            _cardsQueue.Enqueue(cards[i]);
        }
    }
    public List<CardDisplay> newCards;

    //等选择了1张后，清除那三张卡片
    public void Remove3Cards(Player player, CardDisplay display)
    {
        for (int i = 0; i < newCards.Count; i++)
        {
            if (newCards[i] != display)
            {
                _poolingSystem.Enqueue(newCards[i]);
                Vector3 position = new Vector3(20, 20, 20);
                //将不要的卡片移走
                newCards[i].transform.position = position;
            }
            else
            {
                display.transform.SetParent(player.transform);
                //removecard方法为新加的，去掉原先最后一张牌方便添加新牌
                player.Hand.RemoveCard(_poolingSystem);
                player.Hand.AddCard(display);
            }

        }
        newCards.Clear();
    }

    //生成3张新的卡片，
    public void ThreenewCards(Player player)
    {
        //卡片位置
        Vector3 position = player.Transform.position;
        position.y += 0.4f;
        position.z -= 0.7f;

        for (int i = 0; i < 3; i++)
        {
            //Card card = cards[index];
            //CardDisplay display = Resources.Load<CardDisplay>("CardDisplay");
            Card card = GetCard();
            CardDisplay display = _poolingSystem.Dequeue(player.transform, position, player.transform.rotation);

            display.SetCard(card);
            display.FaceUp();

            position.z += 0.3f;
            newCards.Add(display);
            //3张卡片不属于玩家的子物体
            display.transform.SetParent(null);
        }
    }

    public void Deal(Player player, bool showFirstCard = true)
    {
        for (int i = 0; i < 2; i++)
        {
            GiveCard(player, showFirstCard);
            //Apply 'false' for the first card only.
            showFirstCard = true;
        }
    }

    public void GiveCard(Player player, bool showCard = true)
    {
        Card card = GetCard();

        CardDisplay display = GetDisplay(player.Transform);
        display.SetCard(card);

        player.Hand.AddCard(display);

        if (showCard)
        {
            display.FaceUp();
        }
    }

    private Card GetCard()
    {
        return _cardsQueue.Dequeue();
    }

    private CardDisplay GetDisplay(Transform parent)
    {
        Vector3 position = parent.position;
        position.z -= parent.childCount * 0.1f;

        CardDisplay display = _poolingSystem.Dequeue(parent, position, parent.rotation);

        return display;
    }
}