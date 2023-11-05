using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DataBasePanelScript : MonoBehaviour
{
    public static DataBasePanelScript instance;

    [SerializeField] private DatabaseServiceScript dbSerScript;
    [SerializeField] private TableAddPanelScript tabAddPanelScript;
    [SerializeField] private TableViewPanelScript tabViewPanelScript;
    public GameObject tableAddPanel;
    public GameObject tableViewPanel;

    [SerializeField] TextMeshProUGUI dbNameText;
    public TextAsset scriptFile; // Reference to the script file

    [SerializeField] private TMP_Dropdown newTableDropDown;
    [SerializeField] private TMP_Dropdown existTableDropDown;

    public TextMeshProUGUI tableInfoText;
    [SerializeField] private TextMeshProUGUI errorLog;

    private string selectedDB;
    private string dbPath;
    private SQLiteConnection connection;

    [SerializeField] private string newTable = "Select";
    [SerializeField] private string existTable = "Select";
    
    private void Awake()
    {
        if (instance == null)
            instance = this;

        dbSerScript = GetComponent<DatabaseServiceScript>();
        tabAddPanelScript = GetComponent<TableAddPanelScript>();
        tabViewPanelScript = GetComponent<TableViewPanelScript>();
    }

    private void Start()
    {
        // Add a listener to the onValueChanged event of the TMP_Dropdown
        newTableDropDown.onValueChanged.AddListener(OnDropdown1ValueChanged);
        existTableDropDown.onValueChanged.AddListener(OnDropdown2ValueChanged);
    }

    public void OnDbSelected(string name)
    {
        // Set Db Name
        selectedDB = name + ".db";
        // Set the path to your database file
        dbPath = Application.streamingAssetsPath + "/" + selectedDB;

        dbNameText.text = name;

        newTable = "Select";
        existTable = "Select";

        OnGetClassTableName();
        OnGettingDbTableInfo();
    }

    public void OnCreateNewTable()
    {
        // Open a connection to the database
        connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        if (newTable == "Select")
        {
            errorLog.GetComponent<TextMeshProUGUI>().text = "Invalid Selection";
            StartCoroutine(DisplayErrorMsg());
        }
        else
        {
            if (dbSerScript.TableExists(newTable, connection))
            {
                errorLog.GetComponent<TextMeshProUGUI>().text = "Table Already Exists";
                StartCoroutine(DisplayErrorMsg());
            }
            else
            {
                if (newTable == "HomePageTable")
                {
                    connection.DropTable<HomePageTable>();
                    connection.CreateTable<HomePageTable>();
                    errorLog.GetComponent<TextMeshProUGUI>().text = "New Table Created";
                    StartCoroutine(DisplayErrorMsg());
                }
                else if (newTable == "ResearchTable")
                {
                    connection.DropTable<ResearchTable>();
                    connection.CreateTable<ResearchTable>();
                    errorLog.GetComponent<TextMeshProUGUI>().text = "New Table Created";
                    StartCoroutine(DisplayErrorMsg());
                }
                else if (newTable == "BusinessTable")
                {
                    connection.DropTable<BusinessTable>();
                    connection.CreateTable<BusinessTable>();
                    errorLog.GetComponent<TextMeshProUGUI>().text = "New Table Created";
                    StartCoroutine(DisplayErrorMsg());
                }
                else if (newTable == "LifeStyleTable")
                {
                    connection.DropTable<LifeStyleTable>();
                    connection.CreateTable<LifeStyleTable>();
                    errorLog.GetComponent<TextMeshProUGUI>().text = "New Table Created";
                    StartCoroutine(DisplayErrorMsg());
                }
                else if (newTable == "TradeTable")
                {
                    connection.DropTable<TradeTable>();
                    connection.CreateTable<TradeTable>();
                    errorLog.GetComponent<TextMeshProUGUI>().text = "New Table Created";
                    StartCoroutine(DisplayErrorMsg());
                }
                else if (newTable == "GeneralTables")
                {
                    connection.DropTable<GeneralTable>();
                    connection.CreateTable<GeneralTable>();
                    errorLog.GetComponent<TextMeshProUGUI>().text = "New Table Created";
                    StartCoroutine(DisplayErrorMsg());
                }
                else
                {
                    errorLog.GetComponent<TextMeshProUGUI>().text = "Unable to Create Table";
                    StartCoroutine(DisplayErrorMsg());
                }

                OnGettingDbTableInfo();
            }
        }

        // Close the database connection
        connection.Close();
    }
    
    public void AddEntryToExistTable()
    {
        if (existTable == "Select")
        {
            errorLog.GetComponent<TextMeshProUGUI>().text = "Invalid Selection";
            StartCoroutine(DisplayErrorMsg());
        }
        else
        {
            tableAddPanel.SetActive(true);
            tabAddPanelScript.OnAddTableEntry(existTable, dbPath);
        }
    }

    private void OnGettingDbTableInfo()
    {
        // Open a connection to the database
        connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly);

        // Clear any existing options
        existTableDropDown.ClearOptions();

        // Create a list of Dropdown options
        List<TMP_Dropdown.OptionData> dropdownOptions3 = new List<TMP_Dropdown.OptionData>();
        TMP_Dropdown.OptionData dropdownOption = new TMP_Dropdown.OptionData("Select");
        dropdownOptions3.Add(dropdownOption);

        // Get the table names
        List<string> tableNames = GetAllTableNames();
        foreach (string tableName in tableNames)
        {
            //Debug.Log("Table Name: " + tableName);
            TMP_Dropdown.OptionData dropdownOpt = new TMP_Dropdown.OptionData(tableName);
            dropdownOptions3.Add(dropdownOpt);
        }

        // Add the options list to the Dropdown
        existTableDropDown.AddOptions(dropdownOptions3);

        // Display the table names and count
        tableInfoText.text = "Table Count: " + tableNames.Count;

        // Close the database connection
        connection.Close();
    }

    public List<string> GetAllTableNames()
    {
        var query = "SELECT name FROM sqlite_master WHERE type='table'";
        var tableNames = connection.Query<TableInfo>(query);
        return tableNames.ConvertAll(t => t.name);
    }


    public void OnGetClassTableName()
    {
        // Clear any existing options
        newTableDropDown.ClearOptions();

        // Create a list of Dropdown options
        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
        TMP_Dropdown.OptionData dropdownOption = new TMP_Dropdown.OptionData("Select");
        dropdownOptions.Add(dropdownOption);

        // Add each Table Name to option to the list
        if (scriptFile != null)
        {
            string scriptText = scriptFile.text;
            List<string> classNames = ExtractClassNames(scriptText);

            foreach (string className in classNames)
            {
                if (className != "AllTableScript")
                {
                    //Debug.Log(className);
                    TMP_Dropdown.OptionData dropdownOption2 = new TMP_Dropdown.OptionData(className);
                    dropdownOptions.Add(dropdownOption2);
                }
            }
            // Add the options list to the Dropdown
            newTableDropDown.AddOptions(dropdownOptions);
        }
    }

    List<string> ExtractClassNames(string scriptText)
    {
        List<string> classNames = new List<string>();
        string pattern = @"\bclass\s+(\w+)\s*";
        MatchCollection matches = Regex.Matches(scriptText, pattern);

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count >= 2)
            {
                string className = match.Groups[1].Value;
                classNames.Add(className);
            }
        }

        return classNames;
    }


    // Data model for table information
    private class TableInfo
    {
        public string name { get; set; }
    }


    // This 2 method is called when the dropdown value changes
    private void OnDropdown1ValueChanged(int index)
    {
        newTable = newTableDropDown.options[index].text; // Get the selected text
    }
    private void OnDropdown2ValueChanged(int index)
    {
        existTable = existTableDropDown.options[index].text; // Get the selected text
    }

    public void OnViewBtn()
    {
        if (existTable == "Select")
        {
            errorLog.GetComponent<TextMeshProUGUI>().text = "Invalid Selection";
            StartCoroutine(DisplayErrorMsg());
        }
        else
        {
            tableViewPanel.SetActive(true);
            tabViewPanelScript.OnTableViewPanelOpen(dbPath, existTable);
        }
    }

    private IEnumerator DisplayErrorMsg()
    {
        errorLog.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        errorLog.gameObject.SetActive(false);
    }
}
