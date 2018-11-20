using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelect : Photon.MonoBehaviour
{
    //플레이어마다 왼쪽에 있는 스펠 표시
    public Image Spell_one_Img = null;
    public Image Spell_two_Img = null;

    //동기화용 함수
    public void Spell_Image(string d_spell, string f_spell)
    {
        if (!d_spell.Equals(string.Empty))
            Spell_one_Img.sprite = Resources.Load<Sprite>("Spell/" + d_spell);
        if (!f_spell.Equals(string.Empty))
            Spell_two_Img.sprite = Resources.Load<Sprite>("Spell/" + f_spell);
    }

    private void Start()
    {
        
    }
}

