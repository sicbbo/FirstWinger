using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SquadronScheduleDataStruct
{
    public int index;
    public float generateTime;
    public int squadronID;
}

public class SquadronScheduleTable : TableLoader<SquadronScheduleDataStruct>
{
    private List<SquadronScheduleDataStruct> dataTables = new List<SquadronScheduleDataStruct>();

    protected override void AddData(SquadronScheduleDataStruct _data)
    {
        dataTables.Add(_data);
    }

    public SquadronScheduleDataStruct GetScheduleData(int _index)
    {
        if (_index < 0 || _index >= dataTables.Count)
        {
            Debug.LogError(string.Format("GetScheduleData Error! index = ", _index));
            return default(SquadronScheduleDataStruct);
        }

        return dataTables[_index];
    }
}