using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //タイトル表示UI
    public GameObject Title_Display;
    //説明画面(チュートリアル)
    public GameObject Explanation;
    //マップ選択UI
    public GameObject MapSelect_Display;

    //SE
    [SerializeField] private AudioClip enterSE;
    [SerializeField] private AudioClip exitSE;

    //今選ばれているボタン
    GameObject nowButton;
    GameObject prevButton;
    float defaultOpacity = 0.8f;
    //説明画面(チュートリアル)の状態
    bool explanation = false;

    //マップセレクトボタンが押されているか
    [SerializeField] public static bool mapselect = false;

    private void Start()
    {
        //マップセレクトが押されたら
        if (mapselect == true)
        {
            //元の状態に戻す
            mapselect = false;

            Title_Display.SetActive(false);
            MapSelect_Display.SetActive(true);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Stage_1"));
        }
    }

    private void Update()
    {
        nowButton = EventSystem.current.currentSelectedGameObject;

        if (nowButton == null)
        {
            if(prevButton == null)
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            else
                EventSystem.current.SetSelectedGameObject(prevButton);

            nowButton = EventSystem.current.currentSelectedGameObject;
        }
        prevButton= EventSystem.current.currentSelectedGameObject;
        ChangeButtonEffect();
        if (Explanation.activeSelf && Input.GetButtonDown("Cancel"))
        {
            Close_Explanation();
        }
        if (Input.GetButtonDown("Submit") && explanation == false)
        {
            nowButton.GetComponent<Button>().onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }

    //選んでいるボタンの表示を変える
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
    //最初に選択しておくマップ
    private void SelectMap()
    {
        MapSelect_Display.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Stage_1"));

    }
    //マップセレクトを閉じたときに選択しておくボタン
    private void MapExit()
    {
        Title_Display.SetActive(true);
        MapSelect_Display.SetActive(false);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Button_PVP"));
    }
    //PVPモードを選択
    public void PVP()
    {
        Title_Display.SetActive(false);
        Invoke("SelectMap", 0.2f);
        PlayerPrefs.SetString("mode", "PVP");

        SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }
    //PVEモードを選択
    public void PVE()
    {
        Title_Display.SetActive(false);
        Invoke("SelectMap", 0.2f);
        PlayerPrefs.SetString("mode", "PVE");

        SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //説明画面(チュートリアル)を開く
    public void Open_Explanation()
    {
        Title_Display.SetActive(false);
        Explanation.SetActive(true);
        explanation = true;

        SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }
    //説明画面(チュートリアル)を閉じる
    public void Close_Explanation()
    {
        Title_Display.SetActive(true);
        MapSelect_Display.SetActive(false);
        Explanation.SetActive(false);
        explanation = false;

        SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);
    }

    //ゲームを終了
    public void GameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);

        Application.Quit();
    }

    //選んだステージに移行
    public void Stage1()
    {
        Title_Display.SetActive(false);
        MapSelect_Display.SetActive(true);
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetInt("map", 1);
    }
    public void Stage2()
    {
        Title_Display.SetActive(false);
        MapSelect_Display.SetActive(true);
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetInt("map", 2);
    }
    public void Stage3()
    {
        Title_Display.SetActive(false);
        MapSelect_Display.SetActive(true);
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetInt("map", 3);
    }
    public void Stage4()
    {
        Title_Display.SetActive(false);
        MapSelect_Display.SetActive(true);
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetInt("map", 4);
    }
    //マップ選択画面キャンセル (前の画面に戻る)
    public void MapSelect_Exit()
    {
        //ゲームデータを初期化
        GameData.Initialize();

        Invoke("MapExit", 0.2f);
        SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);

    }
}

