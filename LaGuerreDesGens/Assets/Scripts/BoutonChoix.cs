using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoutonChoix : MonoBehaviour
{
    public string Utilite;
    public GameManager JeuEnCours;
    public bool EstMontree = false;

    void Start()
    { this.gameObject.SetActive(false); }

    public void Montrer()
    {
        this.gameObject.SetActive(true);
        EstMontree = true;
    }
    public void Cacher()
    {
        this.gameObject.SetActive(false);
        EstMontree = false;
    }
}