using UnityEngine;
using Photon.Pun;
public interface ISkill
{
    string SkillName { get; set; }
    float Cooltimedown { get; set; }
    void SkillOn();
    void SkillOff();
}
public abstract class Skill : MonoBehaviourPun, ISkill
{
    public SkillData skillData;

    public string SkillName
    {
        get { return skillData.skillName; }
        set { skillData.skillName = value; }
    }

    public float Cooltimedown
    {
        get { return skillData.cooltimedown; }
        set { skillData.cooltimedown = value; }
    }

    public abstract void SkillOn();
    public virtual void SkillOff() { }
}