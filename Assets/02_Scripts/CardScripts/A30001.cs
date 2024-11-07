using UnityEngine;

public class A30001 : AstralBody
{

    private void OnDestroy()
    {
        StunAllAstralBody();
    }



    void StunAllAstralBody()
    {
        if (gameObject.tag == "Ally")
        {
            if (GameManager.Instance.OpponentAstralBody.Count != 0)
            {
                for (int i = 0; i < GameManager.Instance.OpponentAstralBody.Count; i++)
                {
                    GameManager.Instance.OpponentAstralBody[i].GetComponent<AstralBody>().Stuned(AbilityValue); // 무작위 모든 적!
                }

            }
        }
        else
        {
            if (GameManager.Instance.PlayerAstralBody.Count != 0)
            {
                for (int i = 0; i < GameManager.Instance.PlayerAstralBody.Count; i++)
                {
                    GameManager.Instance.PlayerAstralBody[i].GetComponent<AstralBody>().Stuned(AbilityValue); // 무작위 모든 적에게 피해!
                }

            }
        }
    }
}
