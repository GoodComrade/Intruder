using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Common = 0,
    Special = 1,
    Hot = 2
};

[CreateAssetMenu(menuName = "Intruder/Shop/Item")]
public class ShopItem : ScriptableObject
{
    public Texture2D _itemPhoto;
    public String _itemName;
    public String _itemCost;
    public ItemType _itemType;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
