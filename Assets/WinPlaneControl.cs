using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPlaneControl : MonoBehaviour
{
    public Text one;
    public Text two;

    public CharacterManager characterManagerOne;
    public CharacterManager characterManagerTwo;

    private bool isEnd  = false;


    private void Awake()
    {
        one.text = "";
        two.text = "";
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (isEnd)
            return;
        if(characterManagerOne.bodyCollider.transform.position.y < -0.5f)
        {
            characterManagerOne.characterCamera.gameObject.SetActive(false);
            characterManagerTwo.characterCamera.gameObject.SetActive(false);
            one.text = "LOSE";
            two.text = "WIN";
            isEnd = true;
        }
        if (characterManagerTwo.bodyCollider.transform.position.y < -0.5f)
        {
            characterManagerTwo.characterCamera.gameObject.SetActive(false);
            characterManagerOne.characterCamera.gameObject.SetActive(false);
            one.text = "WIN";
            two.text = "LOSE";
            isEnd = true;
        }
    }
}
