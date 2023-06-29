using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;


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
    public float H;
    public float S;
    public int furIndex;
}

public class InputReadManager : MonoBehaviour
{
    public ChooseCharactersPanelContorl chooseCharactersPanel;
    public RandomEventsControl randomEventsControl;
    public List<PlayerData> playerDatas = new List<PlayerData>();
    private CinemachineTargetGroup cinemachineTargetGroup;
    private List<CinemachineTargetGroup> cinemachineTargetGroups = new List<CinemachineTargetGroup>();
    private List<Camera> cameras = new List<Camera>();
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
    private float currentTimeLeft;
    private bool isAllReady = false;
    public TextMeshProUGUI countdownText;

    
    private bool isSplitScreen = false;
    public Button splitScreenButton;
    public TextMeshProUGUI splitScreenText;
    public GameObject splitScreenImage;
    private void Awake()
    {
         m_ButtonPressListener = InputSystem.onAnyButtonPress
         .Call(OnButtonPressed);
        Instance = this;
        ExistingIndex = new List<int>();
        splitScreenButton.onClick.AddListener(switchSplitScreenMode);
        splitScreenText.text = "same screen";
        isSplitScreen = false;
        splitScreenImage.SetActive(false);
    }

    void Update()
    {
        if (chooseCharactersPanel.chooseCharactersPanelItemControls.Count > 0)
            isAllReady = true;
        else
            isAllReady = false;
        foreach(var item in chooseCharactersPanel.chooseCharactersPanelItemControls)
        {
            isAllReady = isAllReady && item.isReady;
        }
        if (isAllReady)
        {
           // countdownText.gameObject.SetActive(true);
            if (currentTimeLeft > 0)
            {
                currentTimeLeft -= Time.deltaTime;
                string formattedSeconds = Convert.ToInt32(currentTimeLeft).ToString("D2");
                countdownText.text = formattedSeconds;
            }
            else
            {
                SetSplitScreenCamera();
                CreatCharacter();
                gameController.StartGame();
                //randomEventsControl.startCountdown = true;
                m_ButtonPressListener?.Dispose();
                Destroy(chooseCharactersPanel.gameObject);
                Destroy(gameObject);
            }
        }
        else
        {
            currentTimeLeft = timeLeft;
            string formattedSeconds = Convert.ToInt32(currentTimeLeft).ToString("D2");
            countdownText.text = formattedSeconds;
            // countdownText.gameObject.SetActive(false);
        }
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
                characterGameObject.name = $"P{index}";
                characterGameObject.GetComponent<CharacterContorl>().inputReader = item.inputReader;
                characterGameObject.GetComponent<CharacterContorl>().SetColor(item.H, item.S);
                characterGameObject.GetComponent<CharacterContorl>().SetFurColor(item.furIndex);
                //foreach(var cinemachineItem in cinemachineTargetGroups)
                //{
                //    cinemachineItem.AddMember(characterGameObject.transform, 2, 4);

                //}
                if (isSplitScreen)
                {
                    characterGameObject.GetComponent<CharacterContorl>().cinemachineTargetGroup = cinemachineTargetGroups[index - 1];
                    characterGameObject.GetComponent<CharacterContorl>().m_camera = cameras[index -1];
                    cinemachineTargetGroups[index - 1].AddMember(characterGameObject.transform, 2, 4);

                }
                else
                {
                    cinemachineTargetGroup.AddMember(characterGameObject.transform, 2, 4);

                }

                characterGameObject.transform.position = pos;
                characterGameObject.GetComponent<CharacterContorl>().playerIndex = index;
            }

        }
    }

    private void switchSplitScreenMode()
    {
        isSplitScreen = !isSplitScreen;
        splitScreenText.text = isSplitScreen ? "split screen" : "same screen";
        gameController.screenMode = isSplitScreen ? ScreenMode.Split : ScreenMode.Same;
    }


    private void SetSplitScreenCamera()
    {
        if (!isSplitScreen)
            return;
        var cameraGameObject = gameController.mainCamera.gameObject;
        var cinemachineTargetGroupGameObject = GameObject.FindObjectOfType<CinemachineTargetGroup>().gameObject;

        if (chooseCharactersPanel.GetCharactersData().Count == 2)
        {
            var cameraOneGameObject = GameObject.Instantiate(cameraGameObject);
            var cameraOne = cameraOneGameObject.GetComponent<Camera>();
            GameObject.Destroy(cameraOneGameObject.GetComponent<AudioListener>());
            //相机一设置
            cameraOne.rect = new Rect(0,0,0.5f,1);
            cameraOne.cullingMask |= 1 << 10;
            cinemachineTargetGroupGameObject.layer = 10;
            for(int i = 0;i<cinemachineTargetGroupGameObject.transform.childCount;i++)
            {
                cinemachineTargetGroupGameObject.transform.GetChild(i).gameObject.layer = 10;
            }
            cinemachineTargetGroups.Add(cinemachineTargetGroupGameObject.GetComponent<CinemachineTargetGroup>());
            cameras.Add(cameraOne);
            cameraOneGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);

            //相机二设置
            var cameraTwoGameObject = GameObject.Instantiate(cameraGameObject);
            var targetGroupTwo = GameObject.Instantiate(cinemachineTargetGroupGameObject);
            GameObject.Destroy(cameraTwoGameObject.GetComponent<AudioListener>());
            var cameraTwo = cameraTwoGameObject.GetComponent<Camera>();
            cameraTwo.rect = new Rect(0.5f, 0, 0.5f, 1);
            cameraTwo.cullingMask |= 1 << 11;
            targetGroupTwo.layer = 11;
            for (int i = 0; i < targetGroupTwo.transform.childCount; i++)
            {
                targetGroupTwo.transform.GetChild(i).gameObject.layer = 11;
            }
            cameraTwoGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);
            cameras.Add(cameraTwo);
            cinemachineTargetGroups.Add(targetGroupTwo.GetComponent<CinemachineTargetGroup>());

        }
        else if(chooseCharactersPanel.GetCharactersData().Count == 3)
        {
            var cameraOneGameObject = GameObject.Instantiate(cameraGameObject);
            var cameraOne = cameraOneGameObject.GetComponent<Camera>();
            GameObject.Destroy(cameraOneGameObject.GetComponent<AudioListener>());
            //相机一设置
            cameraOne.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            cameraOne.cullingMask |= 1 << 10;
            cinemachineTargetGroupGameObject.layer = 10;
            for (int i = 0; i < cinemachineTargetGroupGameObject.transform.childCount; i++)
            {
                cinemachineTargetGroupGameObject.transform.GetChild(i).gameObject.layer = 10;
            }
            cinemachineTargetGroups.Add(cinemachineTargetGroupGameObject.GetComponent<CinemachineTargetGroup>());
            cameras.Add(cameraOne);
            cameraOneGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);

            //相机二设置
            var cameraTwoGameObject = GameObject.Instantiate(cameraGameObject);
            var targetGroupTwo = GameObject.Instantiate(cinemachineTargetGroupGameObject);
            GameObject.Destroy(cameraTwoGameObject.GetComponent<AudioListener>());
            var cameraTwo = cameraTwoGameObject.GetComponent<Camera>();
            cameraTwo.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            cameraTwo.cullingMask |= 1 << 11;
            targetGroupTwo.layer = 11;
            for (int i = 0; i < targetGroupTwo.transform.childCount; i++)
            {
                targetGroupTwo.transform.GetChild(i).gameObject.layer = 11;
            }
            cameraTwoGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);
            cameras.Add(cameraTwo);
            cinemachineTargetGroups.Add(targetGroupTwo.GetComponent<CinemachineTargetGroup>());

            //相机三设置
            var cameraThreeGameObject = GameObject.Instantiate(cameraGameObject);
            var targetGroupThree = GameObject.Instantiate(cinemachineTargetGroupGameObject);
            GameObject.Destroy(cameraThreeGameObject.GetComponent<AudioListener>());
            var cameraThree = cameraThreeGameObject.GetComponent<Camera>();
            cameraThree.rect = new Rect(0, 0, 0.5f, 0.5f);
            cameraThree.cullingMask |= 1 << 12;
            targetGroupThree.layer = 12;
            for (int i = 0; i < targetGroupThree.transform.childCount; i++)
            {
                targetGroupThree.transform.GetChild(i).gameObject.layer = 12;
            }
            cameraThreeGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);
            cameras.Add(cameraThree);
            cinemachineTargetGroups.Add(targetGroupThree.GetComponent<CinemachineTargetGroup>());

        }
        else if(chooseCharactersPanel.GetCharactersData().Count == 4)
        {
            var cameraOneGameObject = GameObject.Instantiate(cameraGameObject);
            var cameraOne = cameraOneGameObject.GetComponent<Camera>();
            GameObject.Destroy(cameraOneGameObject.GetComponent<AudioListener>());
            //相机一设置
            cameraOne.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            cameraOne.cullingMask |= 1 << 10;
            cinemachineTargetGroupGameObject.layer = 10;
            for (int i = 0; i < cinemachineTargetGroupGameObject.transform.childCount; i++)
            {
                cinemachineTargetGroupGameObject.transform.GetChild(i).gameObject.layer = 10;
            }
            cinemachineTargetGroups.Add(cinemachineTargetGroupGameObject.GetComponent<CinemachineTargetGroup>());
            cameras.Add(cameraOne);
            cameraOneGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);

            //相机二设置
            var cameraTwoGameObject = GameObject.Instantiate(cameraGameObject);
            var targetGroupTwo = GameObject.Instantiate(cinemachineTargetGroupGameObject);
            GameObject.Destroy(cameraTwoGameObject.GetComponent<AudioListener>());
            var cameraTwo = cameraTwoGameObject.GetComponent<Camera>();
            cameraTwo.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            cameraTwo.cullingMask |= 1 << 11;
            targetGroupTwo.layer = 11;
            for (int i = 0; i < targetGroupTwo.transform.childCount; i++)
            {
                targetGroupTwo.transform.GetChild(i).gameObject.layer = 11;
            }
            cameraTwoGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);
            cameras.Add(cameraTwo);
            cinemachineTargetGroups.Add(targetGroupTwo.GetComponent<CinemachineTargetGroup>());

            //相机三设置
            var cameraThreeGameObject = GameObject.Instantiate(cameraGameObject);
            var targetGroupThree = GameObject.Instantiate(cinemachineTargetGroupGameObject);
            GameObject.Destroy(cameraThreeGameObject.GetComponent<AudioListener>());
            var cameraThree = cameraThreeGameObject.GetComponent<Camera>();
            cameraThree.rect = new Rect(0, 0, 0.5f, 0.5f);
            cameraThree.cullingMask |= 1 << 12;
            targetGroupThree.layer = 12;
            for (int i = 0; i < targetGroupThree.transform.childCount; i++)
            {
                targetGroupThree.transform.GetChild(i).gameObject.layer = 12;
            }
            cameraThreeGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);
            cameras.Add(cameraThree);
            cinemachineTargetGroups.Add(targetGroupThree.GetComponent<CinemachineTargetGroup>());

            //相机四设置
            var cameraFourGameObject = GameObject.Instantiate(cameraGameObject);
            var targetGroupFour = GameObject.Instantiate(cinemachineTargetGroupGameObject);
            GameObject.Destroy(cameraFourGameObject.GetComponent<AudioListener>());
            var cameraFour = cameraFourGameObject.GetComponent<Camera>();
            cameraFour.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
            cameraFour.cullingMask |= 1 << 13;
            targetGroupFour.layer = 13;
            for (int i = 0; i < targetGroupFour.transform.childCount; i++)
            {
                targetGroupFour.transform.GetChild(i).gameObject.layer = 13;
            }
            cameraFourGameObject.GetComponent<UniversalAdditionalCameraData>().cameraStack.Remove(gameController.uiCamera);
            cameras.Add(cameraFour);
            cinemachineTargetGroups.Add(targetGroupFour.GetComponent<CinemachineTargetGroup>());


        }

        splitScreenImage.SetActive(true);
    }
}



