using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBullet : MonoBehaviour {

    [HideInInspector]
    public GameObject target;

    private Vector3 dir = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null)
        {
            dir = (target.transform.position - transform.position).normalized;

            transform.position += dir * 0.5f;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            Destroy(gameObject);
            // 그리고 타겟에게 타워데미지

            //만약 적이 죽었다면
            //transform.parent.GetComponent<Tower>().target = null;
            //transform.parent.GetComponent<Tower>().Re_detection();
        }
    }
}
