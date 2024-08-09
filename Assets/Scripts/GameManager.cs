using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string username;
    public GameObject panelInput;
    public GameObject panelWelcome;

    public int m_HighScore;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadName();
        LoadHighScore();

        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    public class SaveData {
        public string username;
        public int highScore;
    }

    public void saveName(TMP_Text input){
        if (input.text != "Insert your name​"){
            username = input.text;
        } else {return;}

        SaveData data = new SaveData();
        data.username = username;
        data.highScore = m_HighScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        
        print("Name Saved");
        ChangeScene();
    }

    public void LoadName(){
        panelWelcome.SetActive(false);
        panelInput.SetActive(false);

        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path)){
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);  
            username = data.username; 
            
            if (username != ""){
                WelcomeText(username);
            } else {
                panelInput.SetActive(true);
            }
        } else {
                panelInput.SetActive(true);
            }
    }

    public void WelcomeText(string username){
        panelWelcome.SetActive(true);
        panelWelcome.GetComponentInChildren<TMP_Text>().text = $"Welcome back, {username}...";


        StartCoroutine(fadeIn());

    }

    IEnumerator fadeIn( ){
        CanvasGroup canvasGroup = panelWelcome.GetComponent<CanvasGroup>();

        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / 1);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / 1));
            yield return null;
        }

        panelInput.SetActive(true);

        //ChangeScene();
    }

    public void ChangeScene(){
        int nextScene = SceneManager.GetActiveScene().buildIndex+1;
        SceneManager.LoadScene(nextScene);
    }

    public void saveHighScore(int newHighScore){
        SaveData data = new SaveData();
        data.username = username;
        data.highScore = newHighScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        print("HS Saved");
    }

    public void LoadHighScore(){
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            m_HighScore = data.highScore;
        } else {
            print("Não foi possível carregar o HighScore.");
        }
    }

    public void ExitGame(){
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
