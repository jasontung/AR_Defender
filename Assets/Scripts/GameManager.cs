using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    //UI元件宣告
    public CanvasGroup menuPanel;
    public CanvasGroup gamingPanel;
    public CanvasGroup missionSuccessPanel;
    public CanvasGroup missionFailPanel;
    //遊戲中各模組宣告
    public CrystalController crystalController;
    public HeroController heroController;
    public BattleFieldController battleFieldController;
    public EnemySpawner enemySpawner;
    private bool isGameStarted;
    private void Awake()
    {
        battleFieldController.onBattleFieldReady.AddListener(BattleFieldReady);
        battleFieldController.onBattleFieldLost.AddListener(BattleFieldLost);
        BattleFieldLost();
    }

    //當已偵測到主場景的AR圖像
    public void BattleFieldReady()
    {
        menuPanel.interactable = true;
    }

    //當主場景的AR圖像遺失，暫停遊戲並開啟主選單
    public void BattleFieldLost()
    {
        Pause();
        menuPanel.interactable = false;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        menuPanel.gameObject.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        menuPanel.gameObject.SetActive(false);
        if (isGameStarted == false)
            StartCoroutine(ProcessGameFlow());
    }

    private IEnumerator ProcessGameFlow()
    {
        isGameStarted = true;
        yield return StartCoroutine(crystalController.Execute());
        GameObject resultPanel = missionSuccessPanel.gameObject;
        if (crystalController.isDead)
        {
            resultPanel = missionFailPanel.gameObject;
        }
        enemySpawner.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        resultPanel.SetActive(true);
    }

    //從新載入場景
    public void Reload()
    {
        SceneManager.LoadScene(0);
    }
}
