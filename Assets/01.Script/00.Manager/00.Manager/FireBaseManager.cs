
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;

public class FireBaseManager : Singleton<FireBaseManager>
{
    private static Firebase.FirebaseApp app;
    private static Firebase.Auth.FirebaseAuth auth;
    public static Firebase.FirebaseApp App { get { return app; } }
    public static Firebase.Auth.FirebaseAuth Auth { get { return auth; } }
    public static Firebase.Database.FirebaseDatabase db;
    public static Firebase.Database.FirebaseDatabase DB { get { return db; } }

    protected override void Awake()
    {
        base.Awake();
        CheckFirebaseAvailable();
    }
    void CheckFirebaseAvailable()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(FirebaseAvailableCheck);
    }

    void FirebaseAvailableCheck(Task<Firebase.DependencyStatus> task)
    {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            //app = Firebase.FirebaseApp.DefaultInstance;

            // Set a flag here to indicate whether Firebase is ready to use by your app.
            Debug.Log("Firebase Check Success");
            app = Firebase.FirebaseApp.DefaultInstance;
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            db = Firebase.Database.FirebaseDatabase.DefaultInstance;
        }
        else
        {
            UnityEngine.Debug.LogError(System.String.Format(
              "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            app = null;
            auth = null;
            db = null;
            // Firebase Unity SDK is not safe to use here.
        }
    }

}
