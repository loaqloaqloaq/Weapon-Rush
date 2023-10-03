using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update      
    public GameObject p1, p2;

    [SerializeField]
    float[] xMargin;
    [SerializeField]
    float[] yMargin;

    [SerializeField]
    float[] xHalfMargin;    

    [SerializeField]
    int camNum = 0;

    [SerializeField]
    GameObject border;

    private float shakeDuration;

    private Camera cam;
    void Start()
    {
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");

        cam = GetComponent<Camera>();

        border = GameObject.Find("border");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            if (GetComponent<CameraShake>()) GetComponent<CameraShake>().shakeDuration = 0.1f;
            Debug.Log("shake");
        }

        if (Pause.pausePlayer != 0)
        {
            border.SetActive(false);
            return;
        }
        
            GameObject targetObj = camNum == 0 ? p1 : p2;
            if (targetObj.GetComponent<PlayerController>().HP <= 0) return;
            Vector3 target = Vector3.zero;
            float distance = Vector3.Distance(p1.transform.position, p2.transform.position);
            if (distance > 9.23f)
            {
                border.SetActive(true);
                target = targetObj.transform.position;
                target.z = -10;
                float viewPortLR = p1.transform.position.x > p2.transform.position.x ? (camNum == 1 ? 0 : 0.5f) : (camNum == 1 ? 0.5f : 0);
                cam.rect = new Rect(viewPortLR, 0, 0.5f, 1);

                target.x = target.x > xHalfMargin[1] ? xHalfMargin[1] : target.x < xHalfMargin[0] ? xHalfMargin[0] : target.x;
            }
            else
            {
                border.SetActive(false);
                cam.rect = new Rect(camNum, 0, 1, 1);
                target = new Vector3(
                    (p1.transform.position.x + p2.transform.position.x) / 2,
                    (p1.transform.position.y + p2.transform.position.y) / 2,
                    -10
                );

                target.x = target.x > xMargin[1] ? xMargin[1] : target.x < xMargin[0] ? xMargin[0] : target.x;
            }
            target.y = target.y > yMargin[1] ? yMargin[1] : target.y < yMargin[0] ? yMargin[0] : target.y;
        GetComponent<CameraShake>().originalPos = target;
        if (GetComponent<CameraShake>().shakeDuration <= 0)
        {
            transform.position = target;
        }        
    }
}
