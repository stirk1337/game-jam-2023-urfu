using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject LostCanvas;
    [SerializeField] TextMeshProUGUI survivedText;
    Canvas[] canvases;
    WaveManager waveManager;
    [SerializeField] AudioSource lostSound;
    [SerializeField] AudioSource mainMusic;

    public void Lost()
    {
        foreach (var canv in canvases)
        {
            canv.gameObject.SetActive(false);
        }
        mainMusic.Stop();
        lostSound.Play();
        survivedText.text = "Вы прожили " + waveManager.currentWave.ToString() + " волн";
        LostCanvas.SetActive(true);

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void Start()
    {
        canvases = FindObjectsOfType<Canvas>();
        waveManager = FindObjectOfType<WaveManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
