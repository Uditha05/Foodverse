using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodCardPrefabCtrl : MonoBehaviour
{
    public string foodName;
    public string foodPrice;
    public string foodID;

    FoodItem foodItem;

    [Header("UI components")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PriceText;

    public void setValues(FoodItem foodItem)
    {
        this.foodName = foodItem.name;
        this.foodPrice = foodItem.price.ToString();
        this.foodID = foodItem.id;
        this.foodItem = foodItem;

        UpdateUI();
    }

    private void UpdateUI()
    {
        NameText.text = this.foodName;
        PriceText.text = this.foodPrice;
    }

    public void ClickFoodCard()
    {
        GameObject shopManager = GameObject.Find("Shop Dashboard Manager");
        ShopDashboardM shopDashboardM = shopManager.GetComponent<ShopDashboardM>();
        shopDashboardM.ClickFoodCard(this.foodItem);
    }
}
