using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadronManager : MonoBehaviour
{
    private float gameStartedTime = 0f;
    private int squadronIndex = 0;
    private bool isRunning = false;

    [SerializeField]
    private SquadronTable[] squadronDatas = null;
    [SerializeField]
    private SquadronScheduleTable squadronScheduleTable = null;

    private void Start()
    {
        squadronDatas = GetComponentsInChildren<SquadronTable>();
        for (int i = 0; i < squadronDatas.Length; i++)
        {
            squadronDatas[i].Load();
        }

        squadronScheduleTable.Load();
    }

    private void Update()
    {
        CheckSquadronGeneratings();
    }

    public void StartGame()
    {
        gameStartedTime = Time.time;
        squadronIndex = 0;
        isRunning = true;
    }

    private void CheckSquadronGeneratings()
    {
        if (isRunning == false)
            return;

        if (Time.time - gameStartedTime >= squadronScheduleTable.GetScheduleData(squadronIndex).generateTime)
        {
            GenerateSquadron(squadronDatas[squadronIndex]);
            squadronIndex++;

            if (squadronIndex >= squadronDatas.Length)
            {
                AllSquadronGenerated();
                return;
            }
        }
    }

    private void GenerateSquadron(SquadronTable _table)
    {
        for (int i = 0; i < _table.GetCount(); i++)
        {
            SquadronMemberStruct squadronMember = _table.GetSquadronMember(i);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GenerateEnemy(squadronMember);
        }
    }

    private void AllSquadronGenerated()
    {
        isRunning = false;
    }
}