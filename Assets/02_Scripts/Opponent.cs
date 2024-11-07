using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Opponent : MonoBehaviour
{
    private static Opponent instance;
    public static Opponent Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public int MaxHealth;
    public int CurrentHealth;
    public int CurrentEssence;
    public Slider HealthUI;
    public TextMeshProUGUI HealthText;
    public CopyDataBase WholeCards;
    public List<CardData> OpponentCards;
    public List<Vector3Int> FilledCell;

    bool placeDone;

    private void Awake()
    {
        instance = this;
        MaxHealth = 20;
        CurrentHealth = MaxHealth;
        CurrentEssence = 8;
        FilledCell = new List<Vector3Int>();
        OpponentCards = new List<CardData>();
        placeDone = false;
    }
    void Start()
    {
        InitializeDeck();
        ShuffleList(OpponentCards);
    }

    void Update()
    {
        HealthUI.value = (float)CurrentHealth / MaxHealth;
        HealthText.text = CurrentHealth.ToString();
        AstralBodyPlace();
    }

    public void Damaged(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            EndManager.Instance.Win();
        }
    }
    public void InitializeDeck() // AI�� ���� �ʱ�ȭ�ϴ� �޼���
    {
        foreach (CardData card in WholeCards.CardDataList)
        {
            if (card.IsCardChosenByOpponent)
            {
                CardData ChosenCard = new CardData();
                ChosenCard = card;
                OpponentCards.Add(ChosenCard);
            }
        }
    }
    public void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        for (int i = 0; i < list.Count; i++)
        {
            // ������ �ε����� �����Ͽ� ���� ��ҿ� ��ȯ
            int randomIndex = random.Next(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    public void AstralBodyPlace()
    {
        if (GameManager.Instance.phase == GameManager.Phase.Preparation && !placeDone)
        {
            
            System.Random random = new System.Random();
            CardData drawCard = null;
            for (int j = 0; j < OpponentCards.Count; j++)
            {
                Debug.Log("����Ǵ���üũ");
                drawCard = OpponentCards[j];

                if (drawCard.Cost <= GameManager.Instance.LimitEssence && drawCard.Cost <= CurrentEssence)
                {
                    Vector3Int grid = new Vector3Int(0, 0, 4);
                    
                    while (FilledCell.Contains(grid)) // �������� �׸��� �����ֱ�
                    {
                        int randomCol = random.Next(1, 5);
                        int randomRow = random.Next(-5, 5);
                        grid = new Vector3Int(randomRow, 0, randomCol);

                        if (drawCard.Range == 4)
                        {
                            for (int i = -5; i < 5; i++)
                            {
                                grid = new Vector3Int(i, 0, 4); // ��Ÿ��� �� �ֵ��� �ڿ� ��ġ

                                if (!FilledCell.Contains(grid))
                                {
                                    break;
                                }
                            }

                            for (int i = -5; i < 5; i++)
                            {
                                grid = new Vector3Int(i, 0, 3); // ��Ÿ��� �� �ֵ��� �ڿ� ��ġ

                                if (!FilledCell.Contains(grid))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    // �ռ� ���� �׸��忡 ������Ʈ ����
                    GameObject go = Instantiate(drawCard.Prefab); // ����
                    go.transform.position = grid;
                    go.transform.GetChild(0).eulerAngles = new Vector3(0, 180, 0);
                    FilledCell.Add(grid); // �̹� ��ġ�� �κ� ǥ��
                    GameManager.Instance.OpponentAstralBody.Add(go.transform.GetChild(0).gameObject); // ���� �޴����� �� �÷��̾��� ��ü ���� �Ǵ��ϱ� ���� �ڵ�
                    go.transform.GetChild(0).tag = "Enemy"; // �±� ����
                    go.transform.GetChild(0).GetComponent<AstralBody>().TargetTag = "Ally";
                    CurrentEssence -= drawCard.Cost;
                }
            }
            ShuffleList(OpponentCards);
            placeDone = true;
        }

        if (GameManager.Instance.phase == GameManager.Phase.WaitingBeforePreparation)
        {
            placeDone = false;
            FilledCell.Clear();
        }

    }
}
