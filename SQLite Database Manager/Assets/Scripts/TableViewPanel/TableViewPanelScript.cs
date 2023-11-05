using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using SQLite4Unity3d;

public class TableViewPanelScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tableText;

    [SerializeField] GameObject rowPrefab;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject updatePrefab;
    [SerializeField] GameObject successText;

    [SerializeField] Transform tabViewContent;
    [SerializeField] string dataBasePath;
    [SerializeField] string tableName;

    public bool inUpdateMode;
    int rowCount;
    int columnCount;
    List<string> columnNames = new List<string>();
    List<float> columnLength = new List<float>();

    private SQLiteConnection connection;


    public void OnTableViewPanelOpen(string dbPath, string tabName)
    {
        dataBasePath = dbPath;
        tableName = tabName;
        tableText.text = tableName;
        inUpdateMode = false;
        OnCreateTableUI();
    }

    private void OnCreateTableUI()
    {
        // Open a connection to the database
        connection = new SQLiteConnection(dataBasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        columnNames = GetColumnNames();
        columnCount = columnNames.Count;
        rowCount = GetRowCount();

        OnCreatingHeader();
        OnCreatingFooter();
        OnAdjustCellLength();

        // Closing the connection to the database
        connection.Close();
    }

    private void OnCreatingHeader()
    {
        GameObject go = Instantiate(rowPrefab, tabViewContent);
        go.name = "Table Header";
        Vector2 sizeDelta = go.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = 150f;
        go.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        for (int i = 0; i < columnCount; i++)
        {
            GameObject go2 = Instantiate(cellPrefab, go.transform);
            go2.name = columnNames[i];
            go2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = columnNames[i];
            go2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            go2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 34f;
        }
    }

    private void OnCreatingFooter()
    {
        for (int i = 0; i < rowCount; i++)
        {
            GameObject go = Instantiate(rowPrefab, tabViewContent);
            go.name = "Row" + (i + 1);
            for (int j = 0; j < columnCount; j++)
            {
                GameObject go2 = Instantiate(cellPrefab, go.transform);
                go2.name = columnNames[j];
                string rowData;
                if (j == 0) 
                {
                    rowData = (i + 1).ToString();
                }
                else
                {
                    rowData = GeTabCellData(tableName, columnNames[j], columnNames[0], (i + 1).ToString());
                }
                go2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = rowData;
            }
        }
    }

    private void OnAdjustCellLength()
    {
        for (int i = 0; i < tabViewContent.childCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                float length = tabViewContent.GetChild(i).GetChild(j).GetChild(0).GetComponent<TextMeshProUGUI>().preferredWidth;
                if (columnLength.Count == j)
                {
                    columnLength.Add(length);
                }
                else
                {
                    if (columnLength[j] < length)
                    {
                        columnLength[j] = length;
                    }
                }
            }
        }

        OnAdjustWidth();
    }

    private void OnAdjustWidth()
    {
        for (int i = 0; i < tabViewContent.childCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                int quotient = Mathf.FloorToInt(columnLength[j] / 50);
                quotient *= 50;
                quotient += 75;
                Vector2 sizeDelta2 = tabViewContent.GetChild(i).GetChild(j).GetComponent<RectTransform>().sizeDelta;
                sizeDelta2.x = quotient;
                tabViewContent.GetChild(i).GetChild(j).GetComponent<RectTransform>().sizeDelta = sizeDelta2;
            }
        }
    }

    public int GetRowCount()
    {
        // Use COUNT function to retrieve the number of rows
        string query = $"SELECT COUNT(*) FROM {tableName}";
        return connection.ExecuteScalar<int>(query);
    }

    public List<string> GetColumnNames()
    {
        // Use PRAGMA table_info query to retrieve column information
        var tableInfo = connection.Query<TableInfo>($"PRAGMA table_info('{tableName}')");

        foreach (var info in tableInfo)
        {
            columnNames.Add(info.name);
        }

        return columnNames;
    }

    public void OnUpdateTableBtn()
    {
        if (inUpdateMode)
        {
            OnUpdatingDB();
            StartCoroutine(OnDBSuccessUpdated());
            return;
        }

        //Debug.Log("****");
        // Open a connection to the database
        connection = new SQLiteConnection(dataBasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // Destroy all Cells
        if (tabViewContent.childCount != 0)
        {
            foreach (Transform child in tabViewContent)
            {
                Destroy(child.gameObject);
            }
        }

        // Create Header
        OnCreatingHeader();

        // Create New Input Field Cell
        for (int i = 0; i < rowCount; i++)
        {
            GameObject go = Instantiate(rowPrefab, tabViewContent);
            go.name = "Row" + (i + 1);
            for (int j = 0; j < columnCount; j++)
            {
                GameObject go2 = Instantiate(updatePrefab, go.transform);
                go2.name = columnNames[j];
                string rowData;
                if (j == 0)
                {
                    rowData = (i + 1).ToString();
                    go2.transform.GetChild(0).GetComponent<TMP_InputField>().interactable = false;
                }
                else
                {
                    rowData = GeTabCellData(tableName, columnNames[j], columnNames[0], (i + 1).ToString());
                }
                go2.transform.GetChild(0).GetComponent<TMP_InputField>().text = rowData;
            }
        }

        // Adjusting Width
        OnAdjustWidth();
        inUpdateMode = true;

        // Closing the connection to the database
        connection.Close();
    }

    private void OnUpdatingDB()
    {
        // Open a connection to the database
        connection = new SQLiteConnection(dataBasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);


        for (int i = 1; i < tabViewContent.childCount; i++)
        {
            for (int j = 1; j < columnCount; j++)
            {
                string rowData;
                rowData = tabViewContent.GetChild(i).GetChild(j).GetChild(0).GetComponent<TMP_InputField>().text;
                UpdateDBData(tableName, columnNames[j], columnNames[0], (i).ToString(), rowData);
            }
        }

        // Closing the connection to the database
        connection.Close();
    }

    public class TableInfo
    {
        public string name { get; set; }
    }

    public string GeTabCellData(string tableName, string columnName, string priKeyName, string priKey)
    {
        // Prepare the query
        var query = $"SELECT {columnName} FROM {tableName} WHERE {priKeyName} = '{priKey}'";

        // Execute the query
        var result = connection.ExecuteScalar<string>(query);

        if (result == null)
        {
            result = "null";
        }

        // Return the GameState
        return result.ToString();
    }
    
    public void UpdateDBData(string tableName, string columnName, string priKeyName, string priKey, string newData)
    {
        // Prepare the query
        var query = $"UPDATE {tableName} SET {columnName} = '{newData}' WHERE {priKeyName} = '{priKey}'";

        // Execute the query
        connection.Execute(query);
    }

    private IEnumerator OnDBSuccessUpdated()
    {
        successText.SetActive(true);
        yield return new WaitForSeconds(3f);
        successText.SetActive(false);
    }

    public void OnTabViewBackBtn()
    {
        if (tabViewContent.childCount != 0)
        {
            foreach (Transform child in tabViewContent)
            {
                Destroy(child.gameObject);
            }
        }

        inUpdateMode = false;
        columnNames.Clear();
        columnLength.Clear();
        DataBasePanelScript.instance.tableViewPanel.SetActive(false);
    }
}
