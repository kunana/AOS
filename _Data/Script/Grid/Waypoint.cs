using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Waypoint : MonoBehaviour
{

    public float speed = 5;
    public float waitTime = 0f;
    public float turnSpeed = 90;

    public Transform pathHolder;

    Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }
    IEnumerator TurnToFace(Vector3 lookTarget) //이동 방향으로 바라보기
    {
        Vector3 look = (lookTarget - this.transform.position).normalized;
        Quaternion newRotation = Quaternion.LookRotation(look);
        transform.DORotateQuaternion(newRotation, 0.3f);
        //Vector3 look = (lookTarget - this.transform.position).normalized;
        //Quaternion newRotation = Quaternion.LookRotation(look);
        //rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, newRotation, 50 * Time.deltaTime);
        yield return null;
        //Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        //float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        //while (Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle) > 0.05f)
        //{
        //    transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        //    yield return null;
        //}
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
    }
}
