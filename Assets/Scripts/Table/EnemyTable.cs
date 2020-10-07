using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct EnemyStruct
{
    public int      index;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MarshalTableConstant.charBufferSize)]
    public string   filePath;
    public int      maxHP;
    public int      damage;
    public int      crashDamage;
    public int      bulletSpeed;
    public int      fireRemainCount;
    public int      gamePoint;
}

public class EnemyTable : TableLoader<EnemyStruct>
{
    private Dictionary<int, EnemyStruct> tableDatas = new Dictionary<int, EnemyStruct>();

    private void Start()
    {
        Load();
    }

    protected override void AddData(EnemyStruct _data)
    {
        tableDatas.Add(_data.index, _data);
    }

    public EnemyStruct GetEnemyData(int _index)
    {
        if (tableDatas.ContainsKey(_index) == false)
        {
            Debug.LogError(string.Format("GetEnemyData Error! index = {0}", _index.ToString()));
            return default(EnemyStruct);
        }

        return tableDatas[_index];
    }
}