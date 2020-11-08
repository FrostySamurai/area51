using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _leftPointer;
    [SerializeField] private RectTransform _rightPointer;
    [SerializeField] private List<RectTransform> _menuItems;

    [SerializeField] private float _arrowOffset;
    [SerializeField] private float _animationPeriod;
    [SerializeField] private float _animationSpeed;

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
        if (AppData.InputController.VerticalMovement == 0) _releaseButton = true;
        if (_releaseButton && AppData.InputController.VerticalMovement < 0) { _selectedItemIndex++; _releaseButton = false; _reDrawArrows = true; }
        if (_releaseButton && AppData.InputController.VerticalMovement > 0) { _selectedItemIndex--; _releaseButton = false; _reDrawArrows = true; }
        
        _selectedItemIndex = _selectedItemIndex < 0 ? _menuItems.Count - 1 : _selectedItemIndex > _menuItems.Count - 1 ? 0 : _selectedItemIndex;

        if (_reDrawArrows) SetArrowPositon();
        SimpleAnim(_leftPointer);
        SimpleAnim(_rightPointer, -1);
    }

    void SetArrowPositon() 
    {
        _selectedItem = _menuItems[_selectedItemIndex];

        _leftPointer.transform.position = _selectedItem.transform.position - Vector3.right * _arrowOffset;
        _rightPointer.transform.position = _selectedItem.transform.position + Vector3.right * _arrowOffset;
        _reDrawArrows = false;
    }

    private void Select()
    {
        _selectedItem.GetComponent<Button>().onClick.Invoke();
    }

    void SimpleAnim(RectTransform transform, int orientation = 1) 
    {
        float value = Mathf.Sin(Time.time * _animationPeriod) * orientation;
        Vector2 anim = new Vector2(transform.position.x + value * _animationSpeed, transform.position.y);
        transform.position = anim;
    }
}

