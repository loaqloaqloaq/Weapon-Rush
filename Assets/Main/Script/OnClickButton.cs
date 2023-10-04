using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OnClickButton : MonoBehaviour
{
    //ポーズ時に表示するテキスト
    public GameObject PauseText;
    //タイトルに戻るか最終確認するテキスト
    public GameObject ConfirmationText;

    //SE
    [SerializeField] private AudioClip UiSE;
    

    //タイトルに戻るボタンが押されたとき
    public void OnClick_BacktoTitle_Button()
    {
        //ポーズテキストを非表示
        PauseText.SetActive(false);        
        //最終確認するテキストを表示
        ConfirmationText.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Yes"));
        PlayUISound();
    }  
   
    //はいが押された場合
    public void OnClick_Yes_Button()
    {
        PlayUISound();
        //タイトルシーンに切り替え
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Title");
    }

    private void PlayUISound()
    {
        SoundManager.Instance.Play(UiSE, SoundManager.Sound.UI);
    }

    //Cancelが押された場合
    public void OnClick_Cancel_Button()
    {
        //最終確認するテキストを非表示
        ConfirmationText.SetActive(false);
        //ポーズテキストを表示
        PauseText.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("PauseUI/Continue"));
        PlayUISound();
    }
}
