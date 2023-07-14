using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickButton : MonoBehaviour
{
    //ポーズ時に表示するテキスト
    public GameObject PauseText;
    //タイトルに戻るか最終確認するテキスト
    public GameObject ConfirmationText;   

    //タイトルに戻るボタンが押されたとき
    public void OnClick_BacktoTitle_Button()
    {
        //ポーズテキストを非表示
        PauseText.SetActive(false);        
        //最終確認するテキストを表示
        ConfirmationText.SetActive(true);          
    }  
   
    //はいが押された場合
    public void OnClick_Yes_Button()
    {
        //タイトルシーンに切り替え
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Title");
    }

    //Cancelが押された場合
    public void OnClick_Cancel_Button()
    {
        //最終確認するテキストを非表示
        ConfirmationText.SetActive(false);
        //ポーズテキストを表示
        PauseText.SetActive(true);
    }
}
