using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using System;
using Siccity.GLTFUtility;
using System.IO;

using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;

public class ARViewManager : MonoBehaviour
{

    FirebaseFirestore db;   //Firebase instance

    bool isLoadFoodList = true; //loading flag for initial loading
    public bool isLoadingModel = true;  //loading flag for model rendering
    public bool isDonloadComplete = true;  //donloading complete flag for model

    Restaurant restaurant;  // Restaurent - model object
    List<FoodItem> allFoodItems = new List<FoodItem>(); //Food List - model object list
    public int index = 0;   //initial index
    int attemptCount = 0;

    public Texture2D textureFile;   //Scan Image Texture2D
    public float printedTargetSize; //Scan Image target size
    public string targetName;   //Target Image object name

    public string path; //path for temporary download models

    [Header("UI components")]
    public GameObject ModelLoadingPanel;
    public GameObject InitialLoadingPanel;

    //!GameObject rOut;
    GameObject tracker;

    //public GameObject foodModel;
    //public GameObject renderObj;

    public GameObject RightButtonObj;
    public GameObject LeftButtonObj;

    private ImageTargetBehaviour mTarget;


    private void Awake()
    {
        if (GameObject.Find("Traget_Image"))
        {
            Destroy(GameObject.Find("Traget_Image"));
        }
        // Set loading flag
        this.isLoadFoodList = true;
        this.isLoadingModel = true;
        this.isDonloadComplete = true;

        this.index = 0;
        this.attemptCount = 0;

        // Create firestore instance
        this.db = FirebaseFirestore.DefaultInstance;

        // Set file path
        string filePath = $"{Application.persistentDataPath}/Files/";
        path = $"{filePath}{"hello123.gltf"}";
        if (File.Exists(path))
        {
            Debug.Log("Found file locally at start!, Delete local file!");
            File.Delete(path);
        }

        // Left button hide
        setLeftRightButton();

        // Load ShopDataHolder
        GameObject ShopDataHolderObj = GameObject.Find("ShopDataHolder");


        if (ShopDataHolderObj != null)
        {
            this.textureFile = ShopDataHolderObj.GetComponent<ShopDataHolder>().ScanImage;
            this.restaurant = ShopDataHolderObj.GetComponent<ShopDataHolder>().selectedRestaurant;
            this.loadAllFoodItem(restaurant.id);
        }
        else
        {
            Debug.LogError("ShopDataHolderObj not found");
            Debug.LogError("Scan Target not found");           
        }

        VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromSideloadedTexture;

    }

    // Load FoodItem models from databse 
    public void loadAllFoodItem(string restaurentID)
    {
        allFoodItems.Clear();
        try
        {
            this.db.Collection("Restaurant").Document(restaurentID).Collection("Menu").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                foreach (var oneShop in task.Result)
                {
                    FoodItem foodItem = oneShop.ConvertTo<FoodItem>();
                    allFoodItems.Add(foodItem);
                }

                Debug.Log("Food model loaded!");
                Debug.Log("Loaded Food model count: " + allFoodItems.Count);
                this.isLoadFoodList = false;

                invokeFoodItem();
            });
        }
        catch (Exception error)
        {
            Debug.LogError(error);
        }
    }

    //To load 3D model according to current index and category
    public void invokeFoodItem()
    {
        //!Destroy(this.rOut);
        FoodItem foodItem = allFoodItems[index];
        UpdateText();
        Debug.Log("Invoke Food 3D model: index=" + index + " | foodName=" + foodItem.name);
        if (foodItem != null)
        {
            DownloadFile(foodItem.modelURL);
        }
        else
        {
            Debug.LogError("Food data model is NULL");
        }
    }

    //Download 3D model
    public void DownloadFile(string url)
    {
        Debug.Log("Start download: " + url);
        this.isDonloadComplete = false;
        StartCoroutine(GetFileRequest(url, path, (UnityWebRequest req) =>
        {
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"{req.error} : {req.downloadHandler.text}");
                this.isDonloadComplete = true;
            }
            else
            {
                Debug.Log("Model download completed!");
                TryToChange();
                this.isLoadingModel = false;
                this.isDonloadComplete = true;
            }
        }));
    }

    // Download IEnumerator
    IEnumerator GetFileRequest(string url, string path, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(path);
            yield return req.SendWebRequest();
            callback(req);
        }
    }

    IEnumerator UserClick()
    {
        yield return new WaitUntil(()=> (this.path != null));
        Debug.Log("Download complete");
    }

    void TryToChange()
    {

        this.tracker = GameObject.Find(targetName);
        if(this.tracker == null)
        {
            Debug.Log("Cant change");
            return;
        }
        Debug.Log("Cant change"+ tracker.transform.childCount);
        if (tracker.transform.childCount > 0)
        {
            foreach (Transform child in tracker.transform)
            {
                Destroy(child.gameObject);
            }

            if (this.path != null)
            {
                Debug.Log(path);
                GameObject y = Importer.LoadFromFile(this.path);
                GameObject renderOut = Instantiate(y, tracker.transform);//this.foodModel; 
                Destroy(y);
                renderOut.name = "Target OBJ";
                renderOut.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
                //renderOut.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else
            {
                Debug.LogError("Path is Null");
            }
        }
    }


    // Change Traget image in Vuforia Engine. [Vuforia code]
    void CreateImageTargetFromSideloadedTexture()
    {
        Destroy(this.mTarget);

        this.mTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
            textureFile,
            printedTargetSize,
            targetName);
        this.mTarget.OnTargetStatusChanged += OnTargetStatusChanged;

        // add the Default Observer Event Handler to the newly created game object
        this.mTarget.gameObject.AddComponent<DefaultObserverEventHandler>();
        Debug.Log("[Vuforia] Instant Image Target created " + mTarget.TargetName);
    }

    // Observe traget detecting.And start Render 3D food model when target image have detected [Vuforia code]
    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.Log($"[Vuforia] UI target status: {behaviour.TargetName} {status.Status}");
        this.tracker = GameObject.Find(targetName);

        if (this.tracker != null)
        {
            if (status.Status.ToString() == "EXTENDED_TRACKED")
            {
                Debug.Log("[Vuforia] Image Traget not detected");
                foreach (Transform child in this.tracker.transform)
                {
                    Destroy(child.gameObject);
                }
                return;
            }

            if (this.path != null)
            {
                Debug.Log(path);
                GameObject y = Importer.LoadFromFile(this.path);
                GameObject renderOut = Instantiate(y, tracker.transform);//this.foodModel; 
                Destroy(y);
                renderOut.name = "Target OBJ";
                renderOut.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
                //renderOut.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else
            {
                Debug.LogError("Path is Null");
            }

        }

    }

    // Hide or View => Left or Right button
    void setLeftRightButton()
    {
        if (this.index == 0)
        {
            this.RightButtonObj.SetActive(true);
            this.LeftButtonObj.SetActive(false);
            return;
        }
        if ((this.index + 1) == allFoodItems.Count)
        {
            this.RightButtonObj.SetActive(false);
            this.LeftButtonObj.SetActive(true);
            return;
        }
        this.LeftButtonObj.SetActive(true);
        this.RightButtonObj.SetActive(true);
    }

    // Click Right
    public void ClickGoRight()
    {
        this.index++;
        setLeftRightButton();
        this.isLoadingModel = true;
        UpdateText();
        FoodItem foodItem = allFoodItems[index];
        Debug.Log("Invoke Food 3D model: index=" + index + " | foodName=" + foodItem.name);
        if (foodItem != null)
        {
            
            DownloadFile(foodItem.modelURL);

        }
        else
        {
            Debug.LogError("Food data model is NULL");
        }
    }

    // Click Left
    public void ClickGoLeft()
    {
        this.index--;
        setLeftRightButton();
        this.isLoadingModel = true;
        UpdateText();
        FoodItem foodItem = allFoodItems[index];
        Debug.Log("Invoke Food 3D model: index=" + index + " | foodName=" + foodItem.name);
        if (foodItem != null)
        {
           
            DownloadFile(foodItem.modelURL);
        }
        else
        {
            Debug.LogError("Food data model is NULL");
        }
    }


    private void Update()
    {
        if (isLoadFoodList)
        {

            // Inital loading screen
            InitialLoadingPanel.SetActive(true);
        }
        else if (isLoadingModel)
        {
            // Model loading screen
            InitialLoadingPanel.SetActive(false);
            ModelLoadingPanel.SetActive(true);
        }
        else
        {
            // No any loading
            InitialLoadingPanel.SetActive(false);
            ModelLoadingPanel.SetActive(false);
        }
    }

    public void UpdateText()
    {
        FoodItem foodItem = allFoodItems[index];
        ARViewController aRViewController = this.gameObject.GetComponent<ARViewController>();
        aRViewController.updateText(foodItem.name, foodItem.price.ToString());
    }









}
