using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
///  =============  씬 계층 구조 ================
///  [ Canvas ]
///    | ---------- [ Slots ]
///    | ---------- | ---------- [ Slot<T> ]
///    | ---------- | ---------- | ----------  [ DraggableObject<T> ]
///    ...
///    
/// =======================================
/// 
///  ============= 연관 구조 ===================
///  [ Child Slot ] ---> [ Slot<T> ] <---> [ DraggableObject<T> ] <--- [ Child DraggableElement ] 
/// 
///  Slot은 DraggableElement를, DraggableElement는 Slot을 서로 참조한다 
///  
/// ========================================
/// </summary>

public class Slot<T> : MonoBehaviour
{
    [field: SerializeField] public int SlotId {  get; set; }
    private DraggableObject<T> _draggableElement;

    public DraggableObject<T> DraggableElement
    {
        get
        {
            if (_draggableElement == null) { _draggableElement = GetComponentInChildren<DraggableObject<T>>();  }
            return _draggableElement;
        }
        set
        {
            _draggableElement = value;
        }
    }

    // 현재 슬롯 위에 아이템이 드롭된 경우 
    public void OnDrop(DraggableObject<T> draggedElement)
    {
        if (draggedElement != null && (draggedElement.element.GetType() == typeof(T)))
        {
            SwapDraggableElements(draggedElement, this.DraggableElement);
        }
        else
        {
            Debug.Assert(draggedElement != null, "dragged Element null!");
            Debug.Assert(draggedElement.GetType() == typeof(T), $"{draggedElement.GetType()} != {typeof(T)}");
        }

    }

    public  void SwapDraggableElements(DraggableObject<T> newItem, DraggableObject<T> oldItem)
    {

        // TODO : 더 부드럽게 전환될 수 있지만, 로직이 복잡하여 실제 에셋 적용 후에 판단한다

/*        oldItem.transform.SetParent(newItem.parentTransform); // 부모 오브젝트 교체
        oldItem.parentTransform = newItem.transform.parent; // 부모 오브젝트 변수 재할당
        oldItem.slot = newItem.slot; // 연결된 슬롯 교체
        oldItem.slot.DraggableObject = oldItem; // 슬롯에 연결된 DraggableObject 교체

        newItem.transform.SetParent(transform); 
        newItem.parentTransform = transform.parent;
        newItem.slot = this;
        this.DraggableObject = newItem;*/

        AfterSwapElement(oldItem.slot, newItem.slot);
    }

    public virtual void  AfterSwapElement(Slot<T> slot1, Slot<T> slot2) { }

}
