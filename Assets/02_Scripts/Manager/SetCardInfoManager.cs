using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetCardInfoManager : MonoBehaviour
{
    public CardData cardData;
    public Image Thumbnail;
    public TextMeshProUGUI Ability, Name, Cost, Health, Mana, Damage, AttackSpeed, Range;
    // �Ӵ�~
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
        if (cardData.Cost <= GameManager.Instance.LimitEssence && cardData.Cost <= Player.Instance.CurrentEssence) // �ڽ�Ʈ�� �ȵǸ� Ŭ�� ��ü�� ���ϰ� ���� ���.  && GameManager.Instance.phase == GameManager.Phase.Preparation
        {
            PlacementManager.Instance.CurrentSelectedCardPrefab = this; // ��ġ �� ī�� �������� �ı��ϱ� ����.
            PlacementManager.Instance.StartPlacement(cardData.Id);
        }
        else
        {
            StartCoroutine(WarningCost());
            Debug.Log("���ڽ�Ʈ������");
        }
        
    }

    public void DestroyThis() // ī�带 ��ġ�� �� �� �Լ��� ȣ���Ͽ� ī�� ������ ����
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
