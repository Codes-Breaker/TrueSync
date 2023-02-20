using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (isEnd)
            return;
        if(characterManagerOne.transform.position.y < -5)
        {
            characterManagerOne.characterCamera.gameObject.SetActive(false);
            characterManagerTwo.characterCamera.gameObject.SetActive(false);
            one.text = "LOSE";
            two.text = "WIN";
            isEnd = true;
        }
        if (characterManagerTwo.transform.position.y < -5)
        {
            characterManagerTwo.characterCamera.gameObject.SetActive(false);
            characterManagerOne.characterCamera.gameObject.SetActive(false);
            one.text = "WIN";
            two.text = "LOSE";
            isEnd = true;
        }
    }
}