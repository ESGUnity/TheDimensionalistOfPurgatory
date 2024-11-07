using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetCardInfoManager : MonoBehaviour
{
    public CardData cardData;
    public Image Thumbnail;
    public TextMeshProUGUI Ability, Name, Cost, Health, Mana, Damage, AttackSpeed, Range;
    // 앙대~
    public void SetCardInfoAndGenerate()
    {
        this.Thumbnail.sprite = cardData.Thumbnail;
        Ability.text = cardData.Ability;
        Name.text = cardData.Name;
        Cost.text = cardData.Cost.ToString();
        Health.text = cardData.Health.ToString();
        Mana.text = cardData.Mana;
        Damage.text = cardData.Damage.ToString();
        AttackSpeed.text = cardData.AttackSpeed;
        Range.text = cardData.Range.ToString();
    }
    public void IfYouClickButtonPlz()
    {
        if (cardData.Cost <= GameManager.Instance.LimitEssence && cardData.Cost <= Player.Instance.CurrentEssence) // 코스트가 안되면 클릭 자체를 못하게 막는 기능.  && GameManager.Instance.phase == GameManager.Phase.Preparation
        {
            PlacementManager.Instance.CurrentSelectedCardPrefab = this; // 배치 후 카드 프리팹을 파괴하기 위함.
            PlacementManager.Instance.StartPlacement(cardData.Id);
        }
        else
        {
            StartCoroutine(WarningCost());
            Debug.Log("고코스트내려함");
        }
        
    }

    public void DestroyThis() // 카드를 배치할 때 이 함수를 호출하여 카드 프리팹 제거
    {
        GameManager.Instance.Warning.enabled = false;
        Destroy(gameObject);
    }

    IEnumerator WarningCost()
    {
        yield return null;
        GameManager.Instance.Warning.enabled = true;
        yield return new WaitForSeconds(3f);
        GameManager.Instance.Warning.enabled = false;
    }
}
