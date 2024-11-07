using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{
    private static EndManager instance;
    public static EndManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public Image Panel;
    public TextMeshProUGUI Result;
    public TextMeshProUGUI WinText;
    public TextMeshProUGUI DefeatText;
    public Button Menu;
    void Awake()
    {
        instance = this;
        Panel.enabled = false;
        WinText.enabled = false;
        DefeatText.enabled = false;
        Menu.gameObject.SetActive(false);
    }

    public void Win()
    {
        Panel.enabled = true;
        Menu.gameObject.SetActive(true);
        Result.text = "½Â¸®!";
        WinText.enabled = true;
        Time.timeScale = 0f;
    }

    public void Defeat()
    {
        Panel.enabled = true;
        Menu.gameObject.SetActive(true);
        Result.text = "ÆÐ¹è";
        DefeatText.enabled = true;
        Time.timeScale = 0f;
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
