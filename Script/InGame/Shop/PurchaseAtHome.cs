using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseAtHome : MonoBehaviour {

    private float recoveryTime = 0;

    private void OnTriggerStay(Collider other)
    {
        // 구매가능
        if (other.tag.Equals("Player"))
        {
            PlayerData.Instance.purchaseState = true;
            ChampionData cd = other.GetComponent<ChampionData>();
            if (cd != null)
            {
                recoveryTime += Time.deltaTime;
                if (recoveryTime >= 0.25f)
                {
                    // 체력회복
                    if (cd.totalstat.Hp < cd.totalstat.MaxHp)
                    {
                        // 0.25초당 2.5%회복
                        cd.totalstat.Hp += cd.totalstat.MaxHp * 0.025f;
                        if (cd.totalstat.Hp > cd.totalstat.MaxHp)
                            cd.totalstat.Hp = cd.totalstat.MaxHp;
                    }
                    // 마나회복
                    if (cd.totalstat.Mp < cd.totalstat.MaxMp)
                    {
                        // 0.25초당 2.5%회복
                        cd.totalstat.Mp += cd.totalstat.MaxMp * 0.025f;
                        if (cd.totalstat.Mp > cd.totalstat.MaxMp)
                            cd.totalstat.Mp = cd.totalstat.MaxMp;
                    }
                    recoveryTime -= 0.25f;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            // 구매불가
            PlayerData.Instance.purchaseState = false;
            // 되돌리기 불가
            PlayerData.Instance.ItemUndoListReset();
        }
    }
}
