using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaskDrawer : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Texture2D generatedTexture;
    private RawImage rawImage;
    private RectTransform rt;
    private Vector2Int size;

    private Color[] colorMap;

    public int brushSize = 5;
    public Color brushColor = Color.black;
    public Color resetColor = Color.white;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Start()
    {
        rt = transform as RectTransform;
        
        Assert.IsNotNull(rt, "Missing RectTransform");
        size.x = (int)rt.rect.size.x;
        size.y = (int)rt.rect.size.y;

        generatedTexture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Point;
        
        rawImage.texture = generatedTexture;
        
        colorMap = new Color[size.x * size.y];
        ResetColorMap();
        
        generatedTexture.SetPixels(colorMap);
        generatedTexture.Apply();
    }

   
    public void OnDraw(Vector2 position, bool erase)
    {
        var corner = new Vector2(rt.position.x - rt.pivot.x * rt.rect.size.x, rt.position.y - rt.pivot.y * rt.rect.size.y);
        var pointerPosition = position;
        var localPos = pointerPosition - corner;

        if (localPos.x < 0 || localPos.x > size.x || localPos.y < 0 || localPos.y > size.y)
        {
            return;
        }
        
        DrawBrush((int)localPos.x, (int)localPos.y, erase ? resetColor : brushColor);
        
        generatedTexture.SetPixels(colorMap);
        generatedTexture.Apply();
    }

    void DrawBrush(int x, int y, Color color)
    {
        for (int i = -brushSize; i < brushSize; i++)
        {
            if (x + i < 0 || x + i > size.x)
            {
                continue;
            }

            var yDist = Mathf.RoundToInt(Mathf.Sqrt(brushSize * brushSize - i * i));
            for (int j = -yDist; j < yDist; j++)
            {
                if (y + j < 0 || y + j > size.y)
                {
                    continue;
                }
                
                DrawPoint(x + i,y + j, color);
            }
        }
    }

    void DrawPoint(int x, int y, Color color)
    {
        colorMap[y * size.y + x] = color;
    }
    
    void ResetColorMap()
    {
        for (var i = 0; i < colorMap.Length; i++)
        {
            colorMap[i] = Color.white;
        }
    }

    public void Clear()
    {
        ResetColorMap();
        generatedTexture.SetPixels(colorMap);
        generatedTexture.Apply();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    { 
        var erase = eventData.button == PointerEventData.InputButton.Right;
        prevPosition = eventData.position;
        OnDraw(eventData.position,erase);
    }
    
    private Vector2 prevPosition;
    public void OnDrag(PointerEventData eventData)
    {
        var erase = eventData.button == PointerEventData.InputButton.Right;
        var pointerPosition = eventData.position;
        
        if (prevPosition == pointerPosition)
        {
            OnDraw(eventData.position,erase);
        }
        
        var dist = Mathf.Sqrt((pointerPosition.x - prevPosition.x) * (pointerPosition.x - prevPosition.x) + (pointerPosition.y - prevPosition.y) * (pointerPosition.y - prevPosition.y));
        for (int i = 0; i < dist; i++)
        {
            var frac = i / dist;
            var point = prevPosition + frac * (pointerPosition - prevPosition);
            OnDraw(point,erase);
        }
        prevPosition = pointerPosition;
    }
    
    
}
