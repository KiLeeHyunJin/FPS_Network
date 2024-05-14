using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void Activate();
    void Deactivate();
}

public class SkillController2 : MonoBehaviour
{
    private Dictionary<string, ISkill> skills = new Dictionary<string, ISkill>();

    public enum SkillType
    {
        TimeRewind,
        Mine,
        SpyCam,
        Heal,
        CloakingEffect
    }

    public void RegisterSkill(string skillName, ISkill skill)
    {
        if (!skills.ContainsKey(skillName))
        {
            skills.Add(skillName, skill);
        }
    }

    public void ActivateSkill(string skillName)
    {
        if (skills.ContainsKey(skillName))
        {
            skills[skillName].Activate();
        }
    }

    public void DeactivateSkill(string skillName)
    {
        if (skills.ContainsKey(skillName))
        {
            skills[skillName].Deactivate();
        }
    }

}
