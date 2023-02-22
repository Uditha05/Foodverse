using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Siccity.GLTFUtility;
using UnityEngine.Networking;

public class SandBoxManager : MonoBehaviour
{
    public GameObject pare;
    public GameObject pre1;
    public GameObject pre2;

    List<GameObject> goList = new List<GameObject>();

    bool isTr = false;
   public void clickButton()
    {
        Debug.Log("Screen Width : " + Screen.width);
        if (goList.Count != 0) Destroy(goList[0]);
        goList.Clear();
        if (isTr)
        {
            GameObject g = Instantiate(pre1, pare.transform);
            
            g.transform.localScale = new Vector3(1f, 1f, 1f);
            Debug.Log(g.GetComponent<Renderer>().bounds.size);
            goList.Add(g);
            isTr = false;
        }
        else
        {
            GameObject g = Instantiate(pre2, pare.transform);
            
            g.transform.localScale = new Vector3(1f, 1f, 1f);
            Debug.Log(g.GetComponent<Renderer>().bounds.size);
            goList.Add(g);
            isTr = true;
        }
    }
}
