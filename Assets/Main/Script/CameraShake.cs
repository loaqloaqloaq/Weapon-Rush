using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;
    public float shakeFequence = 0.05f;
    float lastShakeTime = 0;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1f;

    public Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }   

    void Update()
    {        
        if (shakeDuration > 0)
        {
            lastShakeTime += Time.deltaTime;
            if (lastShakeTime >= shakeFequence)
            {
                lastShakeTime = 0;
               Vector3 pos = originalPos + Random.insideUnitSphere * shakeAmount;
                pos = new Vector3(pos.x, pos.y, -10);
                camTransform.localPosition = pos;
                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }
}