using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tint : MonoBehaviour
{
    void Update()
    {
        if (gameObject.tag.Equals("Player Cards"))
        {
            gameObject.GetComponent<Image>().color = new Color32(122, 255, 151, 100);
        }
        if (gameObject.tag.Equals("Ai Cards"))
        {
            gameObject.GetComponent<Image>().color = new Color32(231, 82, 82, 100);
        }
    }
}
