using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ChooseCharactersPanelContorl : MonoBehaviour
{
    public GameObject chooseCharactersPanelContent;
    public GameObject chooseCharactersPanelItem;
    public TMP_Text modetext;
    private GameController gameController;
    public List<ChooseCharactersPanelItemControl> chooseCharactersPanelItemControls = new List<ChooseCharactersPanelItemControl>();
    public Button modeButton;
    public List<int> selectedColors = new List<int>();
    public void AddCharacterItem(InputDevice device, int playerIndex)
    {
        var item = Instantiate(chooseCharactersPanelItem,chooseCharactersPanelContent.transform);
        item.SetActive(true);
        var itemControl = item.GetComponent<ChooseCharactersPanelItemControl>();
        itemControl.Init(device, playerIndex, this);
        chooseCharactersPanelItemControls.Add(itemControl);
        selectedColors.Add(itemControl.control.index);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            SwitchMode();
        }
    }

    private void Awake()
    {
        gameController = GameObject.FindObjectOfType<GameController>();
        SetText();
        modeButton.onClick.AddListener(SwitchMode);
    }

    private void SwitchMode()
    {
        gameController.gameMode = gameController.gameMode == GameMode.Single ? GameMode.Multiple : GameMode.Single;
        SetText();
    }

    private void SetText()
    {
        modetext.text = gameController.gameMode == GameMode.Single ? "Solo" : "Team";
    }


    public List<PlayerData> GetCharactersData()
    {
        List<PlayerData> returnData = new List<PlayerData>();
        foreach(var item in chooseCharactersPanelItemControls)
        {
            returnData.Add(new PlayerData
            {
                inputReader = item.inputReader,
                characterType = item.characterType,
                H = item.control.H,
                S = item.control.S,
                furIndex = item.control.index
            });
        }

        return returnData;
    }
}
