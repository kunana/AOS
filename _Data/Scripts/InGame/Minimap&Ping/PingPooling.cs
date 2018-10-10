using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPooling : MonoBehaviour {

    //풀링
    public GameObject[] FxPrefabs = new GameObject[5];
    protected List<GameObject> HelpPool = new List<GameObject>();
    protected List<GameObject> DangerPool = new List<GameObject>();
    protected List<GameObject> TargetPool = new List<GameObject>();
    protected List<GameObject> MissingPool = new List<GameObject>();
    protected List<GameObject> GoingPool = new List<GameObject>();
    protected Vector3 adjustHeight = new Vector3(0, 1.4f, 0);

    public int MakeCount = 0;
    public int MakeMaxCount = 7;

    private void Awake()
    {
        //풀링
        MakeFxPool("Going");
        MakeFxPool("Missing");
        MakeFxPool("Help");
        MakeFxPool("Danger");
        MakeFxPool("Target");
    }

    //Fx 풀링
    public void MakeFxPool(string name)
    {
        if (name.Equals("Help"))
        {
            for (int i = 0; i < MakeMaxCount; i++)
            {
                var fx = Instantiate(FxPrefabs[0], transform);
                HelpPool.Add(fx);
                fx.transform.position = Vector3.zero;
                fx.gameObject.SetActive(false);
            }
        }
        else if (name.Equals("Danger"))
        {
            for (int i = 0; i < MakeMaxCount; i++)
            {
                var fx = Instantiate(FxPrefabs[1], transform);
                DangerPool.Add(fx);
                fx.transform.position = Vector3.zero;
                fx.gameObject.SetActive(false);
                fx.gameObject.layer = 12;
            }
        }
        else if (name.Equals("Missing"))
        {
            for (int i = 0; i < MakeMaxCount; i++)
            {
                var fx = Instantiate(FxPrefabs[2], transform);
                MissingPool.Add(fx);
                fx.transform.position = Vector3.zero;
                fx.gameObject.SetActive(false);
                fx.gameObject.layer = 12;
            }
        }
        else if (name.Equals("Going"))
        {
            for (int i = 0; i < MakeMaxCount; i++)
            {
                var fx = Instantiate(FxPrefabs[3], transform);
                GoingPool.Add(fx);
                fx.transform.position = Vector3.zero;
                fx.gameObject.SetActive(false);
                fx.gameObject.layer = 12;
            }
        }
        else if (name.Equals("Target"))
        {
            for (int i = 0; i < MakeMaxCount; i++)
            {
                var fx = Instantiate(FxPrefabs[4], transform);
                TargetPool.Add(fx);
                fx.transform.position = Vector3.zero;
                fx.gameObject.SetActive(false);
                fx.gameObject.layer = 12;
            }
        }
    }

    public void GetFxPool(string name, Vector3 pos)
    {
        GameObject fx = null;
        if (name.Equals("Help"))
        {
            if (HelpPool.Count <= 0)
                MakeFxPool("Help");

            fx = HelpPool[0];
            HelpPool.RemoveAt(0);
        }
        else if (name.Equals("Missing"))
        {
            if (MissingPool.Count <= 0)
                MakeFxPool("Missing");

            fx = MissingPool[0];
            MissingPool.RemoveAt(0);
        }
        else if (name.Equals("Going"))
        {
            if (GoingPool.Count <= 0)
                MakeFxPool("Going");

            fx = GoingPool[0];
            GoingPool.RemoveAt(0);
        }
        else if (name.Equals("Target"))
        {
            if (TargetPool.Count <= 0)
                MakeFxPool("Target");

            fx = TargetPool[0];
            TargetPool.RemoveAt(0);
        }
        else if (name.Equals("Danger"))
        {
            if (DangerPool.Count <= 0)
                MakeFxPool("Danger");
            fx = DangerPool[0];
            DangerPool.RemoveAt(0);

        }
        fx.transform.position = pos;
        fx.transform.position = pos + adjustHeight;
        fx.gameObject.SetActive(true);
    }
}
