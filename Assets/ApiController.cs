using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;
using UnityEngine.Networking;

public class ApiController : MonoBehaviour
{

    public string uploadImagesUrl = "http://127.0.0.1:5000/uploadImages";
    public GameObject imageClassGameObject;

    // Start is called before the first frame update
    void Start()
    {
        //CheckApiData();
        EventsManager.instance.UploadImagesTrigger += requestImageUpload;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void requestImageUpload(int id)
    {
        Debug.Log("Start Images Upload");
        ImageClassificationScript imageClassification = imageClassGameObject.GetComponent<ImageClassificationScript>();
        List<string> framesList = imageClassification.framesList;
        StartCoroutine(UploadImages(framesList, uploadImagesUrl));
    }

    public static IEnumerator Login(string username, string password, string url)
    {
        LoginRequest requestBody = new LoginRequest();
        requestBody.email = username;
        requestBody.password = password;
        requestBody.remember_login = false;

        string requestString = JsonUtility.ToJson(requestBody) ?? "";
        UnityWebRequest www = UnityWebRequest.Put(url, requestString);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Dictionary<string, string> dict = www.GetResponseHeaders();
            foreach (KeyValuePair<string, string> header in dict)
            {
                Debug.Log(header.Key + " = " + header.Value);
            }

            Debug.Log("Login complete!");
        }


        /*WWWForm form = new WWWForm();
        form.AddField("login", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Dictionary<string, string> dict = www.GetResponseHeaders();
            foreach (KeyValuePair<string, string> header in dict)
            {
                Debug.Log(header.Key + " = " + header.Value);
            }

            Debug.Log("Form upload complete!");
        }*/
    }

    public static IEnumerator UploadImages(List<string> frames, string url)
    {
        WWWForm form = new WWWForm();
        for (int i = 0; i < frames.Count; i++)
        {
            form.AddField("frame" + i, frames[i]);
        }

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }

    }

    /*public void CheckApiData()
    {
        //ImageClassificationScript imageClassification = imageClassGameObject.GetComponent<ImageClassificationScript>();
        //imageClassification.apiResponse = GetApiData();
        StartCoroutine(GetApiDataUnity());
    }

    IEnumerator GetApiDataUnity()
    {
        UnityWebRequest www = UnityWebRequest.Get(String.Format("http://127.0.0.1:5000/address?cep={0}", "05725060"));
        //UnityWebRequest www = UnityWebRequest.Get(String.Format("http://viacep.com.br/ws/{0}/json", cep));
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            ApiResponse info = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
            ImageClassificationScript imageClassification = imageClassGameObject.GetComponent<ImageClassificationScript>();
            imageClassification.apiResponse = info;

        }
    }

    private ApiResponse GetApiData()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://viacep.com.br/ws/{0}/json", "05725060"));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        ApiResponse info = JsonUtility.FromJson<ApiResponse>(jsonResponse);
        return info;
    }*/

}
