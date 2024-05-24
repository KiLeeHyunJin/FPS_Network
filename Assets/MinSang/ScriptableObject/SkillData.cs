using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float cooltimedown;
    public Sprite icon;
}