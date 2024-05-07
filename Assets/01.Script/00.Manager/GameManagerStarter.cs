using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Manager.Game.Spawn(null);
        Manager.Game.StartGame();
    }
}
