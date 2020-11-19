using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.IO;
using UnityEngine.Networking;

public class ApiController : MonoBehaviour
{

    public string cep;
    public GameObject imageClassGameObject;
    // Start is called before the first frame update
    void Start()
    {
        CheckApiData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static IEnumerator UploadImages(List<string> frames)
    {
        /*string[] framesArray = frames.ToArray();
        UploadImageRequest requestBody = new UploadImageRequest();
        requestBody.frames = framesArray;
        string requestJson = JsonUtility.ToJson(requestBody);
        Debug.Log(requestJson);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(requestJson);

        UnityWebRequest www = UnityWebRequest.Post("url", jsonToSend));

        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");*/

        

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        for(int i = 0; i < frames.Count; i++)
        {
            formData.Add(new MultipartFormDataSection("frame" + i, frames[i]));
        }

        Debug.Log("form data");
        Debug.Log(formData.Count);
        UnityWebRequest www = UnityWebRequest.Post("URL", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }

        /*formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post("http://www.my-server.com/myform", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }*/
    }

    public void CheckApiData()
    {
        //ImageClassificationScript imageClassification = imageClassGameObject.GetComponent<ImageClassificationScript>();
        //imageClassification.apiResponse = GetApiData();
        StartCoroutine(GetApiDataUnity());
    }

    IEnumerator GetApiDataUnity()
    {
        UnityWebRequest www = UnityWebRequest.Get(String.Format("http://viacep.com.br/ws/{0}/json", cep));
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
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://viacep.com.br/ws/{0}/json", cep));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        ApiResponse info = JsonUtility.FromJson<ApiResponse>(jsonResponse);
        return info;
    }

}
