using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform m_CamArmTrans;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_CamArmTrans)
            m_CamArmTrans = GameObject.Find("CameraArm").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.visible)
            return;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = m_CamArmTrans.rotation.eulerAngles;

        float camAngleX = camAngle.x - mouseDelta.y;
        if (camAngleX < 180f)
            camAngleX = Mathf.Clamp(camAngleX, -1f, 30f);
        else
            camAngleX = Mathf.Clamp(camAngleX, 350f, 390f);

        m_CamArmTrans.rotation = Quaternion.Euler(camAngleX, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
