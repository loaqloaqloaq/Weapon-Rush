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

    private void Start()
    {
        GameObject AI = GameObject.Find("Player1").GetComponent<PlayerController>().player == 3 ? GameObject.Find("Player1") : 
            GameObject.Find("Player2").GetComponent<PlayerController>().player == 3? GameObject.Find("Player2") :
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ポーズUIの表示、非表示を切り替え
            PauseUI.SetActive(!PauseUI.activeSelf);

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
