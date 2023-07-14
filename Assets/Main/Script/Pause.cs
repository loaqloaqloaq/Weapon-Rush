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

    GameObject selecting;
    int selectingIndex;
    int interacting;
    [SerializeField]
    GameObject[] PauseMenu,AiMenu;
    GameObject[][] menus = new GameObject[2][];

    [HideInInspector]
    public static int confirmButtonIndex;
    public GameObject[] confirmButtons;
    GameObject confirmButton;    

    private void Start()
    {
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");       
        GameObject AI = PlayerPrefs.GetString("mode") == "PVE" ? p2 : null;           
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

        menus[0] = PauseMenu;
        menus[1] = AiMenu;

        confirmButtonIndex = 1;
        confirmButton = confirmButtons[1];
        ConfirmationButtonEffect();
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

                selectingIndex = 0;
                interacting = 0;
                ChangedSelecting();
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
        //キーボードやゲームパッドで操作
        if (PauseUI.activeSelf) {
            
            string pl = "Player" + pausePlayer.ToString() + "_";
            if (ConfirmationText.activeSelf)
            {                
                if (Input.GetButtonDown(pl + "Horizontal") && Input.GetAxisRaw(pl + "Horizontal") > 0)
                {
                    confirmButtonIndex++;
                    if (confirmButtonIndex > 1) confirmButtonIndex = 1;
                    confirmButton = confirmButtons[confirmButtonIndex];
                    ConfirmationButtonEffect();
                }
                else if (Input.GetButtonDown(pl + "Horizontal") && Input.GetAxisRaw(pl + "Horizontal") < 0)
                {
                    confirmButtonIndex--;
                    if (confirmButtonIndex < 0) confirmButtonIndex = 0;
                    confirmButton = confirmButtons[confirmButtonIndex];
                    ConfirmationButtonEffect();
                }
                if (Input.GetButtonDown(pl + "Attack"))
                {
                    confirmButton.GetComponent<Button>().onClick.Invoke();
                }
            }
            else
            {
                confirmButtonIndex = 1;
                confirmButton = confirmButtons[1];
                if (Input.GetButtonDown(pl + "Vertical") && Input.GetAxisRaw(pl + "Vertical") < 0)
                {
                    selectingIndex--;
                    if (selectingIndex < 0) selectingIndex = 0;
                    ChangedSelecting();
                }
                else if (Input.GetButtonDown(pl + "Vertical") && Input.GetAxisRaw(pl + "Vertical") > 0)
                {
                    selectingIndex++;
                    if (selectingIndex > (menus[interacting].Length - 1)) selectingIndex = (menus[interacting].Length - 1);
                    ChangedSelecting();
                }
                if (Input.GetButtonDown(pl + "Horizontal") && Input.GetAxisRaw(pl + "Horizontal") > 0)
                {
                    interacting++;
                    selectingIndex = 0;
                    if (interacting > 1) interacting = 1;
                    ChangedSelecting();
                }
                else if (Input.GetButtonDown(pl + "Horizontal") && Input.GetAxisRaw(pl + "Horizontal") < 0)
                {
                    interacting--;
                    selectingIndex = 0;
                    if (interacting < 0) interacting = 0;
                    ChangedSelecting();
                }
                if (Input.GetButtonDown(pl + "Attack"))
                {
                    selecting.GetComponent<Button>().onClick.Invoke();
                }
            }
        } 
    }

    //選択エフェクト
    private void ChangeButtonEffect()
    {
        foreach (var button in PauseMenu)
        {
            var img = button.GetComponent<Image>();
            if (button == selecting)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
            }
            else
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
            }
        }
        if (AiSettingPanel.activeSelf)
        {
            foreach (var button in AiMenu)
            {
                var img = button.GetComponent<Image>();
                if (button == selecting)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 1);                    
                }
                else
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
                }
            }
        }
    }
    private void ConfirmationButtonEffect() {
        foreach (var button in confirmButtons)
        {
            var img = button.GetComponent<Image>();
            if (button == confirmButton)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
            }
            else
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
            }
        }
    }

    private void ChangedSelecting() {
        Debug.Log("Interacting: " + interacting.ToString());
        Debug.Log("Index: " + selectingIndex.ToString());
        selecting = menus[interacting][selectingIndex];
        ChangeButtonEffect();
    }

    public void OnClick_Continue() {
        PauseUI.SetActive(false);
        pausePlayer = 0;
        Time.timeScale = 1.0f;
        ConfirmationText.SetActive(false);
    }

}
