using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    public PlayerController player1, player2;
    Timer_CountDown timer;
    [SerializeField]
    GameObject timeup;

    float loadSceneDelay;
    public static bool end;
    public int map = 1;

    private float checkTimer;
    private float delay = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;

        checkTimer = 0f;

        player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerController>();
        timer = GameObject.Find("Canvas/UIPanel/Panel/Text_Timer").GetComponent<Timer_CountDown>();
        timeup = GameObject.Find("Canvas/UIPanel/Text_TimeUp");
        timeup.SetActive(false);
        end = false;

        Pause.pausePlayer = 0;

        List<GameObject> maps = new List<GameObject>();
        if (GameObject.Find("Map1")) maps.Add(GameObject.Find("Map1"));
        if (GameObject.Find("Map2")) maps.Add(GameObject.Find("Map2"));

        foreach (GameObject map in maps) { 
            map.SetActive(false);
        }
        try
        {
            maps[map - 1].SetActive(true);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            maps[0].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Player1: " + player1.HP.ToString());
        if (player1.HP <= 0 || player2.HP <= 0)
        {
            checkTimer += Time.deltaTime;
        }
        if (timer.CountDown_ <= 0)
        {
            timeup.SetActive(true);
            end = true;
        }

        if (checkTimer > delay)
        {
            end = true;
        }

        if (end)
        {
            PlayerPrefs.SetFloat("player1HP", player1.HP);
            PlayerPrefs.SetFloat("player2HP", player2.HP);
            loadSceneDelay += Time.deltaTime;
        }
        if (loadSceneDelay >= 2f)
        {
            if (player1.HP > player2.HP && player1.HP > 0)
            {
                GameData.winnerId = 1;
                GameData.p1.victoryCount++;
            }
            else if (player2.HP > player1.HP && player2.HP > 0)
            {
                GameData.winnerId = 2;
                GameData.p2.victoryCount++;
            }
            else if (player1.HP < 0 && player2.HP < 0 || player1.HP == player2.HP)
            {
                GameData.winnerId = 3;
            }

            SceneManager.LoadScene("Result");
        }
    }
}
