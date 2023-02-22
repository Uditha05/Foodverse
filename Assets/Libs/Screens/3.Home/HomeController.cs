using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeController : MonoBehaviour
{
    FirebaseFirestore db;
    bool isLoad = true;

    List<Restaurant> allRestaurant = new List<Restaurant>();
    [Header("UI components")]
    public GameObject ShopCardPrefab;
    public GameObject ShopCardPanel;
    public GameObject LoadingPanel;
    public GameObject SearchBar;
    public List<GameObject> ShopCardList = new List<GameObject>();

    private void Awake()
    {
        this.isLoad = true;
        this.db = FirebaseFirestore.DefaultInstance;
        this.GetRestaurentAll();
    }

    private void Start()
    {
        var inputField = SearchBar.GetComponent<TMP_InputField>();
        inputField.text = "";
        inputField.onValueChanged.AddListener(delegate { UpdateTextBox(inputField); });
    }

    public void UpdateTextBox(TMP_InputField textbox)
    {
        if (textbox.text.Length > 0)
        {           
            Debug.Log("Value is :" + textbox.text);
            showSearchCard(textbox.text);
        }
        else
        {
            creatShopCards(allRestaurant);
           
        }
    }

    private void Update()
    {
        if (isLoad)
        {
            LoadingPanel.SetActive(true);
        }
        else
        {
            LoadingPanel.SetActive(false);
        }
    }

    private void GetRestaurentAll()
    {
        allRestaurant.Clear();
        try
        {
            this.db.Collection("Restaurant").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                foreach(var oneShop in task.Result)
                {
                    Restaurant restaurant = oneShop.ConvertTo<Restaurant>();
                    allRestaurant.Add(restaurant);
                }
                creatShopCards(allRestaurant);
                this.isLoad = false;
            });
        }
        catch (Exception error)
        {
            Debug.LogError(error);
        }
    }

    void creatShopCards(List<Restaurant> allRestaurant)
    {
        ShopCardList.Clear();

        foreach (Transform child in ShopCardPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Restaurant restaurant in allRestaurant)
        {
            GameObject shopCard = Instantiate(ShopCardPrefab, ShopCardPanel.transform);
            ShopCardPrefabCtrl shopCardPrefabCtrl = shopCard.GetComponent<ShopCardPrefabCtrl>();
            shopCardPrefabCtrl.setValues(restaurant);
            ShopCardList.Add(shopCard);
        }
    }


    void showSearchCard(string searchTerm)
    {       
        List<Restaurant> restaurants = new List<Restaurant>();
        foreach(Restaurant rest in allRestaurant)
        {
            if (rest.searchName.Contains(searchTerm))
            {
                restaurants.Add(rest);
            }
        }
        creatShopCards(restaurants);
    }

}
