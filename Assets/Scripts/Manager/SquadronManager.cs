using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadronManager : MonoBehaviour
{
    private float gameStartedTime = 0f;
    private int scheduleIndex = 0;
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
        scheduleIndex = 0;
        isRunning = true;
    }

    private void CheckSquadronGeneratings()
    {
        if (isRunning == false)
            return;

        SquadronScheduleDataStruct data = squadronScheduleTable.GetScheduleData(scheduleIndex);
        if (Time.time - gameStartedTime >= squadronScheduleTable.GetScheduleData(scheduleIndex).generateTime)
        {
            GenerateSquadron(squadronDatas[data.squadronID]);
            scheduleIndex++;

            if (scheduleIndex >= squadronScheduleTable.GetDataCount())
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