using Firebase;
using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Firebase.Database;

public class SkillController2 : MonoBehaviourPun
{
    public Heal heal;
    public MineSkill mineSkill;
    public CloakingEffect cloakingEffect;
    public TimeRewind timeRewind;
    public SpyCamController spyCamController;

    private DatabaseReference databaseReference;
    void Start()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        });

        // 스킬 초기화
        heal = GetComponent<Heal>();
        mineSkill = GetComponent<MineSkill>();
        spyCamController = GetComponent<SpyCamController>();
        timeRewind = GetComponent<TimeRewind>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) cloakingEffect.Activate();
        if (Input.GetKeyDown(KeyCode.H)) heal.Activate();
        if (Input.GetKeyDown(KeyCode.M)) mineSkill.Activate();
        if (Input.GetKeyDown(KeyCode.F)) spyCamController.Activate();
    }

    public void UseSkill(string skillName)
    {
        switch (skillName)
        {
            case "Heal":
                heal.Activate();
                break;
            case "MineSkill":
                mineSkill.Activate();
                break;
            case "SpyCam":
                spyCamController.Activate();
                break;
            case "TimeRewind":
                break;
            default:
                Debug.LogError("Unknown skill: " + skillName);
                break;
        }

        LogSkillUse(skillName);
    }

    void LogSkillUse(string skillName)
    {
        string userId = PhotonNetwork.LocalPlayer.UserId;
        string key = databaseReference.Child("skills").Push().Key;
        SkillLog skillLog = new SkillLog(userId, skillName, System.DateTime.Now.ToString());
        string json = JsonUtility.ToJson(skillLog);

        databaseReference.Child("skills").Child(key).SetRawJsonValueAsync(json);
    }
}

public class SkillLog
{
    public string userId;
    public string skillName;
    public string timestamp;

    public SkillLog(string userId, string skillName, string timestamp)
    {
        this.userId = userId;
        this.skillName = skillName;
        this.timestamp = timestamp;
    }
}
