using UnityEngine;

public class A10002 : AstralBody
{
    public GameObject Projectile10002;
    
    public void FullnessAttack() // 클립에 넣을 함수
    {
        GameObject go = Instantiate(Projectile10002);
        go.transform.position = transform.position;
        go.GetComponent<RangerProjectile>().PenetratingAttack(gameObject, target, AbilityValue, true);
    }
}
