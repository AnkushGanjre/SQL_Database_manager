using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using SQLite4Unity3d;

public class TableAddPanelScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tableText;
    [SerializeField] GameObject entryPrefab;
    [SerializeField] GameObject successText;
    [SerializeField] Transform tabAddContent;
    [SerializeField] string dataBasePath;
    private SQLiteConnection connection;
    int rowCount;
    string tableName;

    List<string> columnNames = new List<string>();
    List<object> values = new List<object>();


    public void OnAddTableEntry(string tabName, string dbPath)
    {
        tableName = tabName;
        dataBasePath = dbPath;
        tableText.text = tableName;

        // Open a connection to the database
        connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        rowCount = GetRowCount(connection, tabName);
        //Debug.Log(rowCount);

        columnNames = GetColumnNames(connection);
        foreach (string columnName in columnNames)
        {
            GameObject go = Instantiate(entryPrefab, tabAddContent);
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = columnName;
            go.name = columnName;
        }

        StartCoroutine(SettingPrimaryKey());

        // Closing the connection to the database
        connection.Close();
    }

    private IEnumerator SettingPrimaryKey()
    {
        yield return new WaitForSeconds(0.0001f);

        tabAddContent.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().interactable = false;
        tabAddContent.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().text = (rowCount + 1).ToString();
    }

    public int GetRowCount(SQLiteConnection connection, string tableName)
    {
        // Use COUNT function to retrieve the number of rows
        string query = $"SELECT COUNT(*) FROM {tableName}";
        return connection.ExecuteScalar<int>(query);
    }

    public List<string> GetColumnNames(SQLiteConnection connection)
    {
        // Use PRAGMA table_info query to retrieve column information
        var tableInfo = connection.Query<TableInfo>($"PRAGMA table_info('{tableName}')");

        foreach (var info in tableInfo)
        {
            columnNames.Add(info.name);
        }

        return columnNames;
    }

    public class TableInfo
    {
        public string name { get; set; }
    }

    public void OnTabAddBackBtn()
    {
        if (tabAddContent.childCount != 0)
        {
            foreach (Transform child in tabAddContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        columnNames.Clear();
        values.Clear();
        DataBasePanelScript.instance.tableAddPanel.SetActive(false);
    }

    public void OnTabEntryOKBtn()
    {
        // Open a connection to the database
        connection = new SQLiteConnection(dataBasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        for (int i = 1; i < columnNames.Count; i++)
        {
            string value = tabAddContent.GetChild(i).GetChild(1).GetComponent<TMP_InputField>().text;
            values.Add(value);
        }

        // Exclude the Primary Key column from the column names
        columnNames.RemoveAt(0);

        // Build the dynamic insert query
        string columns = string.Join(", ", columnNames);
        string valuePlaceholders = string.Join(", ", columnNames.ConvertAll(c => "?"));
        string query = $"INSERT INTO {tableName} ({columns}) VALUES ({valuePlaceholders})";

        // Execute the insert query with parameters
        connection.Execute(query, values.ToArray());

        StartCoroutine(OnEntryAddSuccess());

        // Closing the connection to the database
        connection.Close();
    }


    private IEnumerator OnEntryAddSuccess()
    {
        successText.SetActive(true);
        yield return new WaitForSeconds(3f);
        successText.SetActive(false);
    }
}
