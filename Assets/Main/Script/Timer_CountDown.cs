using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //シーンの読み込み

public class Timer_CountDown : MonoBehaviour
{
    //Inspector上で見えないようにする
   [HideInInspector]
    //制限時間 (カウントダウン方式)
    public float CountDown;

    private void Start()
    {
        //時間の初期設定
        CountDown = 60.0f;
    }
    private void Update()
    {
        //制限時間を表示する
        UIManager.Instance.UpdateTimerText((int)CountDown);

        //制限時間が0超過のとき
        if (CountDown > 0.0f)
        {
            //カウントダウン
            CountDown -= Time.deltaTime;
        }
        //制限時間が0以下になったとき
        else if (CountDown <= 0.0f)
        {
            //TimeUpTextを表示
            UIManager.Instance.SetActiveTimeUpText();
        }
    }
}