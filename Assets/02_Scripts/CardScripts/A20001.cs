using UnityEngine;

public class A20001 : AstralBody
{
    public void FullnessStun() // Ŭ���� ����� �Լ�
    {
        if (gameObject.tag == "Ally")
        {
            if (GameManager.Instance.OpponentAstralBody.Count != 0)
            {
                GameManager.Instance.OpponentAstralBody[0].GetComponent<AstralBody>().Stuned(AbilityValue); 
            }
        }
        else
        {
            if (GameManager.Instance.PlayerAstralBody.Count != 0)
            {
                GameManager.Instance.PlayerAstralBody[0].GetComponent<AstralBody>().Stuned(AbilityValue); 
            }
        }
    }
}
