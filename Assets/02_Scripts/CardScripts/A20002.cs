using UnityEngine;

public class A20002 : AstralBody
{
    public void OnDestroy()
    {
        if (gameObject.tag == "Ally")
        {
            if (GameManager.Instance.PlayerAstralBody.Count != 0)
            {
                if (GameManager.Instance.PlayerAstralBody[0] == gameObject) // Ȥ�� ������ �ַ��°� �������� ���� Ȯ�� // �Ʊ� ���� �ٶ� üũ�� 2�� ����
                {
                    if (GameManager.Instance.PlayerAstralBody[1] != null)
                    {
                        GameManager.Instance.PlayerAstralBody[1].GetComponent<AstralBody>().Invincible(AbilityValue); // ������ �Ʊ�!
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    GameManager.Instance.PlayerAstralBody[0].GetComponent<AstralBody>().Invincible(AbilityValue); // ������ �Ʊ�!
                }
            }
        }
        else
        {
            if (GameManager.Instance.OpponentAstralBody.Count != 0)
            {
                if (GameManager.Instance.OpponentAstralBody[0] == gameObject) // �Ʊ� ���� �ٶ� üũ�� 2�� ����
                {
                    if (GameManager.Instance.OpponentAstralBody[1] != null)
                    {
                        GameManager.Instance.OpponentAstralBody[1].GetComponent<AstralBody>().Invincible(AbilityValue); // ������ �Ʊ�!
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    GameManager.Instance.OpponentAstralBody[0].GetComponent<AstralBody>().Invincible(AbilityValue); // ������ �Ʊ�!
                }
            }
        }
    }
}
