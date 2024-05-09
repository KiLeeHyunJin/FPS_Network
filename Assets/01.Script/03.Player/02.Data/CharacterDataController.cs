using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataController : MonoBehaviour
{
    CharacterController controller;
    CharacterData data;
    Action[] useItemCallBackMethod;
    Coroutine itemCo                        { get; set; }
    public Define.Weapon GetCurrentWeapon   { get   { return data.GetCurrentWeaponType; } }
    public bool IsAction                    { get   { return itemCo != null ? true : false;}}
    public int SetCurrentWeaponNum          { set   { data.CurrentWeaponNum = value; } }
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        useItemCallBackMethod = new Action[2];
        data = new CharacterData();
    }
    public int AddItem(int count, int weight, ItemBase item)
    {
        if(data.bagValue >= count * weight)
        {
            data.AddItem(count, item);
            return 0;
        }
        else
        {
            int possibleCount = data.bagValue / weight;
            data.AddItem(possibleCount, item);
            return count - possibleCount;
        }
    }


    public void SetMinusDefensiveGear(int _damage, Define.Wearable wearType)
        => data.MinusDefValue(_damage, wearType);

    public void StartAction(float useTime, Action successMethod, Action cancledMethod = null)
    {
        useItemCallBackMethod[0] = successMethod;
        useItemCallBackMethod[1] = cancledMethod;
        itemCo = StartCoroutine(UsingItemRoutine(useTime));
    }

    public Action Reload()
        => data.Reload;
    void ActionCancled()
        => useItemCallBackMethod[1]?.Invoke();
    void ActionSuccessed()
        => useItemCallBackMethod[0]?.Invoke();
    IEnumerator UsingItemRoutine(float useTime)
    {
        float time = 0;
        bool cancle = false;
        while(useTime > time)
        {
            time += Time.deltaTime;
            if (controller.isRun || controller.isJumping)
            {
                cancle = true;
                break;
            }
            yield return null;
        }
        if (cancle)
            ActionCancled();
        else
            ActionSuccessed();
    }


}
