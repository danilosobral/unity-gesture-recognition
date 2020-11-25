using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using System.Security.Cryptography;
using System.IO;

public class ImageClassificationScript : MonoBehaviour
{
    [SerializeField] RawImage cameraDisplay;
    [SerializeField] Text outputTextDisplay;
    [SerializeField] string fileName = "hand_gesture_model.tflite";

    public RawImage rawWebcamTexture;
    public List<string> framesList = new List<string>();
    
    //public string loginUrl = "http://127.0.0.1:5000/login";
    //public string username = "LOGIN";
    //public string password = "SENHA";

    public int framesToRecord = 60;
    public int sampleRate = 3; // Record one in every three frames
    public int initialFrame = 120;

    private WebCamDevice[] devices;
    private WebCamDevice chosenCamera;
    private WebCamTexture cameraTexture;

    private Interpreter interpreter;
    private float[,] inputs = new float[128, 128];
    private float[] outputs = new float[2];
    private bool isProcessing;
    private int frameCounter = 0;
    private int framesRecorded = 0;

    void Start()
    {
        StartCamera();
        outputTextDisplay.text = "Camera Inicializada!";
        StartInterpreter();
        outputTextDisplay.text = "Interpretador Inicializado!";
        //LoginToApi();
    }

    void Update()
    {
        if (!isProcessing && cameraTexture)
        {
            Invoke(cameraTexture);
        }

        if (frameCounter >= initialFrame && framesRecorded < framesToRecord)
        {
            //SaveTextureAsPNG(cameraTexture);
            AddToFrameList(cameraTexture);
        }

        frameCounter++;
    }

    void OnDestroy()
    {
        interpreter?.Dispose();
    }

    void LoginToApi()
    {
        //StartCoroutine(ApiController.Login(username, password, loginUrl));
    }

    void AddToFrameList(WebCamTexture webcamTexture)
    {
        Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);

        //Save the image to the Texture2D
        texture.SetPixels(webcamTexture.GetPixels());
        texture.Apply();

        string base64Tex = System.Convert.ToBase64String(texture.EncodeToPNG());
        //Debug.Log("Base 64:" + base64Tex);
        framesList.Add(base64Tex);
        framesRecorded++;
    }

    void SaveTextureAsPNG(WebCamTexture webcamTexture)
    {
        string dirPath = Application.dataPath + "/../SaveImages/"; //Mudar para Application.persistentDataPath?
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        dirPath += "frame" + framesRecorded;

        Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);

        texture.SetPixels(webcamTexture.GetPixels());
        texture.Apply();

        //Encode it as a PNG.
        byte[] bytes = texture.EncodeToPNG();

        //Save it in a file.
        File.WriteAllBytes(dirPath + ".png", bytes);
    }
    
    void StartCamera()
    {
        devices = WebCamTexture.devices;
        chosenCamera = devices[0];
        foreach (var device in devices)
        {
            if (device.isFrontFacing) chosenCamera = device;
        }
        cameraTexture = new WebCamTexture(chosenCamera.name, 128, 128, 60);
        cameraTexture.Play();
        cameraDisplay.texture = cameraTexture;
    }

    void StartInterpreter()
    {
        
        var options = new InterpreterOptions()
        {
            threads = 2,
            useNNAPI = false,
        };
        outputTextDisplay.text = "AQUI";
        interpreter = new Interpreter(FileUtil.LoadFile(fileName), options);
        interpreter.ResizeInputTensor(0, new int[] { 1, 128, 128, 1 });
        interpreter.AllocateTensors();
    }

    void Invoke(WebCamTexture texture)
    {
        isProcessing = true;

        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 128; j++)
            {
                int W = (int) (texture.width * ((float)j / 128));
                int H = (int) (texture.height * ((float)i / 128));
                inputs[i, j] = pixels[H * texture.width + W].grayscale;
            }
        }

        float startTime = Time.realtimeSinceStartup;
        interpreter.SetInputTensorData(0, inputs);
        interpreter.Invoke();
        interpreter.GetOutputTensorData(0, outputs);
        float duration = Time.realtimeSinceStartup - startTime;

        if (outputs[0] > 0.5)
        {
            outputTextDisplay.text = "FECHADA: " + (outputs[0] * 100).ToString() + "%";
            EventsManager.instance.OnHandMovementTrigger(gameObject.GetInstanceID(), false);
        }
        else
        {
            outputTextDisplay.text = "ABERTA: " + ((1 - outputs[0]) * 100).ToString() + "%";
            EventsManager.instance.OnHandMovementTrigger(gameObject.GetInstanceID(), true);
        }
        isProcessing = false;
    }
}
