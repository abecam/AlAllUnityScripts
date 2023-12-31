using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroCameraShop : MonoBehaviour
{
    Vector3 initPos = new Vector3(29, 2.2f, 2.2f);
    Quaternion initRot = Quaternion.Euler(0, -90, 0);

    // Global view:
    //Vector3 initPos = new Vector3(5.5f, -28, -12);
    //Quaternion initRot = Quaternion.Euler(270, -8, -16);

    float initFoV = 20;

    Vector3 targetPos = new Vector3(0, 0, -30);
    Quaternion targetRot = Quaternion.Euler(0, 0, 0);
    //float targetFoV = 6;
    float targetFoV = 20;

    Vector3 targetPos2 = new Vector3(0, 30, 0);
    Quaternion targetRot2 = Quaternion.Euler(90, 0, 0);
    float targetFoV2 = 20;

    int phaseTranslating = 0;
    float initTimeTr = 0;
    float speed = 0.05f;

    int phaseRotating = 0;
    float initTime = 0;

    int phaseFiedling = 0;
    float initTimeFielding = 0;

    public GameObject support; // We will bounce the shop
    float phase = 0;

    Vector3 initPosForSupport;

    // Start is called before the first frame update
    void Start()
    {
        initPosForSupport = support.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float posY = Mathf.Cos(phase);

        support.transform.localPosition = new Vector3(initPosForSupport.x, initPosForSupport.y + posY, initPosForSupport.z);

        phase += 0.16f;

        if (phaseTranslating == 0)
        {
            initTimeTr += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(initPos, targetPos, initTimeTr * speed);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                phaseTranslating = 1;

                initTimeTr = 0;
            }   
        }
        else if (phaseTranslating == 1)
        {
            initTimeTr += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(targetPos, targetPos2, initTimeTr * speed);

            if (Vector3.Distance(transform.position, targetPos2) < 0.01f)
            {
                phaseTranslating = 2;

                initTimeTr = 0;
            }
        }
        else if (phaseTranslating == 2)
        {
            initTimeTr += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(targetPos2, initPos, initTimeTr * speed);

            if (Vector3.Distance(transform.position, initPos) < 0.01f)
            {
                phaseTranslating = 0;

                initTimeTr = 0;
            }
        }
        if (phaseRotating == 0)
        {
            initTime += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(initRot, targetRot, initTime * speed);

            if (Mathf.Abs(transform.rotation.eulerAngles.y - targetRot.eulerAngles.y) < 0.1f)
            {
                phaseRotating = 1;

                initTime = 0;
            }
        }
        else if (phaseRotating == 1)
        {
            initTime += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(targetRot, targetRot2, initTime * speed);

            if (Mathf.Abs(transform.rotation.eulerAngles.x - targetRot2.eulerAngles.x) < 0.1f)
            {
                phaseRotating = 2;

                initTime = 0;
            }
        }
        else if (phaseRotating == 2)
        {
            initTime += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(targetRot2, initRot, initTime * speed);

            if (Mathf.Abs(transform.rotation.eulerAngles.y - initRot.eulerAngles.y) < 0.1f)
            {
                phaseRotating = 0;

                initTime = 0;
            }
        }
        if (phaseFiedling == 0)
        {
            initTimeFielding += Time.deltaTime;

            Camera.main.fieldOfView = Mathf.Lerp(initFoV, targetFoV, initTime * speed);

            if (Mathf.Abs(Camera.main.fieldOfView - targetFoV) < 0.1f)
            {
                phaseFiedling = 1;

                initTimeFielding = 0;
            }
        }
        else if (phaseFiedling == 1)
        {
            initTimeFielding += Time.deltaTime;

            Camera.main.fieldOfView = Mathf.Lerp(targetFoV, targetFoV2, initTime * speed);

            if (Mathf.Abs(Camera.main.fieldOfView - targetFoV2) < 0.1f)
            {
                phaseFiedling = 2;

                initTimeFielding = 0;
            }
        }
        else if (phaseFiedling == 2)
        {
            initTimeFielding += Time.deltaTime;

            Camera.main.fieldOfView = Mathf.Lerp(targetFoV2, initFoV, initTime * speed);

            if (Mathf.Abs(Camera.main.fieldOfView - initFoV) < 0.1f)
            {
                phaseFiedling = 0;

                initTimeFielding = 0;
            }
        }
    }
}
