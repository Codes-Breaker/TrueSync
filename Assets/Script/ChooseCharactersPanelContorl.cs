using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ChooseCharactersPanelContorl : MonoBehaviour
{
    public GameObject chooseCharactersPanelContent;
    public GameObject chooseCharactersPanelItem;

    public List<ChooseCharactersPanelItemControl> chooseCharactersPanelItemControls = new List<ChooseCharactersPanelItemControl>();

    public void AddCharacterItem(InputDevice device, int playerIndex)
    {
        var item = Instantiate(chooseCharactersPanelItem,chooseCharactersPanelContent.transform);
        item.SetActive(true);
        var itemControl = item.GetComponent<ChooseCharactersPanelItemControl>();
        itemControl.Init(device, playerIndex);
        chooseCharactersPanelItemControls.Add(itemControl);
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
            });
        }

        return returnData;
    }
}
