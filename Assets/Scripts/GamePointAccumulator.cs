using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePointAccumulator
{
    private int gamePoint = 0;
    public int GamePoint { get { return gamePoint; } }

    private PlayerStatePanel playerStatePanel = null;

    public void Accumulate(int _value)
    {
        gamePoint += _value;

        if (playerStatePanel == null)
            playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetScore(gamePoint);
    }

    public void Reset()
    {
        gamePoint = 0;
    }
}