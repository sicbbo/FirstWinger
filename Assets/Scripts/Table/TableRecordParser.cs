using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class MarshalTableConstant
{
    public const int charBufferSize = 256;
}

public class TableRecordParser<TMarshalStruct>
{
    public TMarshalStruct ParseRecordLine(string _line)
    {
        Type type = typeof(TMarshalStruct);
        int structSize = Marshal.SizeOf(type);
        byte[] structBytes = new byte[structSize];
        int structBytesIndex = 0;

        const string spliter = ",";
        string[] fieldDataList = _line.Split(spliter.ToCharArray());

        Type dataType;
        string splited;
        byte[] fieldBytes;
        //byte[] keyBytes;

        FieldInfo[] fieldInfos = type.GetFields();
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            dataType = fieldInfos[i].FieldType;
            splited = fieldDataList[i];

            fieldBytes = new byte[4];
            MakeBytesByFieldType(out fieldBytes, dataType, splited);

            Buffer.BlockCopy(fieldBytes, 0, structBytes, structBytesIndex, fieldBytes.Length);
            structBytesIndex += fieldBytes.Length;

            //if (i == 0)
            //    keyBytes = fieldBytes;
        }

        TMarshalStruct tStruct = MakeStructFromBytes<TMarshalStruct>(structBytes);
        return tStruct;
    }

    protected void MakeBytesByFieldType(out byte[] _fieldBytes, Type _dataType, string _splite)
    {
        _fieldBytes = new byte[1];

        if (typeof(int) == _dataType)
        {
            _fieldBytes = BitConverter.GetBytes(int.Parse(_splite));
        }
        else
        if (typeof(float) == _dataType)
        {
            _fieldBytes = BitConverter.GetBytes(float.Parse(_splite));
        }
        else
        if (typeof(bool) == _dataType)
        {
            bool value = bool.Parse(_splite);
            int temp = value ? 1 : 0;

            _fieldBytes = BitConverter.GetBytes((int)temp);
        }
        else
        if (typeof(string) == _dataType)
        {
            _fieldBytes = new byte[MarshalTableConstant.charBufferSize];
            byte[] byteArr = Encoding.UTF8.GetBytes(_splite);
            Buffer.BlockCopy(byteArr, 0, _fieldBytes, 0, byteArr.Length);
        }
    }

    public static T MakeStructFromBytes<T>(byte[] _bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(_bytes, 0, ptr, size);

        T tStruct = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return tStruct;
    }
}