using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatePanel : BasePanel
{
    [SerializeField]
    private Text scoreValue = null;
    [SerializeField]
    private UIGage HPGage = null;

    private Player player = null;
    public Player Player
    {
        get
        {
            if (player == null)
                player = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Player;
            return player;
        }
    }

    public void SetScore(int _value)
    {
        scoreValue.text = _value.ToString();
    }

    public override void InitializePanel()
    {
        base.InitializePanel();
        HPGage.SetGage(100, 100);
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();
        if (player != null)
        {
            HPGage.SetGage(player.CurrentHP, player.MaxHP);
        }
    }
}