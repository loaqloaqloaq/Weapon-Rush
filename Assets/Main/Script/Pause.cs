using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    //ポーズ時に表示するUI (BackGround含む)
    public GameObject PauseUI;
    //ポーズ時に表示するテキスト
    public GameObject PauseText;
    //タイトルに戻るか最終確認するテキスト
    public GameObject ConfirmationText;

    //AI設定
    public GameObject AiSettingPanel;
    public GameObject EnableWalk, EnableAttack;
    public Text walkText, attackText;
    Button walkButton, attackButton;

    GameObject p1, p2;

    int pausePlayer;

    private void Start()
    {
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");
        GameObject AI = p1.GetComponent<PlayerController>().player == 3 ? p1 :
            p2.GetComponent<PlayerController>().player == 3? p2 :
            null;
        if (AI != null) { 
            AiSettingPanel.SetActive(true);
            walkButton = EnableWalk.GetComponent<Button>();
            attackButton = EnableAttack.GetComponent<Button>();

            walkButton.onClick.AddListener(() =>
            {
                AI.GetComponent<AIController>().enableWalk = !AI.GetComponent<AIController>().enableWalk;
                walkText.text = AI.GetComponent<AIController>().enableWalk ? "はい" : "いいえ";
            });
            attackButton.onClick.AddListener(() =>
            {
                AI.GetComponent<AIController>().enableAttack = !AI.GetComponent<AIController>().enableAttack;
                attackText.text = AI.GetComponent<AIController>().enableAttack ? "はい" : "いいえ";
            });
        }
        else AiSettingPanel.SetActive(false);

       
    }
    void Update()
    {
        //Escapeキーが押された瞬間
        bool p1Pause = (Input.GetButtonDown("Player1_Pause") && p1.GetComponent<PlayerController>().player != 3);
        bool p2Pause = (Input.GetButtonDown("Player2_Pause") && p2.GetComponent<PlayerController>().player != 3);
        if (p1Pause && pausePlayer == 0) pausePlayer = 1;
        else if(p2Pause && pausePlayer == 0) pausePlayer = 2;
        if (p1Pause || p2Pause)
        {

            if (pausePlayer == 1 && p1Pause && PauseUI.activeSelf)
            {
                //ポーズUIの非表示
                PauseUI.SetActive(false);
                pausePlayer = 0;
            }
            else if (pausePlayer == 2 && p1Pause && PauseUI.activeSelf)
            {
                //ポーズUIの非表示
                PauseUI.SetActive(false);
                pausePlayer = 0;
            }
            else {
                //ポーズUIの表示
                PauseUI.SetActive(true);
            }
            

            //ポーズUI表示時
            if (PauseUI.activeSelf == true)
            {
                //一時停止
                Time.timeScale = 0.0f;
                //ポーズテキストを表示
                PauseText.SetActive(true);
            }
            //ポーズUI非表示時
            else
            {
                //再開
                Time.timeScale = 1.0f;
                //最終確認するテキストを非表示
                ConfirmationText.SetActive(false);
            }
        }
    }
    
}
