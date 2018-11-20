using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public bool isdead = false;
    bool isTower = false;
    void Start()
    {   
        if(transform.parent.name.Contains("Suppressor") || transform.parent.name.Contains("Nexus"))
        {
            return;
        }
        else if (transform.parent.parent.name.Contains("Tower"))
            isTower = true;
    }

    private void Update()
    {
        if(!isdead)
        {
            if (isTower)
            {
                transform.Rotate(0, 0, Time.deltaTime * 15);
            }
            else
                transform.Rotate(0, Time.deltaTime * 15, 0);
        }
        
    }

}
