using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCharacter : Character
{
    [SerializeField] private Texture2D _characterIcon;
    public Texture2D CharacterIcon => _characterIcon;
    
    [SerializeField] private string _characterName;
    public string CharacterName => _characterName;

    [SerializeField] private string _characterDescription;
    public string CharacterDescription => _characterDescription;
    
    [SerializeField] private  string _superDescription;
    public string SuperDescription => _superDescription;
}
