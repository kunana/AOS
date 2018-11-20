using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillClass {
    private static SkillClass _instance;
    public static SkillClass instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillClass();
            }
            return _instance;
        }
    }

    private SkillClass()
    {
        ReadCsv();
    }

    public class Skill
    {
        public string championName = "";

        public string passiveName = "";
        public string passiveDescription = "";
        public float passiveCooldown = 0;
        public float passiveDamage = 0;
        public string passiveAstat = "";
        public float passiveAvalue = 0;

        public string qName = "";
        public string qDescription = "";
        public float qRange = 0;
        public float[] qMana = new float[5];
        public float[] qCooldown = new float[5];
        public float[] qDamage = new float[5];
        public string qAstat = "";
        public float qAvalue = 0;

        public string wName = "";
        public string wDescription = "";
        public float wRange = 0;
        public float[] wMana = new float[5];
        public float[] wCooldown = new float[5];
        public float[] wDamage = new float[5];
        public string wAstat = "";
        public float wAvalue = 0;

        public string eName = "";
        public string eDescription = "";
        public float eRange = 0;
        public float[] eMana = new float[5];
        public float[] eCooldown = new float[5];
        public float[] eDamage = new float[5];
        public string eAstat = "";
        public float eAvalue = 0;

        public string rName = "";
        public string rDescription = "";
        public float rRange = 0;
        public float[] rMana = new float[3];
        public float[] rCooldown = new float[3];
        public float[] rDamage = new float[3];
        public string rAstat = "";
        public float rAvalue = 0;

        public Skill ClassCopy()
        {
            return (Skill)this.MemberwiseClone();
        }
    }

    public class Skill2
    {
        public string Name = "";
        public string Description = "";
        public float Range = 0;
        public float[] Mana = new float[5];
        public float[] Cooldown = new float[5];
        public float[] Damage = new float[5];
        public string Astat = "";
        public float Avalue = 0;
        public int skillLevel = 0;
    }

    public Dictionary<string, Skill> skillData = new Dictionary<string, Skill>();

    public void ReadCsv()
    {
        skillData.Clear();

        string fileName = Application.streamingAssetsPath;
        fileName = Path.Combine(fileName, "csv/aos_skill.csv");
        if (File.Exists(fileName) == false)
            return;

        FileStream fStream = new FileStream(fileName, FileMode.Open);
        if (fStream != null)
        {
            StreamReader streamReader = new StreamReader(fStream);
            string skillcsv = streamReader.ReadToEnd();
            string[] lines = skillcsv.Split("\r\n".ToCharArray());

            foreach (string line in lines)
            {
                Skill newskill = new Skill();

                if (line.Length > 0)
                {
                    string[] data = line.Split(',');

                    newskill.championName = data[0];

                    newskill.passiveName = data[1];
                    newskill.passiveDescription = data[2];
                    newskill.passiveCooldown = float.Parse(data[3]);
                    newskill.passiveDamage = float.Parse(data[4]);
                    newskill.passiveAstat = data[5];
                    newskill.passiveAvalue = float.Parse(data[6]);

                    newskill.qName = data[7];
                    newskill.qDescription = data[8];
                    newskill.qRange = float.Parse(data[9]);
                    newskill.qMana[0] = float.Parse(data[10]);
                    newskill.qMana[1] = float.Parse(data[11]);
                    newskill.qMana[2] = float.Parse(data[12]);
                    newskill.qMana[3] = float.Parse(data[13]);
                    newskill.qMana[4] = float.Parse(data[14]);
                    newskill.qDamage[0] = float.Parse(data[15]);
                    newskill.qDamage[1] = float.Parse(data[16]);
                    newskill.qDamage[2] = float.Parse(data[17]);
                    newskill.qDamage[3] = float.Parse(data[18]);
                    newskill.qDamage[4] = float.Parse(data[19]);
                    newskill.qCooldown[0] = float.Parse(data[20]);
                    newskill.qCooldown[1] = float.Parse(data[21]);
                    newskill.qCooldown[2] = float.Parse(data[22]);
                    newskill.qCooldown[3] = float.Parse(data[23]);
                    newskill.qCooldown[4] = float.Parse(data[24]);
                    if(data[25] != string.Empty)
                        newskill.qAstat = data[25];
                    newskill.qAvalue = float.Parse(data[26]);

                    newskill.wName = data[27];
                    newskill.wDescription = data[28];
                    newskill.wRange = float.Parse(data[29]);
                    newskill.wMana[0] = float.Parse(data[30]);
                    newskill.wMana[1] = float.Parse(data[31]);
                    newskill.wMana[2] = float.Parse(data[32]);
                    newskill.wMana[3] = float.Parse(data[33]);
                    newskill.wMana[4] = float.Parse(data[34]);
                    newskill.wDamage[0] = float.Parse(data[35]);
                    newskill.wDamage[1] = float.Parse(data[36]);
                    newskill.wDamage[2] = float.Parse(data[37]);
                    newskill.wDamage[3] = float.Parse(data[38]);
                    newskill.wDamage[4] = float.Parse(data[39]);
                    newskill.wCooldown[0] = float.Parse(data[40]);
                    newskill.wCooldown[1] = float.Parse(data[41]);
                    newskill.wCooldown[2] = float.Parse(data[42]);
                    newskill.wCooldown[3] = float.Parse(data[43]);
                    newskill.wCooldown[4] = float.Parse(data[44]);
                    if (data[45] != string.Empty)
                        newskill.wAstat = data[45];
                    newskill.wAvalue = float.Parse(data[46]);

                    newskill.eName = data[47];
                    newskill.eDescription = data[48];
                    newskill.eRange = float.Parse(data[49]);
                    newskill.eMana[0] = float.Parse(data[50]);
                    newskill.eMana[1] = float.Parse(data[51]);
                    newskill.eMana[2] = float.Parse(data[52]);
                    newskill.eMana[3] = float.Parse(data[53]);
                    newskill.eMana[4] = float.Parse(data[54]);
                    newskill.eDamage[0] = float.Parse(data[55]);
                    newskill.eDamage[1] = float.Parse(data[56]);
                    newskill.eDamage[2] = float.Parse(data[57]);
                    newskill.eDamage[3] = float.Parse(data[58]);
                    newskill.eDamage[4] = float.Parse(data[59]);
                    newskill.eCooldown[0] = float.Parse(data[60]);
                    newskill.eCooldown[1] = float.Parse(data[61]);
                    newskill.eCooldown[2] = float.Parse(data[62]);
                    newskill.eCooldown[3] = float.Parse(data[63]);
                    newskill.eCooldown[4] = float.Parse(data[64]);
                    if (data[65] != string.Empty)
                        newskill.eAstat = data[65];
                    newskill.eAvalue = float.Parse(data[66]);

                    newskill.rName = data[67];
                    newskill.rDescription = data[68];
                    newskill.rRange = float.Parse(data[69]);
                    newskill.rMana[0] = float.Parse(data[70]);
                    newskill.rMana[1] = float.Parse(data[71]);
                    newskill.rMana[2] = float.Parse(data[72]);
                    newskill.rDamage[0] = float.Parse(data[73]);
                    newskill.rDamage[1] = float.Parse(data[74]);
                    newskill.rDamage[2] = float.Parse(data[75]);
                    newskill.rCooldown[0] = float.Parse(data[76]);
                    newskill.rCooldown[1] = float.Parse(data[77]);
                    newskill.rCooldown[2] = float.Parse(data[78]);
                    if (data[79] != string.Empty)
                        newskill.rAstat = data[79];
                    newskill.rAvalue = float.Parse(data[80]);

                    skillData[newskill.championName] = newskill;
                }
            }
            streamReader.Close();
            fStream.Close();
        }
    }
}
