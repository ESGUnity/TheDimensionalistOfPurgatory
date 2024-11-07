using UnityEngine;

public class SkyBox : MonoBehaviour
{
    float rot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rot += Time.deltaTime;
        transform.localEulerAngles = new Vector3(7f, -240f, rot);
    }
}
