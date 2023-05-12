using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Cinemachine;
using TMPro;


public enum CharacterType
{
    Seal =1,
    PolarBear = 2,
    SnowFox = 3,
}

public struct PlayerData
{
    public CharacterType characterType;
    public InputReaderBase inputReader;
}

public class InputReadManager : MonoBehaviour
{
    public ChooseCharactersPanelContorl chooseCharactersPanel;
    public RandomEventsControl randomEventsControl;
    public List<PlayerData> playerDatas = new List<PlayerData>();
    private CinemachineTargetGroup cinemachineTargetGroup;
    public List<int> ExistingIndex = new List<int>();
    public static InputReadManager Instance { get; protected set; }
    public List<Color> playerColors = new List<Color>();

    //预制体Path
    private string sealPath = "Prefabs/bearrig";
    private string polarBearPath = "Prefabs/白熊";
    private string snowFoxPath = "Prefabs/狐狸";

    //现有注册的控制器
    public List<InputDevice> controlDevices = new List<InputDevice>();

    private IDisposable m_ButtonPressListener;
    public float timeLeft = 0;
    public TextMeshProUGUI countdownText;

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            string formattedSeconds = Convert.ToInt32(timeLeft).ToString("D2");
            countdownText.text = formattedSeconds;
        }
        else
        {
            CreatCharacter();
            //randomEventsControl.startCountdown = true;
            m_ButtonPressListener?.Dispose();
            Destroy(chooseCharactersPanel.gameObject);
            Destroy(gameObject);
        }
    }
    private void Awake()
    {
         m_ButtonPressListener = InputSystem.onAnyButtonPress
         .Call(OnButtonPressed);
        Instance = this;
        ExistingIndex = new List<int>();
    }
    void OnButtonPressed(InputControl button)
    {
        if (!Application.isPlaying)
            return;
        var device = button.device;
        if (device is Mouse || device is Keyboard)
            return;
        if (controlDevices.Contains(device))
            return;
        //Debug.Log(device.name);
        controlDevices.Add(device);
        if(chooseCharactersPanel.gameObject.activeSelf)
            chooseCharactersPanel.AddCharacterItem(device, chooseCharactersPanel.chooseCharactersPanelItemControls.Count + 1);


    }
    public void CreatCharacter()
    {
        playerDatas = chooseCharactersPanel.GetCharactersData();
        cinemachineTargetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
        int index = 0;
        var padding = 20f / (playerDatas.Count + 1);
        foreach (var item in playerDatas)
        {
            GameObject characterGameObject;

            switch (item.characterType)
            {
                case CharacterType.Seal:
                    characterGameObject = Instantiate((GameObject)Resources.Load(sealPath));
                    characterGameObject.GetComponent<CharacterContorl>().inputReader = item.inputReader;
                    cinemachineTargetGroup.AddMember(characterGameObject.transform, 2, 4);
                    characterGameObject.transform.position = new Vector3(-10 + padding*(index+1), 10, 0);
                    characterGameObject.GetComponent<CharacterContorl>().playerIndex = index;
                    index++;
                    break;
                case CharacterType.PolarBear:
                    characterGameObject = Instantiate((GameObject)Resources.Load(polarBearPath));
                    characterGameObject.GetComponent<CharacterContorl>().inputReader = item.inputReader;
                    cinemachineTargetGroup.AddMember(characterGameObject.transform, 2, 4);
                    characterGameObject.transform.position = new Vector3(-10 + padding * (index + 1), 6, 0);
                    characterGameObject.GetComponent<CharacterContorl>().playerIndex = index;
                    index++;
                    break;
                case CharacterType.SnowFox:
                    characterGameObject = Instantiate((GameObject)Resources.Load(snowFoxPath));
                    characterGameObject.GetComponent<CharacterContorl>().inputReader = item.inputReader;
                    cinemachineTargetGroup.AddMember(characterGameObject.transform, 2, 4);
                    characterGameObject.transform.position = new Vector3(-10 + padding * (index + 1), 6, 0);
                    characterGameObject.GetComponent<CharacterContorl>().playerIndex = index;
                    index++;
                    break;
            }


        }
    }
}



