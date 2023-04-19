using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    public GameObject GameOverText;
    public GameObject windText;
    public GameObject windIndicator;
    public GameObject infoBg;
    // Start is called before the first frame update
    void Awake()
    {
        GameOverText?.gameObject.SetActive(false);
        windText?.gameObject.SetActive(false);
        infoBg?.gameObject.SetActive(false);
        windIndicator?.gameObject.SetActive(false);
    }

    public void CheckGameState()
    {
        List<CharacterContorl> characters = GameObject.FindObjectsOfType<CharacterContorl>().ToList();
        if (characters.Count <= 1 || characters.Sum(x => x.isDead ? 0: 1) == 1)
        {
            GameOver();
        }

    }

    private void GameOver()
    {
        GameOverText?.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }
        
    }
}
