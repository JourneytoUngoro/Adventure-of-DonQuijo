using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    private Story globalVariablesStory;
    private const string SaveVariablesKey = "SaveVariables";

    public DialogueVariables(TextAsset globalsJSON)
    {
        globalVariablesStory = new Story(globalsJSON.text);

        variables = new Dictionary<string, Ink.Runtime.Object>();

        // IEnumerable<string> variableState
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log(name + " " + value.ToString());
        }
    }

    public void SaveVariables()
    {
        if (globalVariablesStory != null) return;

        VariablesToStory(globalVariablesStory);

        // TODO : link save-load system
    }

    public void StartListening(Story story)
    {
        VariablesToStory(story);

        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    public void VariableChanged(string name, Ink.Runtime.Object value)
    {
        if (variables.ContainsKey(name))
        {
            Debug.Log($"changed variable {name}");
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

}
