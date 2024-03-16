using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantierCartes : MonoBehaviour
{
    public GameManager JeuEnCours;
    public GameObject PrefabCarte;
    public GameObject PrefabTerrain;
    public GameObject PrefabDeck;
    public GameObject DossierCarte;
    public GameObject DossierDeck;
    public Canvas Canvas;

    // Start is called before the first frame update
    void Start()
    {
        foreach (CarteSettings CarteS in JeuEnCours.CartesMontrable) //On instancie les cartes
        {
            GameObject Derniere = Instantiate(PrefabCarte, new Vector3(-500, -500, 0), Quaternion.identity, DossierCarte.transform);
            Derniere.GetComponent<Carte>().Stats = CarteS;
            Derniere.GetComponent<Carte>().JeuEnCours = JeuEnCours;
            Derniere.GetComponent<Carte>().canvas = Canvas;
            Derniere.GetComponent<Carte>().Commencer();
        }

        JeuEnCours.ToutTerrain.Clear();
        for (int i = 0; i <= 10; i++) //On instancie les terrains
        {
            GameObject Derniere = Instantiate(PrefabTerrain, new Vector3(-100, -200, 0), Quaternion.identity, JeuEnCours.DossierTerrain.transform);
            Derniere.GetComponent<PlaceTerrain>().Id = i;
            Derniere.GetComponent<PlaceTerrain>().JeuEnCours = JeuEnCours;
            if (i <= 2 || i == 6 || i == 7)
            {
                Derniere.GetComponent<PlaceTerrain>().Appartenance = JeuEnCours.J1;
                JeuEnCours.J1.Terrain.Add(Derniere.GetComponent<PlaceTerrain>());
            }
            else if (i != 10)
            {
                Derniere.GetComponent<PlaceTerrain>().Appartenance = JeuEnCours.J2;
                JeuEnCours.J2.Terrain.Add(Derniere.GetComponent<PlaceTerrain>());
            }
            JeuEnCours.ToutTerrain.Add(Derniere.GetComponent<PlaceTerrain>());
        }

        for (int i = 0; i < 10; i++) //On instancie les decks
        {
            GameObject Derniere = Instantiate(PrefabDeck, new Vector3(-200, -200, 0), Quaternion.identity, DossierDeck.transform);
            Derniere.GetComponent<PlaceDeck>().Id = i;
            Derniere.GetComponent<PlaceDeck>().JeuEnCours = JeuEnCours;
            if (i < 5)
            {
                Derniere.GetComponent<PlaceDeck>().Appartenance = JeuEnCours.J1;
                JeuEnCours.J1.Deck.Add(Derniere.GetComponent<PlaceDeck>());
            }
            else
            {
                Derniere.GetComponent<PlaceDeck>().Appartenance = JeuEnCours.J2;
                JeuEnCours.J2.Deck.Add(Derniere.GetComponent<PlaceDeck>());
            }
        }
    }

}
