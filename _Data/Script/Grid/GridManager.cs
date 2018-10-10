using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager _instance = null;
    public static GridManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(GridManager)) as GridManager;
            return _instance;
        }
    }
    public int maxX, maxZ; // 그리드 칸 최대 수
    public float gridTerm = 1.5f; // 그리드 한 변의 사이즈 (미니언 오브젝트 스케일의 2배면 됨) // 2배면 한 애가 둘씩 들어가는 경우가 생겼음. 1.25, 1.5도 마찬가지. 1배로 해보자.
    [HideInInspector]
    public GameObject[] gridObjArray;
    [HideInInspector]
    public GridData[] gridDataArray;
    //public  gridDataArray;
    private void OnEnable()
    {
        gridObjArray = new GameObject[maxX * maxZ + 1];
        gridDataArray = new GridData[maxX * maxZ + 1];
    }
}