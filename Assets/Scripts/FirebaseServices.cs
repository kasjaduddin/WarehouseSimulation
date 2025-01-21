using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
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

    public static IEnumerator WriteData(string collectionName, Dictionary<string, object> data, System.Action<string> callback = null)
    {
        string message = null;

        // Get new id
        var getDataTask = reference.Child(collectionName).OrderByKey().LimitToLast(1).GetValueAsync();
        yield return new WaitUntil(() => getDataTask.IsCompleted);

        if (getDataTask.Exception != null)
        {
            message = $"Failed to find last ID in {collectionName} collection: {getDataTask.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        DataSnapshot getIdSnapshot = getDataTask.Result;
        int newId = 1;
        foreach (DataSnapshot child in getIdSnapshot.Children)
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
            message = $"Failed to add new bin to {collectionName} collection: {setDataTask.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
        }
        else
        {
            message = $"New {collectionName.ToLower().Remove(collectionName.Length - 1)} successfully added to {collectionName} collection.";
            Debug.Log(message);
            callback?.Invoke(message);
        }
    }

    public static IEnumerator WriteData(string collectionName, Dictionary<string, object> data, string primaryKey, System.Action<string> callback = null)
    {
        string message = null;

        // Check primary key
        var checkTask = reference.Child(collectionName).OrderByChild(primaryKey).EqualTo(data[primaryKey].ToString()).GetValueAsync();
        yield return new WaitUntil(() => checkTask.IsCompleted);

        if (checkTask.Exception != null)
        {
            message = $"Error checking for duplicate: {checkTask.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        DataSnapshot duplicateCheckSnapshot = checkTask.Result;
        if (duplicateCheckSnapshot.Exists)
        {
            message = $"{collectionName.Remove(collectionName.Length - 1)} with {primaryKey} {data[primaryKey]} has been registered.\r\nReplace {collectionName.Remove(collectionName.Length - 1)} data?";
            Debug.LogWarning(message);
            callback?.Invoke(message);
            yield break;
        }

        // Get new id
        yield return WriteData(collectionName, data, callback);
    }

    public static IEnumerator WriteData(string collectionName, Dictionary<string, object> data, string firstPrimaryKey, string secondPrimaryKey, System.Action<string> callback = null)
    {
        string message = null;

        // Check first primary key
        var checkTask1 = reference.Child(collectionName).OrderByChild(firstPrimaryKey).EqualTo(data[firstPrimaryKey].ToString()).GetValueAsync();
        yield return new WaitUntil(() => checkTask1.IsCompleted);

        if (checkTask1.Exception != null)
        {
            message = $"Error checking for duplicate: {checkTask1.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        DataSnapshot duplicateCheckSnapshot1 = checkTask1.Result;
        if (duplicateCheckSnapshot1.Exists)
        {
            message = $"{collectionName.Remove(collectionName.Length - 1)} with {firstPrimaryKey} {data[firstPrimaryKey]} has been registered.\r\nReplace {collectionName.Remove(collectionName.Length - 1)} data?";
            Debug.LogWarning(message);
            callback?.Invoke(message);
            yield break;
        }

        // Check second primary key
        var checkTask2 = reference.Child(collectionName).OrderByChild(secondPrimaryKey).EqualTo(data[secondPrimaryKey].ToString()).GetValueAsync();
        yield return new WaitUntil(() => checkTask2.IsCompleted);

        if (checkTask2.Exception != null)
        {
            message = $"Error checking for duplicate: {checkTask2.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        DataSnapshot duplicateCheckSnapshot2 = checkTask2.Result;
        if (duplicateCheckSnapshot2.Exists)
        {
            message = $"{collectionName.Remove(collectionName.Length - 1)} with {secondPrimaryKey} {data[secondPrimaryKey]} has been registered.";
            Debug.LogWarning(message);
            callback?.Invoke(message);
            yield break;
        }

        // Get new id
        yield return WriteData(collectionName, data, callback);
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
                JArray jArray = new JArray();
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    var jsonObject = JObject.FromObject(childSnapshot.Value);
                    jArray.Add(jsonObject);
                }

                callback(jArray);
            }
        });
    }

    public static IEnumerator ModifyData(string collectionName, Dictionary<string, object> newData, System.Action<string> callback = null)
    {
        string message = null;

        if (!newData.ContainsKey("id"))
        {
            message = "Data does not contain 'id' field.";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        string documentId = newData["id"].ToString();
        DatabaseReference docRef = reference.Child(collectionName).Child(documentId);

        // Delete id from newData so that it is not updated in the document
        newData.Remove("id");

        // Update data in Firebase
        var updateTask = docRef.UpdateChildrenAsync(newData);
        yield return new WaitUntil(() => updateTask.IsCompleted);

        if (updateTask.Exception != null)
        {
            message = $"Failed to update data with id {documentId} in {collectionName} collection: {updateTask.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
        }
        else
        {
            message = $"Data with id {documentId} successfully updated in {collectionName} collection.";
            Debug.Log(message);
            callback?.Invoke(message);
        }
    }

    public static IEnumerator ModifyData(string collectionName, Dictionary<string, object> newData, string oldKey, string primaryKey, System.Action<string> callback = null)
    {
        string message = null;

        if (!newData.ContainsKey("id"))
        {
            message = "Data does not contain 'id' field.";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        string newKey = newData.ContainsKey(primaryKey) ? newData[primaryKey].ToString() : null;

        // Check for duplication if the new key is different from the last key
        if (!string.IsNullOrEmpty(newKey) && newKey != oldKey)
        {
            var checkTask = reference.Child(collectionName).OrderByChild(primaryKey).EqualTo(newKey).GetValueAsync();
            yield return new WaitUntil(() => checkTask.IsCompleted);

            if (checkTask.Exception != null)
            {
                message = $"Error checking for duplicate: {checkTask.Exception}";
                Debug.LogError(message);
                callback?.Invoke(message);
                yield break;
            }

            DataSnapshot duplicateCheckSnapshot = checkTask.Result;
            if (duplicateCheckSnapshot.Exists)
            {
                message = $"{collectionName.Remove(collectionName.Length - 1)} with {primaryKey} {newData[primaryKey]} has been registered.\r\nReplace bin data?";
                Debug.LogWarning(message);
                callback?.Invoke(message);
                yield break;
            }
        }

        yield return ModifyData(collectionName, newData, callback);
    }

    public static IEnumerator ModifyData(string collectionName, Dictionary<string, object> newData, string oldFirstPrimaryKey, string firstPrimaryKey, string oldSecondPrimaryKey, string secondPrimaryKey, System.Action<string> callback = null)
    {
        string message = null;

        if (!newData.ContainsKey("id"))
        {
            message = "Data does not contain 'id' field.";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        string newKey1 = newData.ContainsKey(firstPrimaryKey) ? newData[firstPrimaryKey].ToString() : null;
        string newKey2 = newData.ContainsKey(secondPrimaryKey) ? newData[secondPrimaryKey].ToString() : null;

        // Check first primary key
        if (!string.IsNullOrEmpty(newKey1) && newKey1 != oldFirstPrimaryKey)
        {
            var checkTask1 = reference.Child(collectionName).OrderByChild(firstPrimaryKey).EqualTo(newKey1).GetValueAsync();
            yield return new WaitUntil(() => checkTask1.IsCompleted);

            if (checkTask1.Exception != null)
            {
                message = $"Error checking for duplicate: {checkTask1.Exception}";
                Debug.LogError(message);
                callback?.Invoke(message);
                yield break;
            }

            DataSnapshot duplicateCheckSnapshot1 = checkTask1.Result;
            if (duplicateCheckSnapshot1.Exists)
            {
                message = $"{collectionName.Remove(collectionName.Length - 1)} with {firstPrimaryKey} {newData[firstPrimaryKey]} has been registered.\r\nReplace bin data?";
                Debug.LogWarning(message);
                callback?.Invoke(message);
                yield break;
            }
        }

        // Check second primary key
        if (!string.IsNullOrEmpty(newKey2) && newKey2 != oldSecondPrimaryKey)
        {
            var checkTask2 = reference.Child(collectionName).OrderByChild(secondPrimaryKey).EqualTo(newKey2).GetValueAsync();
            yield return new WaitUntil(() => checkTask2.IsCompleted);

            if (checkTask2.Exception != null)
            {
                message = $"Error checking for duplicate: {checkTask2.Exception}";
                Debug.LogError(message);
                callback?.Invoke(message);
                yield break;
            }

            DataSnapshot duplicateCheckSnapshot2 = checkTask2.Result;
            if (duplicateCheckSnapshot2.Exists)
            {
                message = $"{collectionName.Remove(collectionName.Length - 1)} with {secondPrimaryKey} {newData[secondPrimaryKey]} has been registered.\r\nReplace bin data?";
                Debug.LogWarning(message);
                callback?.Invoke(message);
                yield break;
            }
        }

        yield return ModifyData(collectionName, newData, callback);
    }

    public static IEnumerator ModifyData(string collectionName, Dictionary<string, object> newData, bool checkForDuplicate = true, string oldKey = "", string primaryKey = "", System.Action<string> callback = null)
    {
        string message = null;

        if (!newData.ContainsKey("id"))
        {
            message = "Data does not contain 'id' field.";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        if (checkForDuplicate)
        {
            string newKey = newData.ContainsKey(primaryKey) ? newData[primaryKey].ToString() : null;

            // Check for duplication if the new key is different from the last key
            if (!string.IsNullOrEmpty(newKey) && newKey != oldKey)
            {
                var checkTask = reference.Child(collectionName).OrderByChild(primaryKey).EqualTo(newKey).GetValueAsync();
                yield return new WaitUntil(() => checkTask.IsCompleted);

                if (checkTask.Exception != null)
                {
                    message = $"Error checking for duplicate: {checkTask.Exception}";
                    Debug.LogError(message);
                    callback?.Invoke(message);
                    yield break;
                }

                DataSnapshot duplicateCheckSnapshot = checkTask.Result;
                if (duplicateCheckSnapshot.Exists)
                {
                    message = $"{collectionName.Remove(collectionName.Length - 1)} with {primaryKey} {newData[primaryKey]} has been registered.\r\nReplace bin data?";
                    Debug.LogWarning(message);
                    callback?.Invoke(message);
                    yield break;
                }
            }
        }

        string documentId = newData["id"].ToString();
        DatabaseReference docRef = reference.Child(collectionName).Child(documentId);

        // Delete id from newData so that it is not updated in the document
        newData.Remove("id");

        // Update data in Firebase
        var updateTask = docRef.UpdateChildrenAsync(newData);
        yield return new WaitUntil(() => updateTask.IsCompleted);

        if (updateTask.Exception != null)
        {
            message = $"Failed to update data with id {documentId} in {collectionName} collection: {updateTask.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
        }
        else
        {
            message = $"Data with id {documentId} successfully updated in {collectionName} collection.";
            Debug.Log(message);
            callback?.Invoke(message);
        }
    }

    public static IEnumerator DeleteData(string collectionName, string primaryKey, string key, System.Action<string> callback = null)
    {
        string message = null;

        // Mencari data dengan primaryKey dan key yang sesuai
        var checkTask = reference.Child(collectionName).OrderByChild(primaryKey).EqualTo(key).GetValueAsync();
        yield return new WaitUntil(() => checkTask.IsCompleted);

        if (checkTask.Exception != null)
        {
            message = $"Error checking for {primaryKey} {key}: {checkTask.Exception}";
            Debug.LogError(message);
            callback?.Invoke(message);
            yield break;
        }

        DataSnapshot snapshot = checkTask.Result;
        if (snapshot.Exists)
        {
            // Menghapus data yang ditemukan
            foreach (DataSnapshot child in snapshot.Children)
            {
                string documentId = child.Key;
                var deleteTask = reference.Child(collectionName).Child(documentId).RemoveValueAsync();
                yield return new WaitUntil(() => deleteTask.IsCompleted);

                if (deleteTask.Exception != null)
                {
                    message = $"Failed to delete data with {primaryKey} {key} in {collectionName} collection: {deleteTask.Exception}";
                    Debug.LogError(message);
                    callback?.Invoke(message);
                    yield break;
                }
            }
            message = $"Data with {primaryKey} {key} successfully deleted in {collectionName} collection.";
            Debug.Log(message);
            callback?.Invoke(message);
        }
        else
        {
            message = $"No data found with {primaryKey} {key} in {collectionName} collection.";
            Debug.LogWarning(message);
            callback?.Invoke(message);
        }
    }
}