using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    public PlayerController player1, player2;
    Timer_CountDown timer;
    [SerializeField]
    GameObject timeup;

    float loadSceneDelay;
    bool end;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
        player2 = GameObject.Find("Player2").GetComponent<PlayerController>();
        timer = GameObject.Find("Canvas/UIPanel/Panel/Text_Timer").GetComponent<Timer_CountDown>();
        timeup = GameObject.Find("Canvas/UIPanel/Text_TimeUp");
        timeup.SetActive(false);
        end = false;
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Player1: " + player1.HP.ToString());
        if (player1.HP <= 0 || player2.HP <= 0) {           
            end = true;
        }
        if (timer.CountDown_ <= 0) {
            timeup.SetActive(true);
            end = true;
        }
        if (end) {
            PlayerPrefs.SetFloat("player1HP", player1.HP);
            PlayerPrefs.SetFloat("player2HP", player2.HP);
            loadSceneDelay += Time.deltaTime;
        }
        if (loadSceneDelay >= 2f) {
            SceneManager.LoadScene("Result");
        }
    }   
}
