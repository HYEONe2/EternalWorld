using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform CameraArmTrans;

    // Start is called before the first frame update
    void Start()
    {
        if (!CameraArmTrans)
            CameraArmTrans = GameObject.Find("CameraArm").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = CameraArmTrans.rotation.eulerAngles;

        float camAngleX = camAngle.x - mouseDelta.y;
        if (camAngleX < 180f)
            camAngleX = Mathf.Clamp(camAngleX, -1f, 30f);
        else
            camAngleX = Mathf.Clamp(camAngleX, 350f, 390f);

        CameraArmTrans.rotation = Quaternion.Euler(camAngleX, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
