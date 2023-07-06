using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //タイマー(UI)表示に使う
using UnityEngine.SceneManagement;
using TMPro;

public class Timer_CountDown : MonoBehaviour
{
    //制限時間を表示するText
    public TextMeshProUGUI Timer_Text;
    //タイムアップテキスト
    // public Text TimeUp_Text;
    //カウントダウン (60sec)    
    [HideInInspector]
    public float CountDown_;

    private void Start()
    {
        CountDown_ = 60f;
        Timer_Text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //制限時間を表示する+   
        Timer_Text.text = CountDown_.ToString("00");
        
        //Count_Down_が0超過のとき
        if(CountDown_ > 0.0f)
        {
            //カウントダウン
            CountDown_ -= Time.deltaTime;
        }               
    }
}
