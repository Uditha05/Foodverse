using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


using Firebase.Storage;

public class ImageLoadFromGallery : MonoBehaviour
{
    public Image LocalProfileImage;

    bool isLoading;

    public GameObject loadingPanel;

    public GameObject UpdateShopName;
    public GameObject UpdateShopCity;

    public string imagePath;
    string oldURL;
    bool isSelectFile = false;

    public Texture2D temptexture;

    public Texture2D deftexture;

    public TextMeshProUGUI err;


    private void OnEnable()
    {
        this.isLoading = true;
        this.isSelectFile = false;
        //set online image, shop id
        Restaurant restaurant = GameObject.Find("Shop Dashboard Manager").GetComponent<ShopDashboardM>().getRestaurent();
        this.oldURL= restaurant.profileImage;
        UpdateShopName.GetComponent<TMP_InputField>().text = restaurant.name;
        UpdateShopCity.GetComponent<TMP_InputField>().text = restaurant.city;
        StartCoroutine(RetrieveLogoTextureFromWeb(this.oldURL));
    }

    private void Update()
    {
        if (this.isLoading)
        {
            loadingPanel.SetActive(true);
        }
        else
        {
            if (loadingPanel.active)
            {
                loadingPanel.SetActive(false);
            }
        }
    }

    public void ShowMediaPicker()
    {
        Debug.Log("Start pick");
        if (Application.isEditor)
        {
            // Do something else, since the plugin does not work inside the editor
            temptexture = deftexture;
        }
        else
        {            
            NativeGallery.GetImageFromGallery((path) =>
            {
                //UploadNewProfileImage(path);
                this.imagePath = path;
                this.isSelectFile = true;
                Debug.Log($"path1: {this.imagePath}");
                err.text = $"path: {this.imagePath}";
                Texture2D texture = NativeGallery.LoadImageAtPath(path);           
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                LocalProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);               

            });
        }
    }

    IEnumerator RetrieveLogoTextureFromWeb(string url)
    {
        //Debug.Log("Start Load Logo image: URL: " + url);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
                isLoading = false;
            }
            else
            {
                // Get downloaded texture once the web request completes
                var texture = DownloadHandlerTexture.GetContent(uwr);
                temptexture = texture;
                LocalProfileImage.sprite = ConvertToSprite(texture);
                //Debug.Log("Logo Image downloaded " + uwr);
                isLoading = false;

            }
        }
    }

    public Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    private IEnumerator UploadProfile()
    {
        Debug.Log($"Test log 1"+ this.imagePath);
        err.text = $"Path 01 {imagePath}";
        //Debug.Log($"Path: {this.imagePath}");

        Restaurant restaurant = GameObject.Find("Shop Dashboard Manager").GetComponent<ShopDashboardM>().getRestaurent();
        var storage = FirebaseStorage.DefaultInstance;
        var profileImageRef = storage.GetReference($"/Profile_Image/{restaurant.id}/profile.jpg");
        string local_file = this.imagePath;
        
        //var bytes = NativeGallery.LoadImageAtPath(path).EncodeToJPG();
        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "image/jpeg";
        var uploadTask = profileImageRef.PutFileAsync(local_file, newMetadata);
        Debug.Log($"Test log 2");
        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if(uploadTask.Exception != null)
        {
            Debug.Log($"Upload error {uploadTask.Exception}");
            err.text = $"Upload error {uploadTask.Exception}";
            yield break;
        }

        var getUrlTask = profileImageRef.GetDownloadUrlAsync();
        yield return new WaitUntil(() => getUrlTask.IsCompleted);

        if (getUrlTask.Exception != null)
        {
            Debug.Log("download error");
            err.text = $"download error {getUrlTask.Exception}";
            yield break;
        }

        Debug.Log("URL: " + getUrlTask.Result);
        SendToDashboardM(getUrlTask.Result.ToString());
    }

    

    public void UpdateData()
    {
        this.isLoading = true;
        Debug.Log("Start update: " + this.oldURL);
        
        if (!this.isSelectFile)
        {
            SendToDashboardM(this.oldURL);
        }
        else
        {
            StartCoroutine(UploadProfile());
        }
        

    }

    void SendToDashboardM(string newUrl)
    {
        string newName = UpdateShopName.GetComponent<TMP_InputField>().text;
        string newCity = UpdateShopCity.GetComponent<TMP_InputField>().text;
        GameObject.Find("Shop Dashboard Manager").GetComponent<ShopDashboardM>().UpdateProfile(newUrl, newName, newCity);
    }


    //void uploadImage()
    //{

    //    var storage = FirebaseStorage.DefaultInstance;
    //    var profileImageRef = storage.GetReference("/Profile/newImage.jpg");

    //    // Create a root reference
    //    StorageReference storageRef = storage.RootReference;

    //    // Create a reference to "mountains.jpg"
    //    StorageReference mountainsRef = storageRef.Child("mountains.jpg");

    //    // Create a reference to 'images/mountains.jpg'
    //    StorageReference mountainImagesRef =
    //        storageRef.Child("images/mountains.jpg");

    //    // File located on disk
    //    string localFile = "...";

    //    // Create a reference to the file you want to upload
    //    StorageReference riversRef = storageRef.Child("images/rivers.jpg");

    //    // Upload the file to the path "images/rivers.jpg"
    //    riversRef.PutFileAsync(localFile)
    //        .ContinueWith((Task<StorageMetadata> task) => {
    //            if (task.IsFaulted || task.IsCanceled)
    //            {
    //                Debug.Log(task.Exception.ToString());
    //        // Uh-oh, an error occurred!
    //    }
    //            else
    //            {
    //        // Metadata contains file metadata such as size, content-type, and download URL.
    //        StorageMetadata metadata = task.Result;
    //                string md5Hash = metadata.Md5Hash;
    //                Debug.Log("Finished uploading...");
    //                Debug.Log("md5 hash = " + md5Hash);
    //            }
    //        });

    //    // While the file names are the same, the references point to different files
    //    Assert.AreEqual(mountainsRef.Name, mountainImagesRef.Name);
    //    Assert.AreNotEqual(mountainsRef.Path, mountainImagesRef.Path);

    //    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
    //    //Editing Metadata
    //    var newMetadata = new MetadataChange();
    //    newMetadata.ContentType = "image/jpeg";

    //    //Create a reference to where the file needs to be uploaded
    //    StorageReference uploadRef = storageReference.Child("uploads/newFile.jpeg");
    //    Debug.Log("File upload started");
    //    uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => {
    //        if (task.IsFaulted || task.IsCanceled)
    //        {
    //            Debug.Log(task.Exception.ToString());
    //        }
    //        else
    //        {
    //            Debug.Log("File Uploaded Successfully!");
    //        }
    //    });
    //}
}
