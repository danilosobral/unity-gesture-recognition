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
    [SerializeField] public ApiResponse apiResponse;

    public RawImage rawWebcamTexture;
    public List<string> framesList = new List<string>();

    private WebCamDevice[] devices;
    private WebCamDevice chosenCamera;
    private WebCamTexture cameraTexture;

    private Interpreter interpreter;
    private float[,] inputs = new float[128, 128];
    private float[] outputs = new float[2];
    private bool isProcessing;
    private int frameCounter = 0;

    void Start()
    {
        StartCamera();
        outputTextDisplay.text = "Camera Inicializada!";
        StartInterpreter();
        outputTextDisplay.text = "Interpretador Inicializado!";
    }

    void Update()
    {
        if (!isProcessing && cameraTexture)
        {
            Invoke(cameraTexture);
        }

        if (frameCounter >= 100 && frameCounter <= 124)
        {

            string dirPath = Application.dataPath + "/../SaveImages/"; //Mudar para Application.persistentDataPath?
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            SaveTextureAsPNG(cameraTexture, dirPath + "frame" + (frameCounter - 100));
        }

        if (frameCounter == 150)
        {
            StartCoroutine(ApiController.UploadImages(framesList));
            createVideo();
        }

        frameCounter++;
    }

    void OnDestroy()
    {
        interpreter?.Dispose();
    }

    void SaveTextureAsPNG(WebCamTexture webcamTexture, string dirPath)
    {
        Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);

        //Save the image to the Texture2D
        texture.SetPixels(webcamTexture.GetPixels());
        texture.Apply();

        //Encode it as a PNG.
        byte[] bytes = texture.EncodeToPNG();

        //Save it in a file.
        File.WriteAllBytes(dirPath + ".png", bytes);

        string base64Tex = System.Convert.ToBase64String(texture.EncodeToPNG());
        //Debug.Log("Base 64:" + base64Tex);
        framesList.Add(base64Tex);
        Debug.Log("Size: " + framesList.Count);
    }

    void createVideo()
    {
        string output = "video.mp4";

        int framerate = 12;
        string inputRegex = "../SaveImages/frame%d.png";

        //int rc = FFmpegWrapper.Execute(string.Format("-f {0} -i {1} {2}", inputTest, inputRegex, output));
        int rc = FFmpegWrapper.Execute(string.Format("-framerate {0} -i {1} {2}", framerate, inputRegex, output));
        Debug.Log("Return Code is " + rc);
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
            outputTextDisplay.text = "FECHADA: " + apiResponse.logradouro;//+ (outputs[0] * 100).ToString() + "%";
            EventsManager.instance.OnOpenHandTrigger(gameObject.GetInstanceID(), false);
        }
        else
        {
            outputTextDisplay.text = "ABERTA: " + ((1 - outputs[0]) * 100).ToString() + "%";
            EventsManager.instance.OnOpenHandTrigger(gameObject.GetInstanceID(), true);
        }
        isProcessing = false;
    }
}
