
using TMPro;
using UnityEngine;

public class MenuCharacterController : MonoBehaviour
{
    [SerializeField] private Transform _characterVisualizationContainer;
    [SerializeField] private TextMeshProUGUI _characterName;
    
    private GameObject _characterVisualization;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (_characterVisualization != null)
        {
            Destroy(_characterVisualization);
        }

        PlayerCharacter character = ProgressManager.Instance.GetCurrentCharacter();
        _characterVisualization = Instantiate(character.CharacterVisualPrefab,
            _characterVisualizationContainer);
        _characterName.text = character.CharacterName;
    }
}
