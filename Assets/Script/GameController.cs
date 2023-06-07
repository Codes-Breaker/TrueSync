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
    public bool startGame = false;
    public bool debug = false;
    public bool isGameOver = false;
    public PlayableDirector director;
    private int winIndex = -1;
    public CinemachineVirtualCamera winVM;
    public GameObject bornPointsParent;
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
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void FixedUpdate()
    {
        if (startGame)
        {
            gameTime = gameTime += Time.fixedDeltaTime;
            CheckSeaLevelRise();
        }

    }

    private void CheckSeaLevelRise()
    {
        if (!hasRiseSea)
        {
            if (gameTime >= RiseSeaLevelTime)
            {
                //ǿ�ƽ�����Ϸ
                hasRiseSea = true;
                oceanController.StartRising();
            }
        }

    }

    public void CheckGameState()
    {
        List<CharacterContorl> characters = GameObject.FindObjectsOfType<CharacterContorl>().ToList();
        if (characters.Count <= 1 || characters.Sum(x => x.isDead ? 0: 1) == 1)
        {
            var winCharacter = characters.FirstOrDefault(x => !x.isDead);
            isGameOver = true;
            winCharacter?.SetWin();
            winIndex = winCharacter.playerIndex;
            winVM.LookAt = winCharacter.transform;
            winVM.Follow = winCharacter.transform;
            GameOver();
        }

    }

    IEnumerator DelayOpen()
    {
        yield return new WaitForSeconds(4);
        GameOverGO.SetActive(true);
        GameOverText.text = $"P{winIndex} Win!\nPress R to Restart";
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