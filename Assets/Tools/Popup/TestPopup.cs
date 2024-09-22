using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class TestPopup : MonoBehaviour
{
    public PopupManager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Debug.Log("Opening popup");
            manager.Show<Popup>();
        }
    }
}
