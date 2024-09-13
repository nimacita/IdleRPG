using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoinPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Components")]
    [SerializeField] private GameObject coinValuePanel;
    [SerializeField] private GameObject sellIconPanel;
    [SerializeField] private GameObject darcerBg;

    //bools
    [SerializeField]
    private bool isSelected = false;
    [SerializeField]
    private bool isItemTake = false;

    void Start()
    {
        ItemDrop();
    }

    //взяли предмет
    public void ItemTake()
    {
        isItemTake = true;
        SellSpriteUpdate();
        sellIconPanel.SetActive(true);
    }

    //положили предмет
    public void ItemDrop()
    {
        isItemTake = false;
        sellIconPanel.SetActive(false);
    }

    //обновляем спрайт панели продать
    private void SellSpriteUpdate()
    {
        if (isItemTake) 
        {
            if (isSelected)
            {
                darcerBg.SetActive(false);
            }
            else
            {
                darcerBg.SetActive(true);
            }
        }
    }

    public void SelectItem()
    {
        if (isItemTake) 
        {
            isSelected = true;
            SellSpriteUpdate();
        }
    }

    public void DeSelectItem()
    {
        isSelected = false;
        SellSpriteUpdate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.touchSupported)
        {
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                SelectItem();
            }
            else
            {
                DeSelectItem();
            }
        }
        else
        {
            DeSelectItem();
        }
    }

    public bool IsSelected
    {
        get { return isSelected; }
    }
}
