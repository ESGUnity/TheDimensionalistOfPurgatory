using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CopyDataBase : MonoBehaviour
{
    public List<CardData> CardDataList;
    public CardDataBase CardData;
    void Awake()
    {
        foreach (CardData card in CardData.CardDataList)
        {
            CardData c = new CardData();
            c = card;
            CardDataList.Add(c);
        }
    }
}
