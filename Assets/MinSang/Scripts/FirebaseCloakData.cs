using Firebase.Database;

public class FirebaseCloakData
{
    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

    public void SaveCloakData(string userId, bool isCloaked)
    {
        reference.Child("users").Child(userId).Child("cloaking").SetValueAsync(isCloaked);
    }
}
