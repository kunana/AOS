using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ward : MonoBehaviour
{
    const float minTime = 90f;
    const float maxTime = 180f;
    const float termTime = 90f;
    float fCooldown = 90f;
    public bool isWardOn = false;
    public BushJoinScript myBush = null;
    public string team = "";
    public string playerTeam = "";
    SkinnedMeshRenderer myMesh;
    GameObject particle;
    private void Awake()
    {
        myMesh = GetComponent<SkinnedMeshRenderer>();
        particle = transform.GetChild(0).gameObject;
        playerTeam = PhotonNetwork.player.GetTeam().ToString();
        if (playerTeam.Equals("red"))
            playerTeam = "Red";
        else if (playerTeam.Equals("blue"))
            playerTeam = "Blue";
        particle.SetActive(false);
    }

    public void MakeWard(string _team, int champLv)
    {
        fCooldown = Mathf.Round(minTime + ((termTime * ((float)(champLv - 1))) / 17f));
        team = _team;
        isWardOn = true;

        if (team.Equals(playerTeam))
        {
            myMesh.enabled = true;
            particle.SetActive(true);
        }
    }

    private void Update()
    {
        if (isWardOn)
        {
            fCooldown -= Time.deltaTime;
            if (fCooldown < 0)
            {
                WardTimeOut();
            }
        }
    }

    private void WardTimeOut()
    {
        transform.position = new Vector3(-50, 0, -50);
        Invoke("Dead", 1f);
    }

    private void Dead()
    {
        team = "";
        myBush = null;
        isWardOn = false;
        gameObject.SetActive(false);
    }
}