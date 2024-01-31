using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public List<Carte> Pioche = new List<Carte>();
    public PlaceDeck PlacePioche;
    public Joueur J1;
    public Joueur J2;
    public Joueur JoueurActif;
    protected Joueur JoueurPassif;
    public int Tour { get; set; }
    public float PMEnCours;
    public List<PlaceDeck> SlotsCartes = new List<PlaceDeck>();
    public List<CarteSettings> CartesMontrable = new List<CarteSettings>();
    public GrosseCarte grosseCarte;
    public bool CarteMontreeEnCours = false;
    public bool EstEnTrainDeChoisirStratège = false;
    public Sprite ImageDosCarte;
    public Text ActionEnCoursTexte;
    public Text PMEnCoursTexte;

    //public List<BoutonCache> BoutonsDerriere = new List<BoutonCache>();

    public GameObject FondColore;

    void Start()
    {
        //Demander les infos des joueurs
        //Demander quelles familles ils veulent jouer
        //NouvellePartie(); //Normalement on aura des choix donc il changera de place
        PMEnCoursTexte.text = "0".ToString();
    }

    public void NouvellePartie() // On récupèrera les joueurs et familles
    {
        JoueurActif = J1; //Ce sera une création de joueur à la place
        JoueurPassif = J2;
        Tour = -1;
        foreach (PlaceDeck slot in JoueurActif.Deck) //On cache le deck qu'on a
        {
            slot.gameObject.SetActive(true);
        }
        foreach (PlaceDeck slot in JoueurPassif.Deck) //On cache le deck qu'on a
        {
            slot.gameObject.SetActive(false);
        }
        SlotsCartes = JoueurActif.Deck;

        foreach (Carte carte in Pioche) //Ca donne les endroits du deck pour le jeu
        {
            carte.PlaceOccupee = PlacePioche;
            carte.Appartenance = null;
        }
        //Virer toutes les cartes des decks
        // Mettre toutes les cartes choisies dans
        /*
        foreach(carte in carteSettings TOTALE)
        if(famille == famille choisie)
        {
            CartesMontrables.Add()
        }
        foreach(carte in carte TOTALE)
        if(famille == famille choisie)
        {
            Pioche.Add()
        }
        */

        // La, on pioche les cartes du tout début de partie
        for (int i = 0; i < 4; i++)
        {
            Piocher();
            JoueurActif.APioche = false;
        }
        ChangerDeTour();
        for (int i = 0; i < 4; i++)
        {
            Piocher();
            JoueurActif.APioche = false;
        }
        ChangerDeTour();

    }

    public void Piocher()
    {
        if (Pioche.Count >= 1 && JoueurActif.APioche == false)
        {
            Carte randCarte = Pioche[Random.Range(0, Pioche.Count)];

            foreach (PlaceDeck slot in SlotsCartes)
            {
                if (slot.availableCarteSlots == true)
                {
                    randCarte.gameObject.SetActive(true);
                    randCarte.rectTransform.anchoredPosition = slot.rectTransform.anchoredPosition;
                    slot.availableCarteSlots = false;
                    slot.CartePlacee = randCarte;
                    randCarte.PositionBase = slot.rectTransform.anchoredPosition;
                    randCarte.PlaceOccupee = slot;
                    randCarte.Appartenance = JoueurActif;
                    Pioche.Remove(randCarte);
                    JoueurActif.CartesPossedees.Add(randCarte);
                    JoueurActif.APioche = true;
                    return;
                }
            }
        }
    }

    public void ChangerDeTour()
    {

        Debug.Log("On change de tour : " + Tour);
        Debug.Log(JoueurPassif);
        JoueurActif.APioche = false;
        Tour++;

        foreach (PlaceDeck slot in JoueurActif.Deck) //On cache le deck qu'on a
        {
            slot.gameObject.SetActive(false);
            if (slot.CartePlacee != null) { slot.CartePlacee.gameObject.SetActive(false); }
        }

        foreach (PlaceDeck slot in JoueurPassif.Deck) // On montre le deck de l'autre joueur
        {
            slot.gameObject.SetActive(true);
            if (slot.CartePlacee != null) { slot.CartePlacee.gameObject.SetActive(true); }
        }

        EchangerTerrainCoteCacherCartes();

        //Retirer la carte montrée
        if (CarteMontreeEnCours == true)
        {
            grosseCarte.SeDecaler(grosseCarte.Carte, false);
            CarteMontreeEnCours = false;
        }

        if (Tour % 2 == 0)
        {
            JoueurActif = J2;
            JoueurPassif = J1;
        }
        else
        {
            JoueurActif = J1;
            JoueurPassif = J2;
        }

        SlotsCartes = JoueurActif.Deck;

        if (Tour == 1 || Tour == 2) { PMEnCours = 3; StartCoroutine(ChoixDeStratege()); }
        else if (Tour > 2) { PMEnCours = JoueurActif.Terrain[0].CartePlacee.PM; AfficherPM(); } // POUR L'instant !! Après le stratège bouge mais on verra ca plus tard

    }

    IEnumerator ChoixDeStratege() //Me permet d'arrêter l'action tant qu'il n'a pas choisit de stratège INCROYABLE
    {
        FondColore.gameObject.transform.Translate(0, -585, 0f);
        ActionEnCoursTexte.text = "Choisissez un stratège, glissez le dans la case du centre".ToString();
        yield return new WaitUntil(() => JoueurActif.Terrain[0].CartePlacee != null);
        FondColore.gameObject.transform.Translate(0, 585, 0f);
        JoueurActif.Terrain[0].CartePlacee.Stratege = true;
        AfficherPM();
    }

    public void AfficherPM()
    {
        PMEnCoursTexte.text = PMEnCours.ToString() + " / " + JoueurActif.Terrain[0].CartePlacee.PM.ToString();
    }

    public void EchangerTerrainCoteCacherCartes()
    {
        //Le gros pavé pour échanger les terrains de côtés et cacher les cartes cachées.

        JoueurActif.Terrain[0].gameObject.transform.Translate(0, 660, 0f);
        if (JoueurActif.Terrain[0].CartePlacee != null) { JoueurActif.Terrain[0].CartePlacee.gameObject.transform.Translate(0, 660, 0f); }
        JoueurPassif.Terrain[0].gameObject.transform.Translate(0, -660, 0f);
        if (JoueurPassif.Terrain[0].CartePlacee != null) { JoueurPassif.Terrain[0].CartePlacee.gameObject.transform.Translate(0, -660, 0f); }
        for (int i = 1; i <= 2; i++)
        {
            if (JoueurActif.Terrain[i].CartePlacee != null)
            {
                JoueurActif.Terrain[i].CartePlacee.gameObject.transform.Translate(0, 460, 0f);
                if (JoueurActif.Terrain[i].CartePlacee.EstCachee == true)
                {
                    JoueurActif.Terrain[i].CartePlacee.GetComponent<UnityEngine.UI.Image>().sprite = ImageDosCarte;
                }
            }
            JoueurActif.Terrain[i].gameObject.transform.Translate(0, 460, 0f);
        }
        for (int i = 1; i <= 2; i++)
        {
            if (JoueurPassif.Terrain[i].CartePlacee != null)
            {
                JoueurPassif.Terrain[i].CartePlacee.gameObject.transform.Translate(0, -460, 0f);
                if (JoueurPassif.Terrain[i].CartePlacee.EstCachee == true)
                {
                    JoueurPassif.Terrain[i].CartePlacee.GetComponent<UnityEngine.UI.Image>().sprite = JoueurPassif.Terrain[i].CartePlacee.ImageOriginale;
                }
            }
            JoueurPassif.Terrain[i].gameObject.transform.Translate(0, -460, 0f);
        }
    }
}
