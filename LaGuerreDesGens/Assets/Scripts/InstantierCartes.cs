using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantierCartes : MonoBehaviour
{

    public GameManager JeuEnCours;
    public GameObject PrefabCarte;
    public Carte DernierePrefab;
    public GameObject Place;
    public Canvas Canvas;

    // Start is called before the first frame update
    void Start()
    {
        foreach (CarteSettings CarteS in JeuEnCours.CartesMontrable)
        {
            GameObject Derniere = Instantiate(PrefabCarte, new Vector3(-100, -100, 0), Quaternion.identity, Place.transform);
            Derniere.GetComponent<Carte>().Stats = CarteS;
            Derniere.GetComponent<Carte>().JeuEnCours = JeuEnCours;
            Derniere.GetComponent<Carte>().canvas = Canvas;
            Derniere.GetComponent<Carte>().Commencer();
        }
    }
}
