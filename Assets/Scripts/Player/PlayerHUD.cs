﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public HealthBar playerHealthBar;
    public GameObject score;
    public Text scoreCounter;
    public Text waveCounter;
    public Text enemyCounter;
    public Text ammoText;
    public Text gameOverHighscoreText;
    public Text gameOverScoreText;
    public GameObject gameOverScreen;
    public GameObject reticle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthBar.SetCurrentHealthRatio(GameController.Instance.playerHealth.GetCurrentHealthRatio());
        waveCounter.text = "Wave " + GameController.Instance.wave;
        scoreCounter.text = GameController.Instance.score.ToString();
        enemyCounter.text = GameController.Instance.GetRemainingEnemies() + " Left";
    }

    public void ShowGameOverText()
    {
        gameOverScreen.SetActive(true);
        gameOverHighscoreText.text = GameController.Instance.highscore.ToString();
        gameOverScoreText.text = GameController.Instance.score.ToString();

        score.SetActive(false);
        playerHealthBar.gameObject.SetActive(false);
        waveCounter.gameObject.SetActive(false);
        enemyCounter.gameObject.SetActive(false);
        ammoText.gameObject.SetActive(false);
        reticle.SetActive(false);
    }

    public void ShowHUD()
    {
        gameOverScreen.SetActive(false);
        playerHealthBar.gameObject.SetActive(true);
        waveCounter.gameObject.SetActive(true);
        scoreCounter.gameObject.SetActive(true);
        ammoText.gameObject.SetActive(true);
    }
}
