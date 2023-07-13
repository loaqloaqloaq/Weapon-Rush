using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleManager : MonoBehaviour
{
    //タイトルの表示 (テキスト、ボタン)
    public GameObject Title_Display;
    //操作説明テキスト
    public GameObject Explanation;

    private void Start()
    {
        GameData.Initialize();
    }

    //PVPボタンを押したとき
    public void OnClickButton_PVP()
    {
        //SceneManager.LoadScene("main");
        LoadingSceneController.LoadScene("main");
    }

    //操作説明ボタンを押したとき
    public void OnClickButton_Explanation()
    {
        Title_Display.SetActive(false);
        Explanation.SetActive(true);
    }

    //ゲームを終了ボタンを押したとき
    public void OnClickButton_GameEnd()
    {        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
}
