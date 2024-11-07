using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    private static CardManager instance;
    public static CardManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    public CopyDataBase WholeCards;
    public List<CardData> PlayerCards;
    public List<CardData> OpponentCards;

    public GameObject CardPrefab;
    public GameObject CardInventoryContent;
    public TextMeshProUGUI RemainCardInDeckText;

    public int inventoryCount = 0;
    int DeckCount = 0;

    private void Awake()
    {
        instance = this;
        PlayerCards = new List<CardData>();
        OpponentCards = new List<CardData>();
    }
    void Start()
    {
        InitializeMyDeck();
        FillDeck();
    }

    // Update is called once per frame
    void Update()
    {
        RemainCardInDeckText.text = $"남은 카드 : {DeckCount}";
        if (DeckCount == 0 && GameManager.Instance.phase == GameManager.Phase.WaitingBeforePreparation)
        {
            FillDeck();
        }
        else if (DeckCount == 0)
        {
            return;
        }

        if (GameManager.Instance.phase == GameManager.Phase.Preparation && inventoryCount < 5)
        {
            DrawCard();
        }
    }

    public void FillDeck() // IsInDeck을 모두 true로.
    {
        foreach (CardData card in PlayerCards)
        {
            card.IsInDeck = true;
        }
        DeckCount = PlayerCards.Count;
    }

    public void InitializeMyDeck() // 플레이어가 만든 덱을 전투 시 가져오는 함수
    {
        if (WholeCards.CardDataList.Count > 0)
        {
            foreach (CardData card in WholeCards.CardDataList)
            {
                if (card.IsCardChosenByPlayer)
                {
                    card.IsInDeck = true;
                    CardData ChosenCard = new CardData();
                    ChosenCard = card;
                    PlayerCards.Add(ChosenCard);
                }
                else
                {
                    card.IsInDeck = false;
                }

            }
        }
    }

    public void DrawCard() // 카드를 뽑을 때마다 카드 프리팹 생성 후 목록에 추가.
    {
        System.Random random = new System.Random();
        int index = -1;

        for (int i = 0; i < PlayerCards.Count; i++)
        {
            int randomIndex = random.Next(0, PlayerCards.Count);
            if (PlayerCards[randomIndex].IsInDeck)
            {
                PlayerCards[randomIndex].IsInDeck = false;
                index = randomIndex;
                break;
            }
        }

        if (index == -1) // 덱의 카드를 다 쓴 경우
        {
            Debug.Log("이거나오면 안 뽑는다는 것");
            return;
        }

        CardData DrawedCard = new CardData();
        DrawedCard = PlayerCards[index];
        GameObject go = Instantiate(CardPrefab, CardInventoryContent.transform);
        go.GetComponent<SetCardInfoManager>().cardData = DrawedCard;
        go.GetComponent<SetCardInfoManager>().SetCardInfoAndGenerate();
        inventoryCount++;
        DeckCount--;
    }
}
