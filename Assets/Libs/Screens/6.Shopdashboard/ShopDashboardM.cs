using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class ShopDashboardM : MonoBehaviour
{
    FirebaseFirestore db;
    bool isLoad = true;

    public string restID;
    Restaurant restaurant;
    FoodItem selectedFood;

    [Header("UI Parts")]
    public TextMeshProUGUI NameText;
    public GameObject UpdateFoodName;
    public GameObject UpdateFoodPrice;
    public GameObject FoodCardPrefab;
    public List<GameObject> FoodCardList = new List<GameObject>();



    [Header("Panels")]
    public GameObject foodUpdatePanel;
    public GameObject profileUpdatePanel;
    public GameObject FoodCardPanel;
    public GameObject loadingPanel;


    private void Awake()
    {
        foodUpdatePanel.SetActive(false);
        this.restID = GameObject.Find("RestOwner").GetComponent<RestOwner>().restID;
        this.isLoad = true;
        this.db = FirebaseFirestore.DefaultInstance;
        GetFoodAll();
        this.GetRestaurentByID(this.restID);
    }

    private void GetFoodAll()
    {
        List<FoodItem> allFoods = new List<FoodItem>();
        try
        {
            this.db.Collection("Restaurant").Document(this.restID).Collection("Menu").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                foreach (var oneFood in task.Result)
                {
                    FoodItem foodItem = oneFood.ConvertTo<FoodItem>();
                    allFoods.Add(foodItem);
                }
                creatFoodCards(allFoods);
                
            });
        }
        catch (Exception error)
        {
            Debug.LogError(error);
        }
    }

    void creatFoodCards(List<FoodItem> allFoods)
    {
        FoodCardList.Clear();
        foreach (FoodItem foodItem in allFoods)
        {
            GameObject foodCard = Instantiate(FoodCardPrefab, FoodCardPanel.transform);
            FoodCardPrefabCtrl shopCardPrefabCtrl = foodCard.GetComponent<FoodCardPrefabCtrl>();
            shopCardPrefabCtrl.setValues(foodItem);
            FoodCardList.Add(foodCard);
        }
    }

    private void Update()
    {
        if(isLoad)
        {
            loadingPanel.SetActive(true);
        }
        else
        {
            loadingPanel.SetActive(false);
        }
    }

    private void GetRestaurentByID(string shopID)
    {
        try
        {
            this.db.Collection("Restaurant").WhereEqualTo("id", shopID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if(task.Result.Count == 0)
                {
                    Debug.LogError("No Shop found for: "+ shopID);
                }
                Restaurant loadedRestaurent = task.Result[0].ConvertTo<Restaurant>();
                this.restaurant = loadedRestaurent;
                updateUIParts();
                this.isLoad = false;
            });
        } 
        catch(Exception error)
        {
            Debug.LogError(error);
        }

       

    }

    public void updateUIParts()
    {
        NameText.text = this.restaurant.name;
    }

    public Restaurant getRestaurent()
    {
        return this.restaurant;
    }

    public void UpdateShop()
    {
        try
        {
            this.isLoad = true;
            DocumentReference shopRef = this.db.Collection("Restaurant").Document(this.restaurant.id);
            shopRef.SetAsync(this.restaurant).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Updated Counter");
                this.isLoad = false;
            });
        }
        catch (Exception error)
        {
            Debug.LogError(error);
        }

    }

    public void ClickFoodCard(FoodItem foodItem)
    {
        this.selectedFood = foodItem;
        foodUpdatePanel.SetActive(true);
        UpdateFoodName.GetComponent<TMP_InputField>().text = foodItem.name;
        UpdateFoodPrice.GetComponent<TMP_InputField>().text = foodItem.price.ToString();
    }

    public void ClickUpdateFood()
    {
        try
        {
            this.selectedFood.name = UpdateFoodName.GetComponent<TMP_InputField>().text;
            string newPriceStr = UpdateFoodPrice.GetComponent<TMP_InputField>().text;
            this.selectedFood.price = Int16.Parse(newPriceStr);
            this.isLoad = true;
            DocumentReference foodRef = this.db.Collection("Restaurant").Document(this.restaurant.id).Collection("Menu").Document(this.selectedFood.id);
            foodRef.SetAsync(this.selectedFood).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Updated Counter");
                RefreshPanel();
                this.isLoad = false;
                CloseUpdatePanel();
            });
        }
        catch (Exception error)
        {
            Debug.LogError(error);
        }

    }

    public void CloseUpdatePanel()
    {
        foodUpdatePanel.SetActive(false);
    }

    void RefreshPanel()
    {
        foreach (Transform child in FoodCardPanel.transform)
        {
            Destroy(child.gameObject);
        }
        GetFoodAll();
    }

    public void OpenProfileUpdatePanel()
    {
        profileUpdatePanel.SetActive(true);
    }

    public void CloseProfileUpdatePanel()
    {
        profileUpdatePanel.SetActive(false);
    }

    public void UpdateProfile(string newURL, string newName, string newCity)
    {
        this.restaurant.profileImage = newURL;
        this.restaurant.name = newName;
        this.restaurant.city = newCity;
        UpdateShop();
        updateUIParts();
        CloseProfileUpdatePanel();
    }

    public void Logout()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();
        SceneManager.LoadScene("Onboard");
    }

}
