
using UnityEngine;

public class PlaySound : MonoBehaviour
{

    [Header("Controller")]
    [SerializeField] private EnemieController enemieController;


    //������ ���� �����
    public void PlayHitSound()
    {
        enemieController.PlayHitSound();
    }
}
