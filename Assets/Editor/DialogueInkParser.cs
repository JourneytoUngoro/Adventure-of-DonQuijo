using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DialogueInkParser : EditorWindow
{
    private TextAsset csvFile;

    [MenuItem("Tools/CSV to Ink Converter")]
    public static void ShowWindow()
    {
        GetWindow<DialogueInkParser>("CSV to Ink");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV to Ink Converter", EditorStyles.boldLabel);
        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV file", csvFile, typeof(TextAsset), false);

        if (GUILayout.Button("Execute") && csvFile != null)
        {
            string output = ConvertCsvToInk(csvFile.text);
            string fileName = Path.GetFileNameWithoutExtension(csvFile.name) + ".ink";
            string path = EditorUtility.SaveFilePanel("Ink save path", "", fileName, "ink");

            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, output);
                Debug.Log("convert completed! file saved : " + path);
            }
        }
    }


    private List<string[]> ParseCsv(string csvText)
    {
        var result = new List<string[]>();
        var lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Regex csvSplit = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");

        foreach (string line in lines)
        {
            string[] fields = csvSplit.Split(line);

            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = fields[i].Trim().Trim('"');
            }

            result.Add(fields);
        }

        return result;
    }


    private string ConvertCsvToInk(string csvText)
    {
        var lines = ParseCsv(csvText);
        string[] headers = lines[0];

        string Get(string[] row, string column)
        {
            int index = Array.IndexOf(headers, column);
            return index >= 0 && index < row.Length ? row[index].Trim() : "";
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        builder.AppendLine("-> ID_1");

        for (int i = 1; i < lines.Count; i++)
        {
            string[] row = lines[i];

            string id = Get(row, "ID");
            string speaker = Get(row, "Speaker");
            string text = Get(row, "Text");
            string portrait = Get(row, "Portrait");
            string method = Get(row, "Method");
            string nextID = Get(row, "NextID");
            string choice1 = Get(row, "Choice1");
            string choice1Next = Get(row, "Choice1Next");
            string choice2 = Get(row, "Choice2");
            string choice2Next = Get(row, "Choice2Next");
            string choice3 = Get(row, "Choice3");
            string choice3Next = Get(row, "Choice3Next");

            builder.AppendLine($"=== ID_{id} ===");

            builder.Append($"{text}");

            if (!string.IsNullOrEmpty(speaker))
            {
                builder.Append($" #speaker:{speaker} ");
            }

            if (!string.IsNullOrEmpty(portrait))
                builder.Append($"#portrait:{portrait} ");
/*
            if (!string.IsNullOrEmpty(method))
            {
                builder.AppendLine($"// Should be modified {method}");
            }*/

            if (!string.IsNullOrEmpty(choice1) && !string.IsNullOrEmpty(choice1Next))
            {
                builder.Append($"\n+[{choice1}] -> ID_{choice1Next}");
            }

            if (!string.IsNullOrEmpty(choice2) && !string.IsNullOrEmpty(choice2Next))
            {
                builder.Append($"\n+[{choice2}] -> ID_{choice2Next}");
            }

            if (!string.IsNullOrEmpty(choice3) && !string.IsNullOrEmpty(choice3Next))
            {
                builder.Append($"\n+[{choice3}] -> ID_{choice3Next}");
            }

            if (string.IsNullOrEmpty(choice1) && string.IsNullOrEmpty(choice2) && string.IsNullOrEmpty(choice3))
            {
                if (!string.IsNullOrEmpty(nextID))
                    builder.AppendLine($"\n-> ID_{nextID}");
                else
                    builder.AppendLine("\n-> DONE");
            }

            builder.AppendLine(); // 구분 줄
        }

        return builder.ToString();
    }
}
