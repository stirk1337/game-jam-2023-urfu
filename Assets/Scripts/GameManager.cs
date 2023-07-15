using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    [SerializeField] GameObject learnToPlayCanvas;
    Localisation localisation;

    [DllImport("__Internal")]
    private static extern void ShowAdv();

    public void Lost()
    {
        foreach (var canv in canvases)
        {
            canv.gameObject.SetActive(false);
        }
        mainMusic.Stop();
        lostSound.Play();
        survivedText.text = localisation.GetTranslate("Вы прожили волн: ") + waveManager.currentWave.ToString();
        LostCanvas.SetActive(true);

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
        #if UNITY_EDITOR
            //Debug.Log("Unity Editor");
        #else
            ShowAdv();
        #endif

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void Start()
    {
        canvases = FindObjectsOfType<Canvas>();
        waveManager = FindObjectOfType<WaveManager>();
        localisation = FindObjectOfType<Localisation>();
        

    }

    public void LearnToPlayCanvas()
    {
        learnToPlayCanvas.SetActive(!learnToPlayCanvas.active);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
