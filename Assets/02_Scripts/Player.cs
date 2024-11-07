using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private static Player instance;
    public static Player Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public int MaxHealth;
    public int CurrentHealth;
    public int CurrentEssence;
    public Slider HealthUI;
    public TextMeshProUGUI HealthText;
    private void Awake()
    {
        instance = this;
        MaxHealth = 20;
        CurrentHealth = MaxHealth;
        CurrentEssence = 8;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }


    void Update()
    {
        HealthUI.value = (float)CurrentHealth / MaxHealth;
        HealthText.text = CurrentHealth.ToString();
    }

    public void Damaged(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            EndManager.Instance.Defeat();
        }
    }

}
