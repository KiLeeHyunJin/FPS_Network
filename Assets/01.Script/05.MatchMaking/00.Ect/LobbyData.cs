using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyData : MonoBehaviour
{
    public enum LobbyState { Random,END}
    bool[] stats;
    private void Awake()
        => stats = new bool[(int)LobbyState.END];
    public void SetLobbyState(LobbyState key,bool state)  //해당 키에 대한 값을 수정
        => stats[(int)key] = state; 
    public bool GetLobbyState(LobbyState key)  //해당 키에 대한 값을 반환
        => stats[(int)key];
    public void ResetState(LobbyState key) //해당 키에 대한 값을 초기화
        => stats[(int)key] = false;
}
