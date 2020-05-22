using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class SceneGameplayWindow : EditorWindow
{
    private GameManager gameManager;
    private CustomersManager customersManager;
    private FoodSpawner foodSpawner;

    private int tablesCount;
    private int spawnPointsCount;

    private Scene currentScene;

    [MenuItem("Custom/Gameplay")]
    public static void ShowWindow()
    {
        GetWindow<SceneGameplayWindow>("Configure gameplay");
    }
    
    private void OnGUI()
    {
        #region Initialize
        if (currentScene != SceneManager.GetActiveScene())
            Initialize();
        else if (gameManager == null)
        {
            if (!FindGameManager())
                return;
        }
        #endregion /Initialize

        #region Tables
        EditorGUILayout.LabelField("Tables");
        if (tablesCount == 0)
            EditorGUILayout.HelpBox("Add tables to the scene and press button below", MessageType.Error);
        else
            EditorGUILayout.LabelField("Number of tables", tablesCount.ToString());
        
        if (GUILayout.Button("Update Tables"))
            FindTables();
        #endregion
        
        #region Food Spawner
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Food Spawner");
        if (foodSpawner == null)
            EditorGUILayout.HelpBox("Add object with food spawner component to a scene", MessageType.Error);
        else if (spawnPointsCount == 0)
            EditorGUILayout.HelpBox("Add (empty) GameObjects as children to food spawner", MessageType.Error);
        else
            EditorGUILayout.LabelField("Number of spawn points", spawnPointsCount.ToString());
        
        if (GUILayout.Button("Update spawn points"))
            FindFoodSpawnPoints(true);
        #endregion
        
        #region Links
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Links");

        if (GUILayout.Button("Go to Game Manager"))
            Selection.objects = GetObjectAsArray(gameManager.gameObject);

        if (GUILayout.Button("Go to Player"))
            Selection.objects = GetObjectAsArray(FindObjectOfType<PlayerMovementWallRun>().gameObject);

        if (foodSpawner)
        {
            if (GUILayout.Button("Go to Food spawner"))
                Selection.objects = GetObjectAsArray(foodSpawner.gameObject);
        }
        #endregion
    }

    bool FindGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            EditorGUILayout.HelpBox("Add prefab 'Essentials'!", MessageType.Error);
            return false;
        }
        customersManager = gameManager.gameObject.GetComponent<CustomersManager>();
        FindFoodSpawnPoints();
        return true;
    }

    void Initialize()
    {
        currentScene = SceneManager.GetActiveScene();
        if (FindGameManager())
        {
            tablesCount = customersManager.freeTables.Count;
        }
    }

    void FindTables()
    {
        Table[] tables = FindObjectsOfType<Table>();
        tablesCount = tables.Length;
        if (tablesCount == 0)
        {
            Debug.LogError("There are no tables in this scene!");
            return;
        }
        
        Undo.RecordObject(customersManager, "Updated tables");
        Object[] tablesObjects = new Object[tablesCount];
        for (int i = 0; i < tablesCount; i++)
        {
            tablesObjects[i] = tables[i].gameObject;
        }
        customersManager.freeTables = new List<Table>(tables);
        EditorUtility.SetDirty(customersManager);

        Selection.objects = tablesObjects;
    }

    void FindFoodSpawnPoints(bool update = false)
    {
        FoodSpawner[] spawnersTmp = FindObjectsOfType<FoodSpawner>();
        if (spawnersTmp.Length > 1)
            Debug.LogError("There are " + spawnersTmp.Length + " food spawners!");
        else if (spawnersTmp.Length == 1)
        {
            foodSpawner = spawnersTmp[0];
            spawnPointsCount = foodSpawner.spawnPoints.Count;
        }

        Undo.RecordObject(customersManager, "Updated food spawn points");
        if (foodSpawner.spawnPoints == null ||
            update && foodSpawner.transform.childCount > 0)
        {
            List<Transform> spawnPoints = new List<Transform>();
            for (int i = 0; i < foodSpawner.transform.childCount; i++)
            {
                spawnPoints.Add(foodSpawner.transform.GetChild(i));
            }
            foodSpawner.SetSpawnPoints(spawnPoints);
            spawnPointsCount = spawnPoints.Count;
        }
        EditorUtility.SetDirty(foodSpawner);
        EditorUtility.SetDirty(customersManager);
    }

    Object[] GetObjectAsArray(Object objectToArray)
    {
        Object[] ret = new Object[1];
        ret[0] = objectToArray;
        return ret;
    }

    void ShowCustomerInspector()
    {
        bool customerInspector = false;
        customerInspector = EditorGUILayout.Foldout(customerInspector, "Show CustomersManager");
        if (customerInspector)
        {
            EditorGUI.indentLevel++;
            Editor editor = Editor.CreateEditor(gameManager.gameObject.GetComponent<CustomersManager>());
            editor.DrawDefaultInspector();
            EditorGUI.indentLevel--;
        }
    }
}
