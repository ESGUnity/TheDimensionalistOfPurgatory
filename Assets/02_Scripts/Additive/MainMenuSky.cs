using UnityEngine;

public class MainMenuSky : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Y = transform.eulerAngles;
        Y.y -= 5 * Time.deltaTime;
        transform.eulerAngles = Y;
    }
}
