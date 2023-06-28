using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //タイマー(UI)表示に使う

public class Timer_CountDown : MonoBehaviour
{
    //制限時間を表示するText
    public Text Timer_Text;
    //タイムアップテキスト
    public Text TimeUp_Text;
    //カウントダウン (60sec)
    float CountDown_ = 60.0f;
    
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
        }
    }
}
