using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _menuItems;


    private RectTransform _selectedItem = null;
    private int _selectedItemIndex = 0;
    private bool _releaseButton = true;
    private bool _reDrawArrows = true;

    private void Start()
    {
        AppData.InputController.OnInteractionPressed += Select;
    }

    private void OnDestroy()
    {
        if (AppData.InputController != null)
            AppData.InputController.OnInteractionPressed -= Select;
    }

    public void StartGame()
    {
        AppData.GameManager.StartGame();
    }

    public void Quit()
    {
        AppData.GameManager.QuitApplication();
    }

    private void Update()
    {
        if (AppData.InputController.HorizontalMovement == 0) _releaseButton = true;
        if (_releaseButton && AppData.InputController.HorizontalMovement < 0) { _selectedItemIndex++; _releaseButton = false; _reDrawArrows = true; }
        if (_releaseButton && AppData.InputController.HorizontalMovement > 0) { _selectedItemIndex--; _releaseButton = false; _reDrawArrows = true; }

        _selectedItemIndex = _selectedItemIndex < 0 ? _menuItems.Count - 1 : _selectedItemIndex > _menuItems.Count - 1 ? 0 : _selectedItemIndex;
        _selectedItem = _menuItems[_selectedItemIndex];
        ScaleAnimation(_selectedItem.GetComponent<RectTransform>());

        for (int i= 0; i< _menuItems.Count; i++)
        {
            if (_selectedItemIndex != i) 
            {
                _menuItems[i].sizeDelta = new Vector2(150, 150);
            }
        }
    }

    private void Select()
    {
        _selectedItem.GetComponent<Button>().onClick.Invoke();
    }

    void ScaleAnimation(RectTransform rect) 
    {
        float value = Mathf.Sin(Time.time * 10f);
        Vector2 anim = new Vector2(rect.sizeDelta.x + value, rect.sizeDelta.y + value);
        rect.sizeDelta = anim;
    }
}

