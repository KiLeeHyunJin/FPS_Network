using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;


public interface ISkill
{
    void Activate();
    void Deactivate();
}

public class SkillController2 : MonoBehaviourPun
{
    private Dictionary<string, ISkill> skills = new Dictionary<string, ISkill>();

    public enum Skill
    {
        TimeRewind,
        CloakingEffect,
        SpyCamController,
        MineSkill,
        Heal
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void RegisterSkill(string skillName, ISkill skill) // 스킬 등록
    {
        if (!skills.ContainsKey(skillName))
        {
            skills.Add(skillName, skill);
        }
    }

    public void ActivateSkill(string skillName) // 스킬 활성화
    {
        if (skills.ContainsKey(skillName))
        {
            skills[skillName].Activate();
        }
    }

    public void DeactivateSkill(string skillName) // 스킬 비활성화
    {
        if (skills.ContainsKey(skillName))
        {
            skills[skillName].Deactivate();
        }
    }
    
    public void RegisterDefaultSkills()
    {
        RegisterSkill(Skill.TimeRewind.ToString(), new TimeRewind());
        RegisterSkill(Skill.SpyCamController.ToString(), new SpyCamController());
        RegisterSkill(Skill.CloakingEffect.ToString(), new CloakingEffect());
        RegisterSkill(Skill.MineSkill.ToString(), new MineSkill());
        RegisterSkill(Skill.Heal.ToString(), new Heal());
    }
} 
