using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    private Dictionary<string, ISkill> skills = new Dictionary<string, ISkill>();

    public void RegisterSkill(ISkill skill)
    {
        if (!skills.ContainsKey(skill.SkillName))
        {
            skills.Add(skill.SkillName, skill);
        }
    }

    public void ActivateSkill(string skillName)
    {
        if (skills.ContainsKey(skillName))
        {
            skills[skillName].SkillOn();
        }
    }

    public void DeactivateSkill(string skillName)
    {
        if (skills.ContainsKey(skillName))
        {
            skills[skillName].SkillOff();
        }
    }
}