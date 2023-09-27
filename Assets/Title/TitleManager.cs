using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //�^�C�g���̕\�� (�e�L�X�g�A�{�^��)
    public GameObject Title_Display;
    //��������e�L�X�g
    public GameObject Explanation;
    public GameObject MapSelect;
    public GameObject stage1;

    //SE
    [SerializeField] private AudioClip enterSE;
    [SerializeField] private AudioClip exitSE;


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
        

        if (nowButton == null)
        {
            return;
        }

        ChangeButtonEffect();
        if (Explanation.activeSelf && Input.GetButtonDown("Cancel")) {
            OnClickButton_CloseExplanation();
        }
        if (Input.GetButtonDown("Submit") && explanation == false) {
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

    //�I���G�t�F�N�g
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

    private void SelectMap()
    {
        MapSelect.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Stage_1"));

    }
    private void MapExit()
    {
        Title_Display.SetActive(true);
        MapSelect.SetActive(false);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Button_PVP"));
    }
    //PVP�{�^�����������Ƃ�
    public void OnClickButton_PVP()
    {
        Title_Display.SetActive(false);
        Invoke("SelectMap", 0.2f);
        PlayerPrefs.SetString("mode", "PVP");

        SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }
    public void OnClickButton_PVE()
    {
        Title_Display.SetActive(false);
        Invoke("SelectMap", 0.2f);
        PlayerPrefs.SetString("mode", "PVE");

        SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //��������{�^�����������Ƃ�
    public void OnClickButton_Explanation()
    {
        Title_Display.SetActive(false);
        Explanation.SetActive(true);
        explanation = true;

        SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    public void OnClickButton_CloseExplanation()
    {
        Title_Display.SetActive(true);
        MapSelect.SetActive(false);
        Explanation.SetActive(false);
        explanation=false;

        SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);
    }

    //�Q�[�����I���{�^�����������Ƃ�
    public void OnClickButton_GameEnd()
    {        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

        SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);

        Application.Quit();
    }
    public void Stage1()
    {
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetInt("map", 1);
    }
    public void Stage2()
    {
        LoadingSceneController.LoadScene("main");
        PlayerPrefs.SetInt("map", 2);
    }
    public void OnClickButton_Exit1()
    {
        Invoke("MapExit", 0.2f);

        SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);
    }
}
