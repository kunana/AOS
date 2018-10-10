using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : Photon.MonoBehaviour,IPunObservable {

    public int Level = 0;
    public float Exp = 0;
    public float Hp = 0;
    public float Hp_Restore_Time = 0;
    public float Mp = 0;
    public float Mp_Restore_Time = 0;
    public float Attack_Damage = 0;
    public float Ability_Power = 0;
    public float Attack_Speed = 0;
    public float Attack_Def = 0;
    public float Ability_Def = 0;
    public float CoolTime_Decrease = 0;
    public float Critical_Percentage = 0;
    public float Move_Speed = 0;
    public float Attack_Range = 0;
    public int Gold = 0;
    public float first_Create_Time = 0;
    public float Respawn_Time = 0;
    public float Exp_Increase = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
   
            stream.SendNext(this.Level);
            stream.SendNext(this.Exp);
            stream.SendNext(this.Hp);
            stream.SendNext(this.Hp_Restore_Time);
            stream.SendNext(this.Mp);
            stream.SendNext(this.Mp_Restore_Time);
            stream.SendNext(this.Attack_Damage);
            stream.SendNext(this.Ability_Power);
            stream.SendNext(this.Attack_Speed);
            stream.SendNext(this.Attack_Def);
            stream.SendNext(this.Ability_Def);
            stream.SendNext(this.CoolTime_Decrease);
            stream.SendNext(this.Critical_Percentage);
            stream.SendNext(this.Move_Speed);
            stream.SendNext(this.Attack_Range);
            stream.SendNext(this.Gold);
            stream.SendNext(this.first_Create_Time);
            stream.SendNext(this.Respawn_Time);
            stream.SendNext(this.Exp_Increase);
        }
        else
        {
            // Network player, receive data
             
            this.Level = (int)stream.ReceiveNext();
            this.Exp = (float)stream.ReceiveNext();
            this.Hp = (float)stream.ReceiveNext();
            this.Hp_Restore_Time = (float)stream.ReceiveNext();
            this.Mp = (float)stream.ReceiveNext();
            this.Mp_Restore_Time = (float)stream.ReceiveNext();
            this.Attack_Damage = (float)stream.ReceiveNext();
            this.Ability_Power = (float)stream.ReceiveNext();
            this.Attack_Speed = (float)stream.ReceiveNext();
            this.Attack_Def = (float)stream.ReceiveNext();
            this.Ability_Def = (float)stream.ReceiveNext();
            this.CoolTime_Decrease = (float)stream.ReceiveNext();
            this.Critical_Percentage = (float)stream.ReceiveNext();
            this.Move_Speed = (float)stream.ReceiveNext();
            this.Attack_Range = (float)stream.ReceiveNext();
            this.Gold = (int)stream.ReceiveNext();
            this.first_Create_Time = (float)stream.ReceiveNext();
            this.Respawn_Time = (float)stream.ReceiveNext();
            this.Exp_Increase = (float)stream.ReceiveNext();

        }
    }
}
