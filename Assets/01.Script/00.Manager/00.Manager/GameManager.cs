
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public Transform redTeamSpawner;
    [SerializeField] public Transform blueTeamSpawner;
    [SerializeField] int spawnRadius = 5;
    [SerializeField] GameObject systemMessagePrefab;
    [SerializeField] Transform messageTransform;
    [SerializeField] ScrollRect systemRect;

    const int BLUE = 1;
    const int RED = 2;

    UserData userData;

    public UserData UserData { get { return userData; } }




    protected override void Awake()
    {
        userData = new UserData();
        base.Awake();
        spawnRadius = 5;
    }
    public void CreateUserData()
    {
        userData = new UserData();
        string json = JsonUtility.ToJson(userData);

        FireBaseManager.DB
            .GetReference("UserData").Child(FireBaseManager.Auth.CurrentUser.UserId)
            .SetRawJsonValueAsync(json);
    }
    public void GetUserData()
    {
        FireBaseManager.DB
               .GetReference("UserData")
               .Child(FireBaseManager.Auth.CurrentUser.UserId)
               .GetValueAsync().ContinueWithOnMainThread(task =>
               {
                   if (task.IsCanceled)
                   {
                       Debug.Log("cancle");
                       return;
                   }
                   else if (task.IsFaulted)
                   {
                       Debug.Log("fault");
                       return;
                   }
                   DataSnapshot snapshot = task.Result;
                   if (snapshot.Exists)
                   {
                       string json = snapshot.GetRawJsonValue();
                       userData = JsonUtility.FromJson<UserData>(json); 
                   }
               });
    }
    public void Spawn(Transform blue, Transform red)
    {
        redTeamSpawner = red;
        blueTeamSpawner = blue;
        Vector3 randomDir = Random.onUnitSphere;
        Vector3 randomPos = randomDir * spawnRadius;

        Vector3 redSpawnPos = red.position + randomPos;
        redSpawnPos.y = 1f;
        Vector3 blueSpawnPos = blue.position + randomPos;
        blueSpawnPos.y = 1f;

        redTeamSpawner.position = redSpawnPos;
        blueTeamSpawner.position = blueSpawnPos;
    }


    public void StartGame(Transform b, Transform r)
    {
        Debug.Log("isStart");
        Spawn(b, r);
        if (BLUE == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            PhotonNetwork.Instantiate("Player", blueTeamSpawner.position, blueTeamSpawner.rotation, 0);
        else
            PhotonNetwork.Instantiate("Player", redTeamSpawner.position, redTeamSpawner.rotation, 0);

    }


    public void ShowMessage(string message)
    {
        if (this == null || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("GameManager is destroyed or inactive, ignoring ShowMessage RPC.");
            return;
        }
        GameObject instance = Instantiate(systemMessagePrefab, messageTransform);
        TMP_Text text = instance.GetComponentInChildren<TMP_Text>();
        text.text = message;
        CanvasGroup canvasGroup = instance.GetComponent<CanvasGroup>();
        StartCoroutine(MessageFading(canvasGroup));
        StartCoroutine(MessageDown());


        IEnumerator MessageDown()
        {
            float startingValue = systemRect.verticalScrollbar.value;
            float targetValue = 0f;
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                systemRect.verticalScrollbar.value = Mathf.Lerp(startingValue, targetValue, t / 1f);
                yield return null;
            }
            systemRect.verticalScrollbar.value = targetValue;
        }
        IEnumerator MessageFading(CanvasGroup canvas)
        {
            float duration = 1f;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = t / duration;
                yield return null;
            }
            canvasGroup.alpha = 1f;


            yield return new WaitForSeconds(3f);


            float disDur = 1f;
            for (float t = 0; t < disDur; t += Time.deltaTime)
            {
                canvasGroup.alpha = 1 - t / disDur;
                yield return null;
            }
            canvasGroup.alpha = 0f;


            Destroy(canvasGroup.gameObject);

        }
    }


}
