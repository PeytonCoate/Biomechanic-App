using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetScroll : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private ScrollRect scrollRect;
    
    public void SetScrollArea()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1;
    }

    public void SetScrollBar()
    {
        scrollbar.value = 1;
    }
}
