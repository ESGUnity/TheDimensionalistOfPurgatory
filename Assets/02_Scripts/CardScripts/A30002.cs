using UnityEngine;

public class A30002 : AstralBody
{
    void FullnessAbility() // Ŭ������ ȣ���ϴ� �Լ�
    {
        GameObject clone = Instantiate(gameObject.transform.parent.gameObject);
        clone.transform.GetChild(0).transform.position = transform.position + new Vector3(1, 0, 0);
        clone.GetComponentInChildren<AstralBody>().CurrentHealth = CurrentHealth;

        if (gameObject.tag == "Ally")
        {
            GameManager.Instance.PlayerAstralBody.Add(clone.transform.GetChild(0).gameObject);
        }
        else
        {
            GameManager.Instance.OpponentAstralBody.Add(clone.transform.GetChild(0).gameObject);
        }
        
    }
}
