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
    public List<Sprite> playerIndicatorSprites = new List<Sprite>();
    public GameController gameController;
    //预制体Path
    private string sealPath = "Prefabs/bearrig";
    private string polarBearPath = "Prefabs/bearrig";
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
            gameController.StartGame();
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
        var randomPointIndex = new List<int>();
        for (int i = 0; i < gameController.bornPointsParent.transform.childCount; i++)
        {
            randomPointIndex.Add(i);
        }
        foreach (var item in playerDatas)
        {
            GameObject characterGameObject = null;
            var posRandIndex = randomPointIndex[UnityEngine.Random.Range(0, randomPointIndex.Count)];
            randomPointIndex.Remove(posRandIndex);
            var pos = gameController.bornPointsParent.transform.GetChild(posRandIndex).position;
            switch (item.characterType)
            {
                case CharacterType.Seal:
                    characterGameObject = Instantiate((GameObject)Resources.Load(sealPath));
                    index++;
                    break;
                case CharacterType.PolarBear:
                    characterGameObject = Instantiate((GameObject)Resources.Load(polarBearPath));
                    index++;
                    break;
                case CharacterType.SnowFox:
                    characterGameObject = Instantiate((GameObject)Resources.Load(snowFoxPath));
                    index++;
                    break;
            }
            if (characterGameObject != null)
            {
                characterGameObject.GetComponent<CharacterContorl>().inputReader = item.inputReader;
                cinemachineTargetGroup.AddMember(characterGameObject.transform, 2, 4);
                characterGameObject.transform.position = pos;
                characterGameObject.GetComponent<CharacterContorl>().playerIndex = index;
            }

        }
    }
}



