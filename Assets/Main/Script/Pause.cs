using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    //ポーズ時に表示するUI (BackGround含む)
    public GameObject PauseUI;
    //ポーズ時に表示するテキスト
    public GameObject PauseText;
    //タイトルに戻るか最終確認するテキスト
    public GameObject ConfirmationText;

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
