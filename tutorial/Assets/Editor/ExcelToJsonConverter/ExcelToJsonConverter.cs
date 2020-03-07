using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ExcelToJsonConverter
{
    public delegate void ConversionToJsonSuccessfullHandler();
    public event ConversionToJsonSuccessfullHandler ConversionToJsonSuccessfull = delegate { };

    public delegate void ConversionToJsonFailedHandler();
    public event ConversionToJsonFailedHandler ConversionToJsonFailed = delegate { };

    /// <summary>
    /// Converts all excel files in the input folder to json and saves them in the output folder.
    /// Each sheet within an excel file is saved to a separate json file with the same name as the sheet name.
    /// Files, sheets and columns whose name begin with '~' are ignored.
    /// </summary>
    /// <param name="inputPath">Input path.</param>
    /// <param name="outputPath">Output path.</param>
    public void ConvertExcelFilesToJson(string inputPath, string outputPath)
    {
        List<string> excelFiles = GetExcelFileNamesInDirectory(inputPath);
        Debug.Log("Excel To Json Converter: " + excelFiles.Count.ToString() + " excel files found.");

        bool succeeded = true;

        for (int i = 0; i < excelFiles.Count; i++)
        {
            if (!ConvertExcelFileToJson(excelFiles[i], outputPath))
            {
                succeeded = false;
                break;
            }
        }

        if (succeeded)
        {
            ConversionToJsonSuccessfull();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            ConversionToJsonFailed();
        }
    }

    /// <summary>
    /// Gets all the file names in the specified directory
    /// </summary>
    /// <returns>The excel file names in directory.</returns>
    /// <param name="directory">Directory.</param>
    private List<string> GetExcelFileNamesInDirectory(string directory)
    {
        string[] directoryFiles = Directory.GetFiles(directory);
        List<string> excelFiles = new List<string>();

        // Regular expression to match against 2 excel file types (xls & xlsx), ignoring
        // files with extension .meta and starting with ~$ (temp file created by excel when fie
        Regex excelRegex = new Regex(@"^((?!(~\$)).*\.(xlsx|xls$))$");

        for (int i = 0; i < directoryFiles.Length; i++)
        {
            string fileName = directoryFiles[i].Substring(directoryFiles[i].LastIndexOf('/') + 1);

            if (excelRegex.IsMatch(fileName))
            {
                excelFiles.Add(directoryFiles[i]);
            }
        }

        return excelFiles;
    }

    /// <summary>
    /// Converts each sheet in the specified excel file to json and saves them in the output folder.
    /// The name of the processed json file will match the name of the excel sheet. Ignores
    /// sheets whose name begin with '~'. Also ignores columns whose names begin with '~'.
    /// </summary>
    /// <returns><c>true</c>, if excel file was successfully converted to json, <c>false</c> otherwise.</returns>
    /// <param name="filePath">File path.</param>
    /// <param name="outputPath">Output path.</param>
    public bool ConvertExcelFileToJson(string filePath, string outputPath)
    {
        Debug.Log("Excel To Json Converter: Processing: " + filePath);
        DataSet excelData = GetExcelDataSet(filePath);

        if (excelData == null)
        {
            Debug.LogError("Excel To Json Converter: Failed to process file: " + filePath);
            return false;
        }

        string spreadSheetJson = "";

        // Process Each SpreadSheet in the excel file
        for (int i = 0; i < excelData.Tables.Count; i++)
        {
            spreadSheetJson = GetSpreadSheetJson(excelData, excelData.Tables[i].TableName);
            if (String.IsNullOrEmpty(spreadSheetJson))
            {
                Debug.LogError("Excel To Json Converter: Failed to covert Spreadsheet '" + excelData.Tables[i].TableName + "' to json.");
                return false;
            }
            else
            {
                // The file name is the sheet name with spaces removed
                string fileName = excelData.Tables[i].TableName.Replace(" ", string.Empty);
                WriteTextToFile(spreadSheetJson, outputPath + "/" + fileName + ".json");
                Debug.Log("Excel To Json Converter: " + excelData.Tables[i].TableName + " successfully written to file.");
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the Excel data from the specified file
    /// </summary>
    /// <returns>The excel data set or null if file is invalid.</returns>
    /// <param name="filePath">File path.</param>
    private DataSet GetExcelDataSet(string filePath)
    {
        // Get the data from the excel file
        DataSet data = new DataSet();

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // FIXME : dispose 시 exception
            //using (var reader = ExcelReaderFactory.CreateReader(stream))
            var reader = ExcelReaderFactory.CreateReader(stream);
            {
                data = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                
                return data;
            }
        }
    }

    /// <summary>
    /// Gets the json data for the specified spreadsheet in the specified DataSet
    /// </summary>
    /// <returns>The spread sheet json.</returns>
    /// <param name="excelDataSet">Excel data set.</param>
    /// <param name="sheetName">Sheet name.</param>
    private string GetSpreadSheetJson(DataSet excelDataSet, string sheetName)
    {
        // Get the specified table
        DataTable dataTable = excelDataSet.Tables[sheetName];

        // Remove empty columns
        for (int col = dataTable.Columns.Count - 1; col >= 0; col--)
        {
            bool removeColumn = true;
            foreach (DataRow row in dataTable.Rows)
            {
                if (!row.IsNull(col))
                {
                    removeColumn = false;
                    break;
                }
            }

            if (removeColumn)
            {
                dataTable.Columns.RemoveAt(col);
            }
        }

        // Remove columns which start with '~'
        Regex columnNameRegex = new Regex(@"^~.*$");
        for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
        {
            if (columnNameRegex.IsMatch(dataTable.Columns[i].ColumnName))
            {
                dataTable.Columns.RemoveAt(i);
            }
        }

        // Serialze the data table to json string
        return Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);
    }

    /// <summary>
    /// Writes the specified text to the specified file, overwriting it.
    /// Creates file if it does not exist.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="filePath">File path.</param>
    private void WriteTextToFile(string text, string filePath)
    {
        System.IO.File.WriteAllText(filePath, text);
    }

}
