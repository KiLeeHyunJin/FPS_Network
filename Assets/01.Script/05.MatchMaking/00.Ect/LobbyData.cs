using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyData : MonoBehaviour
{
    public enum LobbyState { Random,END}
    bool[] stats;
    private void Awake()
        => stats = new bool[(int)LobbyState.END];
    public void SetLobbyState(LobbyState key,bool state) 
        => stats[(int)key] = state;
    public bool GetLobbyState(LobbyState key) 
        => stats[(int)key];
    public void ResetState(LobbyState key)
        => stats[(int)key] = false;
}
