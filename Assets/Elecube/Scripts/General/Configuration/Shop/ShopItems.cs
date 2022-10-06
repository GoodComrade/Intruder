using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ShopItems : ScriptableObject
{
    private static ShopItems _instance;

    [SerializeField] private List<ShopItem> _items;

    public List<ShopItem> GetItems()
    {
        return _items;
    }
    
    public static ShopItems GetInstance()
    {
        if (_instance == null)
        {
            var operation = Addressables.LoadAssetAsync<ShopItems>(
                "Assets/Elecube/Configuration/Shop/ShopItems.asset");
            _instance = operation.WaitForCompletion();
        }
        return _instance;
    }
}
