using UnityEngine;

public class UserPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UserData userData = new UserData("sad");
        FireBaseManager.DB
            .GetReference("UserData")
            .Child("id")
            .SetRawJsonValueAsync(JsonUtility.ToJson(userData));

        //FireBaseManager.DB
        //    .GetReference("UserData")
        //    .Child("LHJ")
        //    .GetValueAsync()
        //    .ContinueWithOnMainThread(task =>
        //    {
        //        if(task.IsCanceled)
        //        {

        //        }
        //        if(task.IsFaulted)
        //        {

        //        }
        //        if(task.IsCompleted)
        //        {
        //            DataSnapshot snapShot = task.Result;
        //            if(snapShot.Exists)
        //            {
        //                string json = snapShot.GetRawJsonValue();
        //                UserData userData = JsonUtility.FromJson<UserData>(json);
        //            }
        //            else
        //            {
        //                UserData userData = new UserData();
        //            }
        //        }
        //    });
    }

}
