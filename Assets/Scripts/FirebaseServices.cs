using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using Newtonsoft.Json.Linq;

public class FirebaseServices : MonoBehaviour
{
    private static DatabaseReference reference;
    // Start is called before the first frame update
    void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                reference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    public static IEnumerator WriteData(string collectionName, Dictionary<string, object> data)
    {
        var getDataTask = reference.Child(collectionName).OrderByKey().LimitToLast(1).GetValueAsync();

        yield return new WaitUntil(() => getDataTask.IsCompleted);

        if (getDataTask.Exception != null)
        {
            Debug.LogError($"Failed to find last ID in {collectionName} collection: {getDataTask.Exception}");
            yield break;
        }

        DataSnapshot snapshot = getDataTask.Result;
        int newId = 1; // Default to 1 if no data exists

        // Find the next available ID
        foreach (DataSnapshot child in snapshot.Children)
        {
            int lastId = int.Parse(child.Key);
            newId = lastId + 1;
        }

        string documentId = newId.ToString();
        DatabaseReference docRef = reference.Child(collectionName).Child(documentId);

        var setDataTask = docRef.SetValueAsync(data);
        yield return new WaitUntil(() => setDataTask.IsCompleted);

        if (setDataTask.Exception != null)
        {
            Debug.LogError($"Failed to add {documentId} to {collectionName} collection: {setDataTask.Exception}");
        }
        else
        {
            Debug.Log($"{documentId} added to {collectionName} collection.");
        }
    }

    public static IEnumerator ReadData(string collectionName, System.Action<JArray> callback)
    {
        yield return reference.Child(collectionName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error reading data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                // Get data from snapshot
                DataSnapshot snapshot = task.Result;

                if (snapshot.Value is IList<object> list)
                {
                    // Transform data list to JArray
                    JArray jArray = new JArray();
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        var jsonObject = JObject.FromObject(childSnapshot.Value);
                        jsonObject["id"] = childSnapshot.Key;
                        jArray.Add(jsonObject);
                    }

                    callback(jArray);
                }
                else
                {
                    Debug.LogError("Data format is not a list.");
                    callback(null);
                }
            }
        });
    }
}