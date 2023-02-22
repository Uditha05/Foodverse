using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using System;
using Siccity.GLTFUtility;
using System.IO;

using Firebase.Extensions;
using Firebase.Firestore;


public class SideLoadImageTarget : MonoBehaviour
{

    FirebaseFirestore db;       //Firebase instance

    bool isLoadFoodList = true;         //loading flag for initial loading
    public bool isLoadingModel = true;       //loading flag for model rendering

    Restaurant restaurant;      // Restaurent - model object
    public string selectedCategory;     // Selected category
    List<FoodItem> allFoodItems = new List<FoodItem>();     //Food List - model object list
    public int index = 0;       //initial index
    int attemptCount = 0;

    public Texture2D textureFile;       //Scan Image Texture2D
    public float printedTargetSize;     //Scan Image target size
    public string targetName;           //Target Image object name
    
    public string path;                 //path for temporary download models

    [Header("UI components")]
    public GameObject ModelLoadingPanel;
    public GameObject InitialLoadingPanel;

    GameObject rOut;
    GameObject tracker;

    public GameObject foodModel;
    public GameObject renderObj;

    public GameObject RightButtonObj;
    public GameObject LeftButtonObj;


    //public GameObject ROBJ;
    //public List<GameObject> renderObjList = new List<GameObject>();

    private ImageTargetBehaviour mTarget;    


    private void Awake()
    {

        // Set loading flag
        this.isLoadFoodList = true;
        this.isLoadingModel = true;

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

         // Create dummy object
         //ROBJ = new GameObject();

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
            //TODO: Redirect to home            
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
                this.isLoadFoodList= false;

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
        Destroy(this.rOut);
        FoodItem foodItem = allFoodItems[index];// this.gameObject.GetComponent<ARViewController>().Get3DmodelByIndex(index);
        UpdateText();
        Debug.Log("Invoke Food 3D model: index=" + index + " | foodName=" + foodItem.name);
        if (foodItem != null)
        {
            DownloadFile(foodItem.modelURL);
        }
        else
        {
            Debug.LogError("Food data model is NULL");
            //TODO: Show Error
        }
    }

    //Download 3D model
    public void DownloadFile(string url)
    {
        Debug.Log("Start download: " + url);
        StartCoroutine(GetFileRequest(url, path, (UnityWebRequest req) =>
        {
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                Debug.Log("Model download completed!");
                LoadModel(path, url);
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

    // Load GLTF file to frontend
    void LoadModel(string path, string url)
    {
        
        //!this.renderObj.name = "Kamalll123";
        if (!File.Exists(path))
        {
            Debug.Log("Can not find GLTF file local path.");
            this.attemptCount++;
            if(this.attemptCount < 3)
            {
                DownloadFile(url);
                return;
            }
            else
            {
                Debug.Log("Download fail 3 times :/");
                //TODO: add error window
                return;
            }            
        }
        this.attemptCount = 0;
        //!this.renderObj.SetActive(true);
        //!this.renderObj = Importer.LoadFromFile(path);
        //this.renderObj.transform.position = new Vector3(x, y, z);
        //!this.renderObj.SetActive(false);
        Debug.Log("Render Prefab status is active? : " + (this.renderObj.active));
        Debug.Log("Render Prefab status is null? : " + (this.renderObj == null));
 
        isLoadingModel = false;
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
                foreach (Transform child in tracker.transform)
                {
                    Destroy(child.gameObject);
                }
                return;

            }

            //!this.renderObj.SetActive(true);
            if (this.path != null)//this.renderObj; 
            {
                
                this.foodModel = Importer.LoadFromFile(path); //this.renderObj;                
                this.rOut = Instantiate(Importer.LoadFromFile(path), tracker.transform);//this.foodModel; 
                this.rOut.name = "Target OBj";
                rOut.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                //rOut.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
                //!this.renderObj.SetActive(false);

            }
            else
            {
                Debug.LogError("RenderObj is Null");
                //!this.renderObj.SetActive(false);
            }
            //!this.renderObj.SetActive(false);
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
        Destroy(this.rOut);
        FoodItem foodItem = allFoodItems[index];// this.gameObject.GetComponent<ARViewController>().Get3DmodelByIndex(index);
        Debug.Log("Invoke Food 3D model: index=" + index + " | foodName=" + foodItem.name);
        if (foodItem != null)
        {
            DownloadFile(foodItem.modelURL);
        }
        else
        {
            Debug.LogError("Food data model is NULL");
            //TODO: Show Error
        }
    }

    // Click Left
    public void ClickGoLeft()
    {
        this.index--;
        setLeftRightButton();
        this.isLoadingModel = true;
        //Destroy(this.rOut);
        //FoodItem foodItem = allFoodItems[index];// this.gameObject.GetComponent<ARViewController>().Get3DmodelByIndex(index);
        //DownloadFile(foodItem.modelURL);
        UpdateText();
        Destroy(this.rOut);
        FoodItem foodItem = allFoodItems[index];// this.gameObject.GetComponent<ARViewController>().Get3DmodelByIndex(index);
        Debug.Log("Invoke Food 3D model: index=" + index + " | foodName=" + foodItem.name);
        if (foodItem != null)
        {
           // DownloadFile(foodItem.modelURL);
        }
        else
        {
            Debug.LogError("Food data model is NULL");
            //TODO: Show Error
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
