using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //タイトルの表示 (テキスト、ボタン)
    public GameObject Title_Display;
    //操作説明テキスト
    public GameObject Explanation;

    GameObject nowButton;
    float defaultOpacity = 0.8f;

    bool explanation = false;

    private void Start()
    {
        GameData.Initialize();
    }

    private void Update()
    {
        nowButton = EventSystem.current.currentSelectedGameObject;
        ChangeButtonEffect();
        if (Explanation.activeSelf && Input.GetButtonDown("Cancel")) {
            OnClickButton_CloseExplanation();
        }
        if (Input.GetButtonDown("Submit") && explanation == false) {
            nowButton.GetComponent<Button>().onClick.Invoke();
        }       
    }

    //選択エフェクト
    private void ChangeButtonEffect()
    {
        nowButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        foreach (Button button in FindObjectsOfType<Button>())
        {
            if (button.gameObject != nowButton)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(1, 1, 1, defaultOpacity);
                }
            }
        }
    }

    //PVPボタンを押したとき
    public void OnClickButton_PVP()
    {
        //SceneManager.LoadScene("main");
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetString("mode", "PVP");
    }
    public void OnClickButton_PVE()
    {
        //SceneManager.LoadScene("main");
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetString("mode", "PVE");
    }

    //操作説明ボタンを押したとき
    public void OnClickButton_Explanation()
    {
        Title_Display.SetActive(false);
        Explanation.SetActive(true);
        explanation = true;
    }

    public void OnClickButton_CloseExplanation()
    {
        Title_Display.SetActive(true);
        Explanation.SetActive(false);
        explanation=false;
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
