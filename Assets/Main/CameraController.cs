using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update      
    public GameObject p1, p2;
    float[] xMargin= { 0,9.84f};
    float[] yMargin= { 0,5};

    private Camera cam;
    void Start()
    {
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = new Vector3(
            (p1.transform.position.x + p2.transform.position.x) / 2,
            (p1.transform.position.y + p2.transform.position.y) / 2,
            -10
        );
        target.x = target.x > xMargin[1] ? xMargin[1] : target.x < xMargin[0] ? xMargin[0] : target.x;
        target.y = target.y > yMargin[1] ? yMargin[1] : target.y < yMargin[0] ? yMargin[0] : target.y;
        transform.position = target;       
    }
}
