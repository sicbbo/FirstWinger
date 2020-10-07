using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SquadronMemberStruct
{
    public int index;
    public int enemyID;
    public float generatePointX;
    public float generatePointY;
    public float appearPointX;
    public float appearPointY;
    public float disappearPointX;
    public float disappearPointY;
}

public class SquadronTable : TableLoader<SquadronMemberStruct>
{
    private List<SquadronMemberStruct> tableDatas = new List<SquadronMemberStruct>();

    protected override void AddData(SquadronMemberStruct _data)
    {
        tableDatas.Add(_data);
    }

    public SquadronMemberStruct GetSquadronMember(int _index)
    {
        if (_index < 0 || _index >= tableDatas.Count)
        {
            Debug.LogError(string.Format("GetSquadronMember Error! index = ", _index));
            return default(SquadronMemberStruct);
        }

        return tableDatas[_index];
    }

    public int GetCount()
    {
        return tableDatas.Count;
    }
}