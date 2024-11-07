using UnityEngine;

public class A20002 : AstralBody
{
    public void OnDestroy()
    {
        if (gameObject.tag == "Ally")
        {
            if (GameManager.Instance.PlayerAstralBody.Count != 0)
            {
                if (GameManager.Instance.PlayerAstralBody[0] == gameObject) // 혹시 버프를 주려는게 본인인지 먼저 확인 // 아군 버프 줄땐 체크를 2번 하자
                {
                    if (GameManager.Instance.PlayerAstralBody[1] != null)
                    {
                        GameManager.Instance.PlayerAstralBody[1].GetComponent<AstralBody>().Invincible(AbilityValue); // 무작위 아군!
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    GameManager.Instance.PlayerAstralBody[0].GetComponent<AstralBody>().Invincible(AbilityValue); // 무작위 아군!
                }
            }
        }
        else
        {
            if (GameManager.Instance.OpponentAstralBody.Count != 0)
            {
                if (GameManager.Instance.OpponentAstralBody[0] == gameObject) // 아군 버프 줄땐 체크를 2번 하자
                {
                    if (GameManager.Instance.OpponentAstralBody[1] != null)
                    {
                        GameManager.Instance.OpponentAstralBody[1].GetComponent<AstralBody>().Invincible(AbilityValue); // 무작위 아군!
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    GameManager.Instance.OpponentAstralBody[0].GetComponent<AstralBody>().Invincible(AbilityValue); // 무작위 아군!
                }
            }
        }
    }
}
