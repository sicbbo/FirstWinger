using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableLoader<TMarshalStruct> : MonoBehaviour
{
    [SerializeField]
    protected string filePath = string.Empty;

    TableRecordParser<TMarshalStruct> tableRecordParser = new TableRecordParser<TMarshalStruct>();

    public bool Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if (textAsset == null)
        {
            Debug.LogError(string.Format("Load Failed! filePath = {0}", filePath));
            return false;
        }

        ParseTable(textAsset.text);

        return true;
    }

    private void ParseTable(string _text)
    {
        StringReader reader = new StringReader(_text);

        string line = null;
        bool fieldRead = false;
        while ((line = reader.ReadLine()) != null)
        {
            if (fieldRead == false)
            {
                fieldRead = true;
                continue;
            }

            TMarshalStruct data = tableRecordParser.ParseRecordLine(line);
            AddData(data);
        }
    }

    protected virtual void AddData(TMarshalStruct _data)
    {

    }
}