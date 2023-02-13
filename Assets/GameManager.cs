using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    TrueSyncManager trueSyncManager;

    // Start is called before the first frame update
    void Start()
    {
        trueSyncManager = GameObject.Find("TrueSyncManager").GetComponent<TrueSyncManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 80, 20), "重新开始")){
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

        if (GUI.Button(new Rect(20, 80, 80, 20), "退出")){
            Application.Quit();
        }
    }
}
