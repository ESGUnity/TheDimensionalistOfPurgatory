using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class A10003 : AstralBody
{
    public GameObject A10003Effect;
    private void OnDestroy()
    {
        GameObject go = Instantiate(A10003Effect);
        go.transform.position = transform.position;
        DamageAllAstralBody();
        Destroy(go, 1f);
    }

    void DamageAllAstralBody()
    {
        if (gameObject.tag == "Ally")
        {
            if (GameManager.Instance.OpponentAstralBody.Count != 0)
            {
                for (int i = 0; i < GameManager.Instance.OpponentAstralBody.Count; i++)
                {
                    GameManager.Instance.OpponentAstralBody[i].GetComponent<AstralBody>().Damaged(AbilityValue, gameObject); // 무작위 모든 적!
                }

            }
        }
        else
        {
            if (GameManager.Instance.PlayerAstralBody.Count != 0)
            {
                for (int i = 0; i < GameManager.Instance.PlayerAstralBody.Count; i++)
                {
                    GameManager.Instance.PlayerAstralBody[i].GetComponent<AstralBody>().Damaged(AbilityValue, gameObject); // 무작위 모든 적에게 피해!
                }

            }
        }
    }
}
