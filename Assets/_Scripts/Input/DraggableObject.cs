using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class DraggableObject<T> : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public T element {  get;  set; }

    private Image _elementImage;
    public Image elementImage
    {
        get
        {
            if (_elementImage == null) { _elementImage = GetComponent<Image>(); }
            return _elementImage;
        }
        set
        {
            _elementImage = value;
        }
    }


    [HideInInspector] public Transform parentTransform;
    [HideInInspector] public Slot<T> slot;

    Transform canvasTransform;

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null )
        {
            canvasTransform = canvas.transform;
        }

        parentTransform = transform.parent;
        slot = GetComponentInParent<Slot<T>>();
    }

    public virtual void InitElements() { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (element == null) return;

        Debug.Assert(canvasTransform != null, "Canvas null");

        parentTransform = transform.parent;
        transform.SetParent(canvasTransform);
        transform.SetAsLastSibling();
        elementImage.raycastTarget = false;
    
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (element == null) return;

        transform.position = Input.mousePosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (element == null) return;

        List<RaycastResult> results = new List<RaycastResult> ();
            
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            var droppedSlot = result.gameObject.GetComponent<Slot<T>>();
            if (droppedSlot != null)
            {
                transform.SetParent(parentTransform);
                droppedSlot.OnDrop(this);
                elementImage.raycastTarget = true;
                DroppedOutsideSlot();
                return;
            }
        }

        // 인벤토리 슬롯이 아닌 외부에 놓인 경우 
        transform.SetParent (parentTransform);
        elementImage.raycastTarget = true;

        // TODO : 슬롯 외부에 뒀을 때 로직 추가
        Debug.Log("슬롯 외부에 element를 뒀습니다.");
        DroppedInsideSlot();
    }

    public virtual void DroppedOutsideSlot() { }
    public virtual void DroppedInsideSlot() { }

}
