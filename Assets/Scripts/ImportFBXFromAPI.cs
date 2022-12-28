using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ImportFBXFromAPI : MonoBehaviour
{
    public string apiBase;
    public string modelName; 

    public Vector3 scaleModel; 
    public Vector3 positionModel;
    public Quaternion rotationModel;

    private string apiURL;
    private string resourcePath;

    void Start()
    {
        LoadModelByQR(modelName);
    }

    void Update()
    {
        
    }

    public void LoadModelByQR(string QRCode)
    {
        apiURL = apiBase + "/" + QRCode;
        StartCoroutine(ImportFBXModel());
    }

    IEnumerator ImportFBXModel()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiURL);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Error downloading FBX model: " + request.error);
            yield break;
        }



        // Get the binary data of the FBX model from the response
        byte[] modelData = request.downloadHandler.data;

        // Save the FBX model data to the resource folder
        resourcePath = "Assets/Resources/" + modelName;
        File.WriteAllBytes(resourcePath, modelData);

        // Import the FBX model into the project
        AssetDatabase.ImportAsset(resourcePath);

        // Load the FBX model from the resource folder using Resources.Load
        GameObject fbxModel = Resources.Load<GameObject>(modelName.Replace(".fbx",""));

        // Instantiate the FBX model at a specific position
        GameObject modelInstance = Instantiate(fbxModel, new Vector3(0, 0, 0), Quaternion.identity);

        // Scale, position and rotation
        modelInstance.transform.localScale = scaleModel;
        modelInstance.transform.position = positionModel;
        modelInstance.transform.rotation = rotationModel;

    }

    void OnApplicationQuit()
    {
        File.Delete(resourcePath);
    }
}
