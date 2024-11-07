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
        RemainCardInDeckText.text = $"���� ī�� : {DeckCount}";
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

    public void FillDeck() // IsInDeck�� ��� true��.
    {
        foreach (CardData card in PlayerCards)
        {
            card.IsInDeck = true;
        }
        DeckCount = PlayerCards.Count;
    }

    public void InitializeMyDeck() // �÷��̾ ���� ���� ���� �� �������� �Լ�
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

    public void DrawCard() // ī�带 ���� ������ ī�� ������ ���� �� ��Ͽ� �߰�.
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

        if (index == -1) // ���� ī�带 �� �� ���
        {
            Debug.Log("�̰ų����� �� �̴´ٴ� ��");
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
