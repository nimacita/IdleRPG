using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocationBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Components")]
    [SerializeField] private TMPro.TMP_Text locName;
    [SerializeField] private GameObject bgMask;
    [SerializeField] private TMPro.TMP_Text recomendedLevelTxt;
    [SerializeField] private ParticleSystem locParticles;

    [Header("Spawner Stats")]
    [SerializeField] private SpawnerStats spawnerStats;
    [SerializeField] private int locInd;
    [SerializeField] private int recomendedLevel = 0;

    [Header("Location Controller")]
    [SerializeField] private LocationController locationController;
    [SerializeField] private PlayerCurrentStats currentStats;

    private bool isSelected = false;


    void Start()
    {
        UpdateBtnVisual();

        locName.text = spawnerStats.spawnerLocations[locInd].locationName;

        gameObject.GetComponent<Button>().onClick.AddListener(BtnClick);
    }

    //обновл€ем вид кнопки
    public void UpdateBtnVisual()
    {
        bgMask.SetActive(isSelected);

        //устанавливаем текст рекомендуемого уровн€
        recomendedLevelTxt.text = $"–екомендуемый ”ровень: {recomendedLevel}";
        recomendedLevelTxt.gameObject.SetActive(IsRecomended());

        //если выбрана эта локаци€ включаем частицы
        if (locationController.GetCurrLoc == locInd)
        {
            locParticles.Play();
            locParticles.gameObject.SetActive(true);
        }
        else
        {
            locParticles.Stop();
            locParticles.gameObject.SetActive(false);
        }
    }

    //нажали на кнопку
    private void BtnClick()
    {
        isSelected = false;
        locationController.UpdateLocation(locInd);
    }

    //показываем ли рекомендуемый текст
    private bool IsRecomended()
    {
        //если выбрано
        if (isSelected)
        {
            //если уровень, меньше рекомендуемого
            if (currentStats.Level < recomendedLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void SelectItem()
    {
        isSelected = true;
        UpdateBtnVisual();
    }

    public void DeSelectItem()
    {
        isSelected = false;
        UpdateBtnVisual();
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

}
