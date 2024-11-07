using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardDataBase : ScriptableObject
{
    [field: SerializeField]
    public List<CardData> CardDataList { get; private set; }
}

[Serializable]
public class CardData
{
    [field: SerializeField]
    public int Id { get; private set; }
    [field: SerializeField]
    public int Cost { get; private set; }
    [field: SerializeField]
    public int Health { get; private set; }
    [field: SerializeField]
    public int Damage { get; private set; }
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public string Ability { get; private set; }
    [field: SerializeField]
    public string AttackSpeed { get; private set; }
    [field: SerializeField]
    public string Mana { get; private set; }
    [field: SerializeField]
    public float Range { get; private set; }
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
    [field: SerializeField]
    public Sprite Thumbnail { get; private set; }
    [field: SerializeField]
    public bool IsCardChosenByPlayer { get; private set; }
    [field: SerializeField]
    public bool IsCardChosenByOpponent { get; private set; }
    [field: SerializeField]
    public AudioClip SpawnSpeech { get; private set; }
    public bool IsInDeck;
}
