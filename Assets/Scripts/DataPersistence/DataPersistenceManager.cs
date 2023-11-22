using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllIDataPersistenceObjects();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        dataHandler.Save(gameData);
        LoadGame();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
            return;
        }

        if (this.gameData == null)
        {
            Debug.Log("No data found to load game.");
            return; 
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            dataPersistenceObj.LoadData(gameData);

        Debug.Log("Game Loaded");
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("No data was found to save.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            dataPersistenceObj.SaveData(gameData);

        dataHandler.Save(gameData);

        Debug.Log("Game Saved, There were " + dataPersistenceObjects.Count + " data persistance objects.");
    }

    private List<IDataPersistence> FindAllIDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        List<IDataPersistence> dataPersistences = new List<IDataPersistence>(dataPersistenceObjects);

        foreach (var obj in dataPersistences)
        {
            if (obj is GameManager && !((GameManager)obj).activated)
            {
                dataPersistences.Remove(obj);
                break;
            }
        }

        return dataPersistences;
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
