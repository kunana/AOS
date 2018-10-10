using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Name { Going, Help, Targeting, Danger, Missing };
public class PingDisable : MonoBehaviour
{

    Name Pingname = Name.Danger;

    public float selfdestruct_in = 4; // Setting this to 0 means no selfdestruct.
    public PingPooling Pingpool;

    private void Awake()
    {
        if (!Pingpool)
        {
            GameObject.FindGameObjectWithTag("PingPool").GetComponent<PingPooling>();
        }
    }
    private void OnEnable()
    {
        selfdestruct_in = 4;

        if (gameObject.name.Contains("Going"))
            Pingname = Name.Going;
        else if (gameObject.name.Contains("Help"))
            Pingname = Name.Help;
        else if (gameObject.name.Contains("Targeting"))
            Pingname = Name.Targeting;
        else if (gameObject.name.Contains("Danger"))
            Pingname = Name.Danger;
        else if (gameObject.name.Contains("Missing"))
            Pingname = Name.Missing;
    }

    private void Update()
    {
        selfdestruct_in -= Time.deltaTime;
        if (selfdestruct_in <= 0)
            this.gameObject.SetActive(false);
    }

    //private void OnDisable()
    //{
    //    if (!Pingpool)
    //    {
    //        GameObject.FindGameObjectWithTag("PingPool").GetComponent<PingPooling>();
    //    }
    //    switch (Pingname)
    //    {
    //        case Name.Going:
    //            Pingpool.GoingPool.Add(gameObject);
    //            break;
    //        case Name.Help:
    //            Pingpool.HelpPool.Add(gameObject);
    //            break;
    //        case Name.Targeting:
    //            Pingpool.TargetPool.Add(gameObject);
    //            break;
    //        case Name.Danger:
    //            Pingpool.DangerPool.Add(gameObject);
    //            break;
    //        case Name.Missing:
    //            Pingpool.MissingPool.Add(gameObject);
    //            break;
    //    }
    //}
}
