using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBall : MonoBehaviour
{


    public float size = 1f;

    public float density = 1f;

    public float ballSize = 1f;


    private Vector3 SetBallPos(Vector3 Pos)
    {
        var result = new Vector3((int)Pos.x * size, 1, (int)Pos.z * size);
        return result;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        for (float x = 0; x <= 40 * density; x++)
        {
            for (float z = 0; z <= 40 * density; z++)
            {
                var point = SetBallPos(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point / density, 0.1f * ballSize);
            }
        }
    }
}