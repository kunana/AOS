using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<T, U>
{
    public T first;
    public U second;
    public Pair(T f, U s)
    {
        first = f;
        second = s;
    }
}

public class SkillTest : MonoBehaviour
{
    Vector3 startVec, finDirVec, direction, rightAngle;
    int width, destDist;
    Vector3 p1, p2, p3, p4, pCenter;
    List<Pair<float, float>> crashGrid = new List<Pair<float, float>>();
    float gridTerm;
    int gridMaxX;
    float minX, maxX, minZ, maxZ;
    int minGridX, maxGridX, minGridZ, maxGridZ;
    private void OnEnable()
    {
        Init();
        Check();
    }

    private void Init()
    {
        startVec = new Vector3(3, 0, 3);
        width = 1;
        destDist = 1;
        finDirVec = new Vector3(6, 0, 3);
        gridTerm = GridManager.instance.gridTerm;
        gridMaxX = GridManager.instance.maxX;
    }

    private void Check()
    {
        CheckInit();

        minX = Min(p1.x, p2.x, p3.x, p4.x);
        maxX = Max(p1.x, p2.x, p3.x, p4.x);
        minZ = Min(p1.z, p2.z, p3.z, p4.z);
        maxZ = Max(p1.z, p2.z, p3.z, p4.z);

        minGridX = Mathf.RoundToInt(minX / gridTerm);
        maxGridX = Mathf.RoundToInt(maxX / gridTerm);
        minGridZ = Mathf.RoundToInt(minZ / gridTerm);
        maxGridZ = Mathf.RoundToInt(maxZ / gridTerm);
        for (int i = minGridX; i <= maxGridX; ++i)
        {
            int nowMinZ = -1, nowMaxZ = -1;
            float fi = (float)i * gridTerm;
            for (int j = minGridZ; j <= maxGridZ; ++j)
            {
                float fj = (float)j * gridTerm;
                if (CrossCount(new Vector3(fi, 0, fj), p1, p2, p3, p4))
                {
                    nowMinZ = j;
                    break;
                }
            }
            if (nowMinZ != -1)
            {
                for (int j = maxGridZ; j >= minGridZ; --j)
                {
                    float fj = j * gridTerm;
                    if (CrossCount(new Vector3(fi, 0, fj), p1, p2, p3, p4))
                    {
                        nowMaxZ = j;
                        break;
                    }
                }
                for (int j = nowMinZ; j <= nowMaxZ; ++j)
                    crashGrid.Add(new Pair<float, float>(i, j));
            }
        }
        print("count" + crashGrid.Count);
        foreach (Pair<float, float> p in crashGrid)
        {
            print("(" + p.first + ", " + p.second + ")");
        }
    }

    private void CheckInit()
    {
        direction = (finDirVec - startVec).normalized;
        rightAngle = new Vector3(-direction.z, 0, direction.x);
        p1 = startVec - (rightAngle * (width / 2f));
        p2 = startVec + (rightAngle * (width / 2f));
        p3 = p2 + (direction * destDist);
        p4 = p1 + (direction * destDist);
    }

    private float Min(params float[] num)
    {
        float min = num[0];
        for (int i = 0, j = num.Length - 1; i < j; min = Mathf.Min(min, num[++i])) ;
        return min;
    }

    private float Max(params float[] num)
    {
        float max = num[0];
        for (int i = 0, j = num.Length - 1; i < j; max = Mathf.Max(max, num[++i])) ;
        return max;
    }

    private bool CheckOneLine(Vector3 refPoint, Vector3 v1, Vector3 v2)
    {
        bool checker = false;
        if (v1.x == refPoint.x || v2.x == refPoint.x)
            checker = true;
        else if ((v1.x > refPoint.x && v2.x < refPoint.x) || (v1.x < refPoint.x && v2.x > refPoint.x))
            checker = true;
        if (checker)
            if (direction.z > 0)
            {
                if (refPoint.z <= EquationOfLine(refPoint.x, v1, v2))
                    return true;
            }
            else if (refPoint.z >= EquationOfLine(refPoint.x, v1, v2))
            {
                return true;
            }
        return false;
    }

    private float EquationOfLine(float refX, Vector3 v1, Vector3 v2)
    {//두 점을 잇는 직선의 방정식에서 x에 일치하는 z 리턴
        if (v1.x != v2.x)
            return (((v2.z - v1.z) * (refX - v1.x)) / (v2.x - v1.x)) + v1.z;
        if (p2.x == p1.x)
        {
            if (p2.z >= p1.z)
                return p2.z;
            else
                return p1.z;
        }
        else if (p3.x == p4.x)
        {
            if (p3.z >= p4.z)
                return p3.z;
            else
                return p4.z;
        }
        else
        {
            print("SkillTest.cs::EquationOfLine()::Error::I can't calculate it.");
        }
        return 0;
    }

    private bool CrossCount(Vector3 refPoint, params Vector3[] polyPoint)
    {
        int count = 0;
        for (int i = 0, j = polyPoint.Length; i < j; ++i)
            if (CheckOneLine(refPoint, polyPoint[i], polyPoint[(i + 1) % j]))
                ++count;
        if (count % 2 == 1)
            return true;
        return false;
    }
}