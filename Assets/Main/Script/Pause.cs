using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    //ポーズ時に表示するUIプレハブ
    public GameObject PauseUI;
    
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
            }
            //ポーズUI非表示時
            else
            {
                //再開
                Time.timeScale = 1.0f;
            }
        }
        
    }
}
