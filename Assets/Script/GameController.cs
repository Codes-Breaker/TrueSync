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
    public VisualEffect windEffect;
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
    public CinemachineVirtualCamera winVM;
    public GameObject bornPointsParent;
    public PlayableDirector randomEventDirector;
    public float RandomEventTime = 120;
    public GameMode gameMode = GameMode.Single;
    public ScreenMode screenMode = ScreenMode.Same;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameOverGO?.gameObject.SetActive(false);
        windText?.gameObject.SetActive(false);
        infoBg?.gameObject.SetActive(false);
        windIndicator?.gameObject.SetActive(false);
        windEffect.SetFloat("ParticlesRate", 0);
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
        director?.Play();
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
