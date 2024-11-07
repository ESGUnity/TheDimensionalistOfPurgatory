using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public TextMeshProUGUI Warning;
    public Player player;
    public Opponent opponent;
    public Slider TimeBar;
    public TextMeshProUGUI LimitEs, MaxEs;
    public int MaxEssence;
    public int CurrentEssence;
    public int LimitEssence;
    public int CurrentRound;
    public float PreparationTime;
    public float BattleTime;
    public float WaitingTime;
    [SerializeField]
    public List<GameObject> PlayerAstralBody = new();
    [SerializeField]
    public List<GameObject> OpponentAstralBody = new();
    public enum Phase { Preparation, Battle, WaitingBeforePreparation, WaitingBeforeBattle };
    public Phase phase;
    int EndBattleDamage;

    private void Awake()
    {
        PreparationTime = 20f;
        BattleTime = 40f;
        WaitingTime = 3f;
        MaxEssence = 6; // �ڷ�ƾ�� ���۵� �� 2�� �߰������� ���� �������� 6���� �Ѵ�.
        CurrentEssence = MaxEssence;
        LimitEssence = 3; // �� ������ ��������.
        EndBattleDamage = 0;
        phase = Phase.WaitingBeforePreparation;
        instance = this;
    }
    void Start()
    {
        StartCoroutine("PreparationTerm");
    }

    void Update()
    {
        PlayerAstralBody.RemoveAll(item => item == null); // Destroy�� ���� ������Ʈ �ʱ�ȭ
        OpponentAstralBody.RemoveAll(item => item == null);
        LimitEs.text = $"������ �Ѱ� : {LimitEssence}";
        MaxEs.text = $"��� ���� ������ : {CurrentEssence}/{MaxEssence}";
    }

    IEnumerator WaitingTermBeforePreparation() 
    {
        phase = Phase.WaitingBeforePreparation;
        float remainTime = WaitingTime;

        if (PlayerAstralBody.Count == 0 || OpponentAstralBody.Count == 0)
        {
            if (PlayerAstralBody.Count < OpponentAstralBody.Count)
            {
                player.Damaged(EndBattleDamage);
            }
            else if (PlayerAstralBody.Count > OpponentAstralBody.Count)
            {
                opponent.Damaged(EndBattleDamage);
            }
        }

        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            TimeBar.value = remainTime / WaitingTime;
            yield return null;
        }

        StartCoroutine("PreparationTerm");
        yield break;
    }
    IEnumerator PreparationTerm()
    {
        phase = Phase.Preparation;
        MaxEssence += 2;
        LimitEssence += 1;
        CurrentEssence = MaxEssence;
        Debug.Log(player.CurrentEssence.ToString());
        Player.Instance.CurrentEssence = MaxEssence;  // �� �÷��̾��� ������ �ʱ�ȭ
        Opponent.Instance.CurrentEssence = MaxEssence;
        float remainTime = PreparationTime;
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            TimeBar.value = remainTime / PreparationTime;
            yield return null;
        }
        
        StartCoroutine("WaitingTermBeforeBattle");
        yield break;
    }

    IEnumerator WaitingTermBeforeBattle()
    {
        phase = Phase.WaitingBeforeBattle;
        float remainTime = WaitingTime;
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            TimeBar.value = remainTime / WaitingTime;
            yield return null;
        }
        
        StartCoroutine("BattleTerm");
        yield break;
    }
    IEnumerator BattleTerm()
    {
        phase = Phase.Battle;
        float remainTime = BattleTime;
        while (remainTime > 0)
        {
            if (PlayerAstralBody.Count == 0 || OpponentAstralBody.Count == 0)
            {
                EndBattleDamage = Mathf.Abs(PlayerAstralBody.Count - OpponentAstralBody.Count); // ���� ������ �� ���� ��ü ����ŭ �������� �ִ� ��.
                phase = Phase.WaitingBeforePreparation;
                StartCoroutine("WaitingTermBeforePreparation");
                remainTime = 0;
                yield break;
            }
            remainTime -= Time.deltaTime;
            TimeBar.value = remainTime / BattleTime;
            yield return null;
        }
        
        StartCoroutine("WaitingTermBeforePreparation");
        yield break;
    }

}
