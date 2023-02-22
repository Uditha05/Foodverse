using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Storage;
using Siccity.GLTFUtility;
using System.IO;


public class FirebaseStorageDownload : MonoBehaviour
{
    public GameObject wrapper;
    string filePath;

    FirebaseStorage storage;
    StorageReference storageReference;

    void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/";
        wrapper = new GameObject
        {
            name = "Model"
        };

        DownloadFile("https://firebasestorage.googleapis.com/v0/b/test-unity-connect.appspot.com/o/3D_Models%2FKFC%2FDoughNut_FBX.gltf?alt=media&token=059cbac5-919c-47e6-92b3-c0af33c5381a");


        ///
        /// Uncomment Following if need firbase storage with auth
        ///
        ////initialize storage reference
        //storage = FirebaseStorage.DefaultInstance;
        //storageReference = storage.GetReferenceFromUrl("gs://test-unity-connect.appspot.com/3D_Models/");
        ////get reference of image
        //StorageReference image = storageReference.Child("KFC/apples.gltf");
        ////Get the download link of file
        //image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        //{
        //    if (!task.IsFaulted && !task.IsCanceled)
        //    {
        //        Debug.Log(task.Result.ToString());
        //        //StartCoroutine(LoadImage(Convert.ToString(task.Result))); //Fetch file from the link
        //    }
        //    else
        //    {
        //        Debug.Log(task.Exception);
        //    }
        //});
    }

    //IEnumerator LoadImage(string MediaUrl)
    //{
    //    UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
    //    yield return request.SendWebRequest(); //Wait for the request to complete
    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(request.error);
    //    }
    //    else
    //    {
    //        //rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //        // setting the loaded image to our object
    //        ResetWrapper();
    //        GameObject model = Importer.LoadFromFile(filePath);
    //        model.transform.SetParent(wrapper.transform);
    //    }
    //}

    public void DownloadFile(string url)
    {
        string path = GetFilePath(url);
        Debug.Log(path);
        //if (File.Exists(path))
        //{
        //    Debug.Log("Found file locally, loading...");
        //    LoadModel(path);
        //    return;
        //}

        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        {
            if (req.result != UnityWebRequest.Result.Success)
            {
                // Log any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                // Save the model into a new wrapper
                LoadModel(path);
            }
        }));
    }

    string GetFilePath(string url)
    {
        //string[] pieces = url.Split('/');
        //string filename = pieces[pieces.Length - 1];

        return $"{filePath}helo.gltf";
    }

    void LoadModel(string path)
    {
        ResetWrapper();
        GameObject model = Importer.LoadFromFile(path);
        model.transform.SetParent(wrapper.transform);
    }

    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
            yield return req.SendWebRequest();
            callback(req);
        }
    }

    void ResetWrapper()
    {
        if (wrapper != null)
        {
            foreach (Transform trans in wrapper.transform)
            {
                Destroy(trans.gameObject);
            }
        }
    }

}
