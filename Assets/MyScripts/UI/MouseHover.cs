using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    void OnMouseEnter()
    {
        Debug.Log("?!");
        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, GetComponent<AudioSource>(), 5);
    }
}
