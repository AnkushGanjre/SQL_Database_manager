using SQLite4Unity3d;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class MainPanelScript : MonoBehaviour
{
    [SerializeField] private DatabaseServiceScript dbSerScript;
    [SerializeField] private DataBasePanelScript dbPanelScript;

    public TMP_InputField databaseNameInput;
    public TMP_InputField ExistdbNameInput;
    [SerializeField] private TextMeshProUGUI errorLog;
    [SerializeField] private GameObject databasePanel;


    private void Awake()
    {
        dbSerScript = GetComponent<DatabaseServiceScript>();
        dbPanelScript = GetComponent<DataBasePanelScript>();
    }

    public void OnCreateDatabase()
    {
        string databaseName = databaseNameInput.text;

        if (string.IsNullOrEmpty(databaseName))
        {
            errorLog.text = "Database name cannot be empty!";
            StartCoroutine(HideErrorMsg());
            return;
        }

        // Path to the persistent data path on the device
        string dbPath = Path.Combine(Application.persistentDataPath, databaseName + ".db");
        dbSerScript.DatabaseName = databaseName + ".db";

        // Create a new SQLite database
        SQLiteConnection connection = new SQLiteConnection(dbPath);
        dbSerScript.OnDatabaseService();

        // You can perform further operations on the database here, like creating tables and adding data
        // For example:
        // connection.CreateTable<MyTable>();

        connection.Close();
        dbSerScript.DatabaseName = null;
        //Debug.Log("Database created: " + dbPath);

        databasePanel.SetActive(true);
        dbPanelScript.OnDbSelected(databaseName);
    }

    public void OnDbSearchBtn()
    {
        string dbName = ExistdbNameInput.text;
        if (string.IsNullOrEmpty(dbName))
        {
            errorLog.text = "Database name cannot be empty!";
            StartCoroutine(HideErrorMsg());
            return;
        }
        

        // Path to the streaming assets folder
        string streamingAssetsPath = Application.streamingAssetsPath;

        // Get a list of all files in the streaming assets folder
        string[] filesInStreamingAssets = Directory.GetFiles(streamingAssetsPath);

        // Filter the list to get only database files
        string[] databaseFiles = filesInStreamingAssets
            .Where(filePath => Path.GetExtension(filePath).Equals(".db", System.StringComparison.OrdinalIgnoreCase))
            .ToArray();

        // Check if any database file in the streaming assets matches the input name
        bool databaseExists = databaseFiles
            .Any(databaseFile => Path.GetFileNameWithoutExtension(databaseFile).Equals(dbName, System.StringComparison.OrdinalIgnoreCase));

        if (!databaseExists)
        {
            errorLog.text = $"Database '{dbName}' does not exist in StreamingAssets.";
            StartCoroutine(HideErrorMsg());
        }
        else
        {
            dbSerScript.DatabaseName = dbName + ".db";
            dbSerScript.OnDatabaseService();
            dbSerScript.DatabaseName = null;
            databasePanel.SetActive(true);
            dbPanelScript.OnDbSelected(dbName);
        }
    }

    private IEnumerator HideErrorMsg()
    {
        errorLog.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        errorLog.gameObject.SetActive(false);
    }
}
