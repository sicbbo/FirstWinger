using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BGScrollData
{
    public Renderer rendererForScroll;
    public float speed;
    public float offsetX;
}

public class BGScroller : MonoBehaviour
{
    [SerializeField]
    private BGScrollData[] scrollDatas = null;

    private void Update()
    {
        UpdateScroll();
    }

    private void UpdateScroll()
    {
        for (int i = 0; i < scrollDatas.Length; i++)
        {
            SetTextureOffset(scrollDatas[i]);
        }
    }

    private void SetTextureOffset(BGScrollData _scrollData)
    {
        _scrollData.offsetX += (float)_scrollData.speed * Time.deltaTime;
        if (_scrollData.offsetX > 1f)
            _scrollData.offsetX = _scrollData.offsetX % 1.0f;

        Vector2 offset = new Vector2(_scrollData.offsetX, 0f);

        _scrollData.rendererForScroll.material.SetTextureOffset("_MainTex", offset);
    }
}
