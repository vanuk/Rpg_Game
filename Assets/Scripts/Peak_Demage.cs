using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peak_Demage : MonoBehaviour
{
    // Скільки демаги наносить пік
    [SerializeField] private int _peakDamage = 50; // Діапазон дії піку
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Animation Event: викликається на 3-му кадрі анімації піку
    public void DoPeakDamage()
    {
        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, GetComponent<Collider2D>().bounds.size, 0f, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            Move_Soldier playerScript = playerCollider.GetComponent<Move_Soldier>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(_peakDamage);
            }
        }
    }
}
