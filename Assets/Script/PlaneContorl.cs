using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneContorl : MonoBehaviour
{
    // Start is called before the first frame update
    float scale = 0.05f;
    private float timer = 0;
    private float delayTime = 1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        timer += Time.deltaTime;
        if (timer >= delayTime)
        {
            transform.localScale =Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x - scale, transform.localScale.y, transform.localScale.z - scale),0.1f);
            timer = 0;
        }
    }
}
