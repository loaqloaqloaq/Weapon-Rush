using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //シーンの読み込み

public class Timer_CountDown : MonoBehaviour
{
    //制限時間 (カウントダウン方式)
    float CountDown_;
    //制限時間終了からの経過時間
    float FinishTime_;

    private void Start()
    {
        //時間の初期設定
        CountDown_ = 60.0f;
        FinishTime_ = 0.0f;
    }
    private void Update()
    {
        //制限時間を表示する
        UIManager.Instance.UpdateTimerText((int)CountDown_);

        //制限時間が0超過のとき
        if (CountDown_ > 0.0f)
        {
            //カウントダウン
            CountDown_ -= Time.deltaTime;
        }
        //制限時間が0以下になったとき
        else if (CountDown_ <= 0.0f)
        {
            //TimeUpTextを表示
            UIManager.Instance.SetActiveTimeUpText();
            FinishTime_ += Time.deltaTime;
        }

        //制限時間終了から3秒経過
        if (FinishTime_ >= 3.0f)
        {
            //リザルトシーンに切り替え
            SceneManager.LoadScene("Result");
        }
    }
}