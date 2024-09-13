
using UnityEngine;

public class PlaySound : MonoBehaviour
{

    [Header("Controller")]
    [SerializeField] private EnemieController enemieController;


    //играем звук удара
    public void PlayHitSound()
    {
        enemieController.PlayHitSound();
    }
}
