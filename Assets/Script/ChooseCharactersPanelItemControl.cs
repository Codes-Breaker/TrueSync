using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ChooseCharactersPanelItemControl : MonoBehaviour
{
    public InputReaderBase inputReader;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI playerIndexText;
    private bool isButtonDown;
    private int playerIndex;
    public Image background;
    public CharacterType characterType = CharacterType.PolarBear;
    public bool isReady = false;
    public List<Sprite> images = new List<Sprite>();
    public Image avatar;

    private void Awake()
    {

    }

    public void Init(InputDevice inputDevice, int playerIndex)
    {
        var go = new GameObject("InputReader");
        inputReader = go.AddComponent<InputReaderBase>();
        inputReader.Init(inputDevice);
        this.playerIndex = playerIndex;
        playerIndexText.text = $"P{playerIndex}";
        background.color = InputReadManager.Instance.playerColors[playerIndex - 1];
        avatar.sprite = images[(int)characterType - 1];
    }

    private void LateUpdate()
    {
        if(inputReader.interact && !isButtonDown)
        {
            //var index = (int)characterType;
            //index = index + 1;
            //if (index > 3)
            //    index = 1;
            //characterType = (CharacterType)index;
            isReady = !isReady;
            isButtonDown = true;
        }
        if (!inputReader.interact)
            isButtonDown = false;
        //if (characterType == CharacterType.Seal)
        //    characterNameText.text = "PolarBear";
        //else if (characterType == CharacterType.PolarBear)
        //    characterNameText.text = "PolarBear";
        //else if (characterType == CharacterType.SnowFox)
        //    characterNameText.text = "SnowFox";

        characterNameText.text = isReady ? "READY" : "NOT READY";
        avatar.sprite = images[(int)characterType];
    }
}
