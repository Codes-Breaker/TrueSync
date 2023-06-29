using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.VFX;
using UnityEngine.Playables;
using Cinemachine;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    public Camera mainCamera;
    public Camera uiCamera;
    public GameObject GameOverGO;
    public TMP_Text GameOverText;
    public GameObject windText;
    public GameObject windIndicator;
    public GameObject infoBg;
    public OceanController oceanController;
    public float RiseSeaLevelTime = 180;
    private float gameTime = 0;
    private bool hasRiseSea = false;
    private bool hasStartEvent = false;
    public bool startGame = false;
    public bool debug = false;
    public bool isGameOver = false;
    public PlayableDirector director;
    private int winIndex = -1;
    private int winFurIndex = -1;
    public string winFurName = "";
    private List<CharacterContorl> winners = new List<CharacterContorl>();
    public CinemachineVirtualCamera winVM;
    public GameObject bornPointsParent;
    public PlayableDirector randomEventDirector;
    public float RandomEventTime = 120;
    public GameMode gameMode = GameMode.Single;
    public ScreenMode screenMode = ScreenMode.Same;
    public GameObject splitScreenBlackImage;
    public bool ufoSpeedUpPhase = false;
    public UFODevice ufoDevice;
    // Start is called before the first frame update
    void Awake()
    {
        GameOverGO?.gameObject.SetActive(false);
        windText?.gameObject.SetActive(false);
        infoBg?.gameObject.SetActive(false);
        windIndicator?.gameObject.SetActive(false);
        gameTime = 0;
        hasRiseSea = false;
        isGameOver = false;
        hasStartEvent = false;
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void FixedUpdate()
    {
        if (startGame)
        {
            gameTime = gameTime += Time.fixedDeltaTime;
            CheckSeaLevelRise();
            CheckRandomEvent();
            CheckUFOSpeed();
        }

    }

    private void CheckUFOSpeed()
    {
        if (!hasStartEvent || randomEventDirector.state != PlayState.Playing)
            return;
        if (ufoSpeedUpPhase)
        {
            randomEventDirector.playableGraph.GetRootPlayable(0).SetSpeed(1 + ufoDevice.catchedPlayer * 0.5f);
        }
        else
        {
            randomEventDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }
    }

    private void CheckRandomEvent()
    {
        if (!hasStartEvent)
        {
            if (gameTime >= RandomEventTime)
            {
                hasStartEvent = true;
                randomEventDirector.Play();
            }
        }
    }

    private void CheckSeaLevelRise()
    {
        if (!hasRiseSea)
        {
            if (gameTime >= RiseSeaLevelTime)
            {
                //强制结束游戏
                hasRiseSea = true;
                oceanController.StartRising();
            }
        }

    }

    public void StartSpeedUpUFO()
    {
        ufoSpeedUpPhase = true;
    }

    public void EndSpeedUpUFO()
    {
        ufoSpeedUpPhase = false;
    }

    public void CheckGameState()
    {
        List<CharacterContorl> characters = GameObject.FindObjectsOfType<CharacterContorl>().ToList();
        switch (gameMode)
        {
            case GameMode.Single:
                if (characters.Count <= 1 || characters.Sum(x => x.isDead ? 0 : 1) == 1)
                {
                    var winCharacter = characters.FirstOrDefault(x => !x.isDead);
                    isGameOver = true;
                    winCharacter?.SetWin();
                    winners.Add(winCharacter);
                    winIndex = winCharacter.playerIndex;
                    //winVM.LookAt = winCharacter.transform;
                    //winVM.Follow = winCharacter.transform;
                    GameOver();
                }
                break;

            case GameMode.Multiple:
                if (characters.Count <= 1 || characters.Where(x => !x.isDead).GroupBy(x => x.furIndex).Count() == 1)
                {
                    var winFur = characters.FirstOrDefault(x => !x.isDead).furIndex;
                    winFurName = characters.FirstOrDefault(x => !x.isDead).furData.furDataList[winFur].furName;
                    isGameOver = true;
                    foreach(var character in characters.Where(x => !x.isDead))
                    {
                        character.SetWin();
                        winners.Add(character);
                    }
                    winFurIndex = winFur;

                    //winCharacter?.SetWin();
                    //winIndex = winCharacter.playerIndex;
                    //winVM.LookAt = winCharacter.transform;
                    //winVM.Follow = winCharacter.transform;
                    GameOver();
                }
                break;

            default:
                break;
        }


    }

    IEnumerator DelayOpen()
    {
        yield return new WaitForSeconds(4);
        GameOverGO.SetActive(true);
        switch (gameMode)
        {
            case GameMode.Single:
                GameOverText.text = $"P{winIndex} Win!\nPress R to Restart";
                break;
            case GameMode.Multiple:
                GameOverText.text = $"{winFurName} Win!\nPress R to Restart";
                break;

            default:

                break;
        }

    }

    public void StartGame()
    {
        startGame = true;
    }

    private void GameOver()
    {
        //director?.Play();
        List<CharacterContorl> characters = GameObject.FindObjectsOfType<CharacterContorl>().ToList();
        if(screenMode == ScreenMode.Split)
        {
            foreach (var character in characters)
            {
                character.m_camera?.gameObject.SetActive(false);
            }
            var winner = winners.FirstOrDefault();
            Camera.main.cullingMask |= 1 << winner.cinemachineTargetGroup.gameObject.layer;
            foreach (var w in winners)
            {
                winner.cinemachineTargetGroup.AddMember(w.transform, 2, 4);
            }
            splitScreenBlackImage.gameObject.SetActive(false);
        }

        StartCoroutine(DelayOpen());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            debug = !debug;
        }
        
    }
}
