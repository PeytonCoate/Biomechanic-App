using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    [SerializeField]
    private Page InitialPage;
    [SerializeField]
    private GameObject FirstFocusItem;
    [SerializeField]
    private Canvas RootCanvas;

    private Stack<Page> PageStack = new Stack<Page>();

    private void Awake()
    {
        RootCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        if (FirstFocusItem != null)
        {
            EventSystem.current.SetSelectedGameObject(FirstFocusItem);
        }

        if (InitialPage != null)
        {
            PushPage(InitialPage);
        }
    }

    public void Back(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (RootCanvas.enabled && RootCanvas.gameObject.activeInHierarchy)
        {
            if (PageStack.Count != 0)
            {
                PopSpecialPage();
            }
        }
    }
    /*private void OnSpace()
    {
        Debug.Log("hello");
    }
    */
    public bool IsPageInStack(Page Page)
    {
        return PageStack.Contains(Page);
    }

    public bool IsPageOnTopOfStack(Page Page)
    {
        return PageStack.Count > 0 && Page == PageStack.Peek();
    }

    public void PushPage(Page Page)
    {
        Page.Enter(true);

        if (PageStack.Count > 0)
        {
            Page currentPage = PageStack.Peek();

            if (currentPage.ExitOnNewPagePush)
            {
                currentPage.Exit(false);
            }
        }

        PageStack.Push(Page);
    }

    public void PopPage()
    {
        if (PageStack.Peek().isSpecial || PageStack.Peek().isPrimary)
        {
            Debug.LogWarning("SpecialPages and primary pages can only be popped with their respecive function calls!");
            return;
        }
        if (PageStack.Count > 1)
        {
            Page page = PageStack.Pop();
            page.Exit(true);

            Page newCurrentPage = PageStack.Peek();
            if (newCurrentPage.ExitOnNewPagePush)
            {
                newCurrentPage.Enter(false);
            }
        }
        else
        {
            Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
        }
    }

    public void PopSpecialPage()//used to pop base UI elements that should not be popped otherwise. Used to "change scenes."
    {
        if (PageStack.Peek().isPrimary)
        {
            Debug.LogWarning("PrimaryPage can only be popped with its respective function call!");
            return;
        }
        if (PageStack.Count > 1)
        {
            Page page = PageStack.Pop();
            page.Exit(true);

            Page newCurrentPage = PageStack.Peek();
            if (newCurrentPage.ExitOnNewPagePush)
            {
                newCurrentPage.Enter(false);
            }
        }
        else
        {
            Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
        }
    }

    public void PopPrimaryPage()//used to pop base UI elements that should not be popped otherwise. Used to "change scenes."
    {
        if (!PageStack.Peek().isPrimary)
        {
            Debug.LogWarning("Only the Primary page can be popped with this function!");
            return;
        }
        if (PageStack.Count > 1)
        {
            Page page = PageStack.Pop();
            page.Exit(true);

            Page newCurrentPage = PageStack.Peek();
            if (newCurrentPage.ExitOnNewPagePush)
            {
                newCurrentPage.Enter(false);
            }
        }
        else
        {
            Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
        }
    }

    public void PopAllPages()
    {
        for (int i = 1; i < PageStack.Count; i++)
        {
            PopPage();
        }
    }
}
