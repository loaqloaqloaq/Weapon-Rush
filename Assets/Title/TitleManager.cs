using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleManager : MonoBehaviour
{
    //タイトルの表示 (テキスト、ボタン)
    public GameObject Title_Display;
    //操作説明テキスト
    public GameObject Explanation;
    //    // Start is called before the first frame update
    //    void Start()
    //    {

    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }

    //PVPボタンを押したとき
    void OnClickButton_PVP()
    {
        SceneManager.LoadScene("main");
    }

    //操作説明ボタンを押したとき
    void OnClickButton_Explanation()
    {
        Title_Display.SetActive(false);
        Explanation.SetActive(true);
    }

    //ゲームを終了ボタンを押したとき
    void OnClickButton_GameEnd()
    {
        Application.Quit();
    }
}
