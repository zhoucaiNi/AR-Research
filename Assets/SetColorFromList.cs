using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorFromList : MonoBehaviour
{
    public List<Color> colorList;

    public void SecColor(int i) 
    { 
        GetComponent<Renderer>().material.color = colorList[i];
    }

}