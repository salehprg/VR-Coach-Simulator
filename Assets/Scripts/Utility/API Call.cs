using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Networking;

public delegate void Callback(UnityWebRequest.Result result, string message);

public class APICall
{
    string authToken = "";
    string AuthHeader = "";
    public APICall(string authToken, string AuthHeader = "Authorization")
    {
        this.authToken = authToken;
        this.AuthHeader = AuthHeader;
    }

    public IEnumerator Get(string uri, Callback callback)
    {
        using UnityWebRequest request = UnityWebRequest.Get(uri);

        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader(AuthHeader, authToken);
        }

        yield return request.SendWebRequest();

        callback(request.result, request.downloadHandler.text);
    }

    public IEnumerator Post(string uri, object data, Callback callback)
    {
        string jsonData = JsonUtility.ToJson(data);

        using UnityWebRequest request = UnityWebRequest.Post(uri, jsonData, "application/json");

        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader(AuthHeader, authToken);
        }

        yield return request.SendWebRequest();

        callback(request.result, request.downloadHandler.text);
    }
}
