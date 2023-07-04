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
    private bool isLeftButtonDown;
    private bool isRightButtonDown;
    private int playerIndex;
    public Image background;
    public CharacterType characterType = CharacterType.PolarBear;
    public bool isReady = false;
    public List<Sprite> images = new List<Sprite>();
    public List<string> previewPrefabPath = new List<string>();
    public Image avatar;
    public RawImage rt;
    public GameObject previewObject;
    public RenderTexture renderTexture;
    public CharacterChooseControl control;
    public ChooseCharactersPanelContorl masterControl;
    private void Awake()
    {

    }

    public void Init(InputDevice inputDevice, int playerIndex, ChooseCharactersPanelContorl mcontrol)
    {
        masterControl = mcontrol;
        var go = new GameObject("InputReader");
        inputReader = go.AddComponent<InputReaderBase>();
        inputReader.Init(inputDevice);
        this.playerIndex = playerIndex;
        playerIndexText.text = $"P{playerIndex}";
        background.color = InputReadManager.Instance.playerColors[playerIndex - 1];
        avatar.sprite = images[(int)characterType - 1];

        previewObject = Instantiate(Resources.Load(previewPrefabPath[(int)(characterType - 1)], typeof(GameObject)) as GameObject, new Vector3(999*playerIndex, 0, 0), Quaternion.identity);
        control = previewObject.GetComponent<CharacterChooseControl>();
        int index = 0;
        while (masterControl.selectedColors.Contains(index))
        {
            index++;
        }
        control.index = index;
        renderTexture = new RenderTexture(300, 400, 24, RenderTextureFormat.ARGB64);
        rt.texture = renderTexture;
        control.characterCam.targetTexture = renderTexture;
        control.characterCam.Render();
    }

    private void OnDestroy()
    {
        Destroy(previewObject.gameObject);
    }

    private void LateUpdate()
    {
        if(inputReader.jump && !isButtonDown)
        {
            //var index = (int)characterType;
            //index = index + 1;
            //if (index > 3)
            //    index = 1;
            //characterType = (CharacterType)index;
            isReady = !isReady;
            isButtonDown = true;
        }
        if (!inputReader.jump)
            isButtonDown = false;

        if (inputReader.charge && !isRightButtonDown)
        {
            isRightButtonDown = true;
            control.SwitchColor(true);
        }
        if (!inputReader.charge)
            isRightButtonDown = false;

        if (inputReader.brake && !isLeftButtonDown)
        {
            isLeftButtonDown = true;
            control.SwitchColor(false);
        }
        if (!inputReader.brake)
            isLeftButtonDown = false;
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
