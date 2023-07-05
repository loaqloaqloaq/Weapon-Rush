using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultDirector : MonoBehaviour
{
    // Start is called before the first frame update
    Text t;
    float p1hp, p2hp;
    void Start()
    {
        p1hp = PlayerPrefs.GetFloat("player1HP");
        p2hp = PlayerPrefs.GetFloat("player2HP");
        t=GetComponent<Text>();
        t.text = "p1:" + p1hp.ToString() + "\np2:" + p2hp.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("player1HP", 0);
        PlayerPrefs.SetFloat("player2HP", 0);
    }
}
