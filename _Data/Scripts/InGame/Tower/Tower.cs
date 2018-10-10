using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public GameObject target = null;
    public float radius = 15f;
    public GameObject bullet = null;

    private bool champion_first = false;
    private Vector3 towerAttackPos = Vector3.zero;

    private float AttackTime = 1.0f;
    // Use this for initialization
    void Start () {
        towerAttackPos = transform.position + new Vector3(0, 9, 0);
    }
	
	// Update is called once per frame
	void Update () {
        AttackTime -= Time.deltaTime;

        //레이저 가이드
        if (target != null)
        {
            LaserLine();

            if (AttackTime < 0)
            {
                Attack();
                AttackTime = 1.0f;
            }
        }
        else
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            lr.positionCount = 0;
        }

        // 아군이 타워범위 내에서 적에게 맞으면 때린 적챔피언을 우선으로함
        // 챔피언이 피격시 때린적이 타워안에 있는지 체크하여 안에 있다면 타워의 타겟을 바꿔줌.
        if(champion_first)
        {

        }

        // 타워 성장은 미니언, 정글몹 성장과 마찬가지로 하기. 스탯을 이용하므로

        // 타워가 적에게 데미지를 주고난 후 적이 죽으면 
        // target = null; Re_detection(); 해주기. 그래야 새로 탐색하니까

        //print(target);
	}

    public void Attack()
    {
        var towerBullet = Instantiate(bullet, towerAttackPos, Quaternion.identity);
        towerBullet.transform.parent = transform;
        towerBullet.GetComponent<TowerBullet>().target = target;
    }

    public void LaserLine()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.SetPosition(0, towerAttackPos);
        lr.SetPosition(1, target.transform.position);
    }
    
    public void Re_detection()
    {
        if (target == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            
            float min_distance_m = 1000;
            float min_distance_p = 1000;
            int minionCount = 0;
            GameObject player = null;
            for (int i = 0; i < hitColliders.Length; i++)
            {
                // 같은편이면 검사안함
                if (LayerMask.LayerToName(gameObject.layer) == LayerMask.LayerToName(hitColliders[i].gameObject.layer))
                    continue;

                // 재탐색시 대포미니언이 있으면 대포미니언 우선
                if (hitColliders[i].CompareTag("CannonMinion"))
                {
                    target = hitColliders[i].gameObject;
                    break;
                }

                // 아니면 거리 계산하여 가장 가까운 미니언을 잡음.
                float distance = Vector3.Distance(hitColliders[i].transform.position, transform.position);
                if (distance <= min_distance_m)
                {
                    if (hitColliders[i].CompareTag("Minion"))
                    {
                        target = hitColliders[i].gameObject;
                        min_distance_m = distance;
                        minionCount++;
                    }
                }

                // 플레이어가 타워범위내에 있으면 가장가까운 플레이어를 찾아만 놓음
                if (distance <= min_distance_p)
                {
                    if (hitColliders[i].CompareTag("Player"))
                    {
                        player = hitColliders[i].gameObject;
                        min_distance_p = distance;
                    }
                }
            }

            // 재탐색이 끝났을때 범위내에 플레이어가 존재하고 미니언이 없으면 타겟을 플레이어로(가장 가까운거 찾아뒀으니)
            if (player != null && minionCount == 0)
                target = player;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 들어온 오브젝트가 나와 같은팀이면 아무것도 안함
        if (LayerMask.LayerToName(gameObject.layer) == LayerMask.LayerToName(other.gameObject.layer))
            return;

        if (other.CompareTag("CannonMinion") || other.CompareTag("Minion") || other.CompareTag("Player"))
        {
            if (target == null)
                target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (target == other.gameObject)
        {
            target = null;
            Re_detection();
        }
    }
}
