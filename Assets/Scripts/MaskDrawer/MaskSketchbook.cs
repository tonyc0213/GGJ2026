using System;
using PartsGen;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaskDrawer
{
    public class MaskSketchbook : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler, IEndDragHandler
    {
        public Texture2D generatedTexture;
        public ColorTable colorTable;

        private Canvas canvas;
        private RawImage rawImage;
        private RectTransform rt;
        private Vector2Int size;
        private Color[] colorMap;

        public int brushSize = 5;
        public Color brushColor = Color.black;
        public Color resetColor = Color.white;

        public Transform cursorIndicator;

        public AudioSource audioSource;
        
        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rawImage = GetComponent<RawImage>();
        
            rt = transform as RectTransform;
            Assert.IsNotNull(rt, "Missing RectTransform");
            size.x = (int)rt.rect.size.x;
            size.y = (int)rt.rect.size.y;
            colorMap = new Color[size.x * size.y];
            generatedTexture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            generatedTexture.filterMode = FilterMode.Point;
        }

        private void Start()
        {
            rawImage.texture = generatedTexture;
            ResetColorMap();
    
            generatedTexture.SetPixels(colorMap);
            generatedTexture.Apply();
        
            SetBrushColor(0);
        }

        public void SetTexture(Texture2D texture)
        {
            Assert.IsTrue(texture.dimension == generatedTexture.dimension, "Invalid Texture dimension.");
            texture.GetPixels().CopyTo(colorMap, 0);
        
            generatedTexture.SetPixels(colorMap);
            generatedTexture.Apply();
        }
    
        public void SetBrushColor(int colorID)
        {
            var color = colorTable.GetItem(colorID);
            brushColor = color;
        }

        public void SetAllowDrawing(bool canDraw)
        {
            this.canDraw = canDraw;
        }

        public Texture2D ExportTexture()
        {
            var texture =  new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(generatedTexture.GetPixels());
            texture.Apply();
            return texture;
        }

        private bool canDraw;

        public void OnDraw(Vector2 position, bool erase)
        {
            if (!canDraw)
            {
                return;
            }
            
            var corner = (Vector2)rt.position;
            var pointerPosition = position;
            var scale = canvas.transform.lossyScale;
            var v = (pointerPosition - corner);
            var localPos = new Vector2(v.x / scale.x, v.y / scale.y);

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
            var index = y * size.x + x;
            if(index < 0 || index > colorMap.Length)
            {
                return;
            }
            colorMap[index] = color;
        }

        void ResetColorMap()
        {
            for (var i = 0; i < colorMap.Length; i++)
            {
                colorMap[i] = resetColor;
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
                OnDraw(pointerPosition,erase);
            }
            else
            {
                var dist = Mathf.Sqrt((pointerPosition.x - prevPosition.x) * (pointerPosition.x - prevPosition.x) + (pointerPosition.y - prevPosition.y) * (pointerPosition.y - prevPosition.y));
                for (int i = 0; i < dist; i++)
                {
                    var frac = i / dist;
                    var point = prevPosition + frac * (pointerPosition - prevPosition);
                    OnDraw(point,erase);
                }
            }
            
            prevPosition = pointerPosition;
        }

        private Vector2 prevSoundPos;
        private void Update()
        {
            if(Input.mouseScrollDelta.y > 0)
            {
                brushSize = Math.Min(brushSize + 1, 8);
                UpdateBrushSize();
            }
            else if(Input.mouseScrollDelta.y < 0)
            {
                brushSize = Math.Max(brushSize - 1, 1);
                UpdateBrushSize();
            }
        }

        private void FixedUpdate()
        {
            if (Input.GetMouseButton(0) && canDraw)
            {
                CheckPlaySound(prevSoundPos, prevPosition);
            }
            prevSoundPos = prevPosition;
        }

        private void UpdateBrushSize()
        {
            var cursorRT = cursorIndicator.transform as RectTransform;
            cursorRT.sizeDelta = new Vector2(brushSize*2, brushSize*2);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UpdateBrushSize();
            cursorIndicator.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cursorIndicator.gameObject.SetActive(false);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            cursorIndicator.position = eventData.position;
        }

        [Header("AudioSettings")] 
        public float playThreshold;
        public float maxPitchMagnitude;
        public float maxPitchUp;
        
        void CheckPlaySound(Vector2 prevPos, Vector2 currentPos)
        {
            var magnitude = (currentPos - prevPos).magnitude;
            if (magnitude > playThreshold)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                
                audioSource.pitch = 1 + Math.Min((magnitude - playThreshold) / (maxPitchMagnitude - playThreshold),1) * maxPitchUp;
            }
            else
            {
                audioSource.Stop();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            audioSource.Stop();
        }
    }
}

