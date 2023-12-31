using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraShop : MonoBehaviour
{
    Vector3 initPos = new Vector3(0, -20, -6);
    Quaternion initRot = Quaternion.Euler(270, 0, 0);

    // Global view:
    //Vector3 initPos = new Vector3(5.5f, -28, -12);
    //Quaternion initRot = Quaternion.Euler(270, -8, -16);

    float initFoV = 41;

    Vector3 targetPos = new Vector3(0, 0, -100);
    Quaternion targetRot = Quaternion.Euler(0, 0, 0);
    float targetFoV = 6;

    bool isTranslating = true;
    float initTimeTr = 0;
    float speed = 0.2f;

    bool isRotating = true;
    float initTime = 0;

    bool isFiedling = true;
    float initTimeFielding = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTranslating)
        {
            initTimeTr += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(initPos, targetPos, initTimeTr * speed);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                isTranslating = false;
            }
        }
        if (isRotating)
        {
            initTime += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(initRot, targetRot, initTime * speed);

            if (Mathf.Abs(transform.rotation.eulerAngles.x - targetRot.eulerAngles.x) < 0.1f)
            {
                isRotating = false;
            }
        }
        if (isFiedling)
        {
            initTimeFielding += Time.deltaTime;

            Camera.main.fieldOfView= Mathf.Lerp(initFoV, targetFoV, initTime * speed);

            if (Mathf.Abs(Camera.main.fieldOfView - targetFoV) < 0.1f)
            {
                isFiedling = false;
            }
        }
    }
}
