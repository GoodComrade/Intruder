using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : SceneController
{
    [SerializeField] private MenuCharacterController _menuCharacterController;
    [SerializeField] private MenuMatchingPanelController _matchingPanelController;
    public static MenuController GetMenuInstance()
    {
        return (MenuController) _instance;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    public async void StartGame()
    {
        _matchingPanelController.Show(MenuMatchingPanelController.Stage.STARTING_ROOM);
        await MultiplayerConnectionManager.Instance.StartGame();
        await Task.Delay(500);
        _matchingPanelController.Show(MenuMatchingPanelController.Stage.WAITING_FOR_PLAYER);
        while (MultiplayerConnectionManager.Instance.GetPlayerCount() < 2)
        {
            await Task.Delay(500);
        }
        _matchingPanelController.Show(MenuMatchingPanelController.Stage.STARTING);
        await Task.Delay(500);
        MultiplayerConnectionManager.Instance.SetScene(1);
    }
    
    public async void StartSoloGame()
    {
        _matchingPanelController.Show(MenuMatchingPanelController.Stage.STARTING_ROOM);
        await MultiplayerConnectionManager.Instance.StartSolo();
        _matchingPanelController.Show(MenuMatchingPanelController.Stage.STARTING);
        MultiplayerConnectionManager.Instance.SetScene(1);
    }

    public void ChangeTeam(bool monster)
    {
        ProgressManager.Instance.SetCurrentTeam(monster);
        _menuCharacterController.Refresh();
    }

    public void ChangeTeam()
    {
        ChangeTeam(!ProgressManager.Instance.GetCurrentTeamIsMonster());
    }
}
