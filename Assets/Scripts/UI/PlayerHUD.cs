using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField]
    private UIGage hpGage = null;

    [SerializeField]
    private Player owerPlayer = null;

    private Transform owerTrans = null;
    private Transform selfTrans = null;

    private void Start()
    {
        selfTrans = transform;
    }

    public void Initialize(Player _player)
    {
        owerPlayer = _player;
        owerTrans = owerPlayer.transform;
    }

    private void Update()
    {
        UpdatePosition();
        UpdateHP();
    }

    private void UpdatePosition()
    {
        if (owerTrans != null)
            selfTrans.position = Camera.main.WorldToScreenPoint(owerTrans.position);
    }

    private void UpdateHP()
    {
        if (owerPlayer != null)
        {
            if (owerPlayer.gameObject.activeSelf == false)
                gameObject.SetActive(owerPlayer.gameObject.activeSelf);

            hpGage.SetGage(owerPlayer.CurrentHP, owerPlayer.MaxHP);
        }
    }
}