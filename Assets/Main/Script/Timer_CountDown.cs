using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //タイマー(UI)表示に使う
using UnityEngine.SceneManagement;

public class Timer_CountDown : MonoBehaviour
{
    //制限時間を表示するText
    public Text Timer_Text;
    //タイムアップテキスト
    public Text TimeUp_Text;
    //カウントダウン (60sec)
    float CountDown_ = 60.0f;
    //終了してからの時間
    float FinishTime = 0.0f;
    
    void Update()
    {
        //制限時間を表示する
        Timer_Text.text = CountDown_.ToString("00");
        
        //Count_Down_が0超過のとき
        if(CountDown_ > 0.0f)
        {
            //カウントダウン
            CountDown_ -= Time.deltaTime;
        }
        //CountDown_が0以下になったとき
        else if (CountDown_ <= 0.0f)
        {
            TimeUp_Text.text = "TIME UP";
            FinishTime += Time.deltaTime;
        }

        //終了してから3秒経過
        if (FinishTime >= 3.0f)
        {
            //リザルトシーンに切り替え
            SceneManager.LoadScene("Result");
        }
    }
}
