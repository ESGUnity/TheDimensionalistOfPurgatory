using UnityEngine;

public class A10001 : AstralBody
{
    public void OnDestroy()
    {
        if (gameObject.tag == "Ally")
        {
            if (GameManager.Instance.OpponentAstralBody.Count != 0)
            {
                GameManager.Instance.OpponentAstralBody[0].GetComponent<AstralBody>().Stigmaed(AbilityValue); // 무작위 적!
            }
        }
        else
        {
            if (GameManager.Instance.PlayerAstralBody.Count != 0)
            {
                GameManager.Instance.PlayerAstralBody[0].GetComponent<AstralBody>().Stigmaed(AbilityValue); // 무작위 적에게 낙인!
            }
        }
    }
}
