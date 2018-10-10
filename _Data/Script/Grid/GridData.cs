using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData : MonoBehaviour
{
    public int gridX, gridZ, beforeX, beforeZ, beforeInt, afterInt;
    //public int debug_gridObjArr, debug_gridDatArray;
    private void Start()
    {
        InitGridData();
    }
    private void Update()
    {
        //debug_gridObjArr = GridManager.instance.gridObjArray.Length;
        //debug_gridDatArray = GridManager.instance.gridDataArray.Length;
        ChangeGridData();
        if (beforeX != gridX || beforeZ != gridZ)
        {
            beforeInt = beforeZ * GridManager.instance.maxX + beforeX;
            afterInt = gridZ * GridManager.instance.maxX + gridX;
            if (GridManager.instance.gridObjArray[beforeInt] == this.gameObject)
            {//기존의 그리드가 지꺼였을 때 제거
                GridManager.instance.gridObjArray[beforeInt] = null;
                GridManager.instance.gridDataArray[beforeInt] = null;
            }
            if (GridManager.instance.gridObjArray[afterInt] != null)
            {//기존에 뭐시기가 있는데 그게 같은 좌표를 공유하고 있으면 이 그리드 값이 문제가 있는거니까 콘솔에 메시지를 출력해주자.
                if (GridManager.instance.gridObjArray[afterInt].activeInHierarchy)
                {
                    Vector3 tempVec = GridManager.instance.gridObjArray[afterInt].transform.position / GridManager.instance.gridTerm;
                    if ((int)tempVec.x == gridX && (int)tempVec.z == gridZ)
                    {
                        print("Warning : Overlab Grid Value ");
                    }
                }
            }
            GridManager.instance.gridObjArray[afterInt] = this.gameObject;
            GridManager.instance.gridDataArray[afterInt] = this;
            beforeX = gridX;
            beforeZ = gridZ;
        }
    }

    public void InitGridData()
    {
        ChangeGridData();
        beforeX = gridX;
        beforeZ = gridZ;
        GridManager.instance.gridObjArray[gridZ * GridManager.instance.maxX + gridX] = this.gameObject;
        GridManager.instance.gridDataArray[gridZ * GridManager.instance.maxX + gridX] = this;
    }

    public void ChangeGridData()
    {
        gridX = Mathf.RoundToInt(transform.position.x / GridManager.instance.gridTerm);
        gridZ = Mathf.RoundToInt(transform.position.z / GridManager.instance.gridTerm);
    }

    public void RemoveGridData()
    {
        if (gridX != -1 && gridZ != -1)
            if (GridManager.instance.gridObjArray[gridZ * GridManager.instance.maxX + gridX] != null)
                if (GridManager.instance.gridObjArray[gridZ * GridManager.instance.maxX + gridX] == this.gameObject)
                {
                    GridManager.instance.gridObjArray[gridZ * GridManager.instance.maxX + gridX] = null;
                    GridManager.instance.gridDataArray[gridZ * GridManager.instance.maxX + gridX] = null;
                }
        //gridX = -1;
        //gridZ = -1;
        //beforeZ = -1;
        //beforeX = -1;
        //afterInt = -1;
        //beforeInt = -1;
    }
}