using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public List<Carte> Pioche = new List<Carte>();
    public Joueur J1;
    public Joueur J2;
    public Joueur JoueurActif;
    protected Joueur JoueurPassif;
    public int Tour { get; set; }
    public float PMEnCours;
    public List<PlaceDeck> SlotsCartes = new List<PlaceDeck>();
    public List<CarteSettings> CartesMontrable = new List<CarteSettings>();
    public GrosseCarte grosseCarte;
    public Carte CarteMontree;
    public Carte CarteCiblee; //Carte ciblée dans une attaque
    public PlaceTerrain TerrainCiblee;
    public GameObject ToutTerrain;
    public bool EstEnTrainDAttaquer = false;
    public bool EstEnTrainDeSeDeplacer = false;
    public Sprite ImageDosCarte;
    public Text WarningTexte;
    public Text ActionEnCoursTexte;
    public Text PMEnCoursTexte;
    public List<BoutonChoix> BoutonsDeJeu = new List<BoutonChoix>();

    public GameObject FondColore;

    void Start()
    {
        //Demander les infos des joueurs
        //Demander quelles familles ils veulent jouer
        //NouvellePartie(); //Normalement on aura des choix donc il changera de place
        PMEnCoursTexte.text = "0".ToString();
        Warning("Bienvenue sur le jeu !");
        //System.Threading.Thread.Sleep(200);
    }

    public void NouvellePartie() // On récupèrera les joueurs et familles
    {
        JoueurActif = J1; //Ce sera une création de joueur à la place
        JoueurPassif = J2;
        Tour = -1;
        CarteCiblee = null;
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
            carte.PlaceDeDeck = null;
            carte.PlaceDeTerrain = null;
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
        PMEnCours = 4;
        for (int i = 0; i < 4; i++)
        {
            Piocher();
            JoueurActif.APioche = false;

        }
        ChangerDeTour();
        PMEnCours = 4;
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
            if (PMEnCours >= 1)
            {
                if (EstEnTrainDAttaquer == false && EstEnTrainDeSeDeplacer == false)
                {
                    Carte randCarte = Pioche[Random.Range(0, Pioche.Count)];
                    foreach (PlaceDeck slot in SlotsCartes)
                    {
                        if (slot.availableCarteSlots == true)
                        {
                            PMEnCours--;
                            AfficherPM();
                            randCarte.gameObject.SetActive(true);
                            randCarte.rectTransform.anchoredPosition = slot.rectTransform.anchoredPosition;
                            slot.availableCarteSlots = false;
                            slot.CartePlacee = randCarte;
                            randCarte.PositionBase = slot.rectTransform.anchoredPosition;
                            randCarte.PlaceDeDeck = slot;
                            randCarte.Appartenance = JoueurActif;
                            Pioche.Remove(randCarte);
                            JoueurActif.CartesPossedees.Add(randCarte);
                            JoueurActif.APioche = true;
                            return;
                        }
                    }
                }
                else { Warning("Finissez votre action avant de piocher."); }
            }
            else { Warning("Vous n'avez pas assez de PM pour piocher."); }
        }
        else { Warning("Vous ne pouvez piocher qu'une fois par tour."); }
    }
    public void ChangerDeTour()
    {

        Warning("On change de tour : " + Tour.ToString() + ". C'est au tour de : " + JoueurPassif.Prenom);
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

        EchangeDeTerrain(JoueurActif, 1);
        EchangeDeTerrain(JoueurPassif, -1);

        if (CarteMontree != null) { CacherGrandeCarte(); } //Retirer la carte montrée

        if (Tour % 2 == 0) { JoueurActif = J2; JoueurPassif = J1; } //Changement de joueur
        else { JoueurActif = J1; JoueurPassif = J2; }

        SlotsCartes = JoueurActif.Deck;
        EnleverBonBoutons();
        if (Tour == 1 || Tour == 2) { PMEnCours = 3; StartCoroutine(ChoixDeStratege()); }
        else if (Tour > 2 && JoueurActif.TrouverStratege() != null) { PMEnCours = JoueurActif.TrouverStratege().Stats.PM; } // POUR L'instant !! Après le stratège bouge mais on verra ca plus tard
        else { PMEnCours = 1; }
        AfficherPM();
    }
    IEnumerator ChoixDeStratege() //Me permet d'arrêter l'action tant qu'il n'a pas choisit de stratège INCROYABLE
    {
        FondColore.gameObject.transform.Translate(0, -585, 0f);
        ActionEnCoursTexte.text = "Choisissez un stratège, glissez le dans la case du centre".ToString();
        yield return new WaitUntil(() => JoueurActif.Terrain[0].CartePlacee != null);
        FondColore.gameObject.transform.Translate(0, 585, 0f);
        JoueurActif.Terrain[0].CartePlacee.Stratege = true;
    }
    public bool StrategeSeul()
    {
        if (JoueurActif.CartesPossedees.Count == 1 && Pioche.Count == 0)
        { return true; }
        return false;
    }
    public void Warning(string texteDonne)
    { WarningTexte.text = texteDonne.ToString(); }

    public void AfficherPM()
    {
        if (JoueurActif.Terrain[0].CartePlacee != null)
        { PMEnCoursTexte.text = PMEnCours.ToString() + " / " + JoueurActif.Terrain[0].CartePlacee.Stats.PM.ToString(); }
        else { PMEnCoursTexte.text = "Stratege".ToString(); }
    }

    public void Attaquer()
    {
        if (PMEnCours >= 1)
        {
            EstEnTrainDAttaquer = true;
            //Qui voulez vous attaquer ? + Ecran derrière, possibilité de tapper. Tant qu'on ne choisit pas
            FondColore.gameObject.transform.Translate(0, -50, 0f);
            ActionEnCoursTexte.text = "Choisissez une cible, une personne à attaquer".ToString();
            EnleverBonBoutons();
            if (CarteMontree.Appartenance == JoueurActif) { StartCoroutine(AttendreCiblee()); }
            /*else
            {StartCoroutine(AttendreCiblee()); }*/
        }
        else { Warning("Pas assez de PM pour attaquer"); }
    }

    IEnumerator AttendreCiblee()
    {
        Warning("Choisissez avec qui " + CarteMontree.Stats.Prenom + " va se battre.");
        yield return new WaitUntil(() => CarteCiblee != null);
        FondColore.gameObject.transform.Translate(0, 50, 0f);
        if (CarteCiblee.Appartenance == JoueurActif) // Vérifier que la carte est à côtée // Vérifier que la carte n'est pas liée
        {
            Warning("Cette carte vous appartient !");
        }
        else if (!VerifierTerrainACote(CarteCiblee.PlaceDeTerrain))
        {
            Warning("Cette carte est trop loin !");
        }
        else
        {
            PMEnCours--;
            CarteCiblee.GetComponent<UnityEngine.UI.Image>().sprite = CarteCiblee.ImageOriginale;
            CarteCiblee.EstCachee = false;
            CarteMontree.EstCachee = false;
            CarteCiblee.Stats.PVar = CarteCiblee.Stats.PVar - CarteMontree.Stats.PA;
            Warning(CarteMontree.Stats.Prenom + " a infligé " + CarteMontree.Stats.PA.ToString() + " a " + CarteCiblee.Stats.Prenom);
            if (CarteCiblee.Stats.PVar > 0)
            {
                CarteMontree.Stats.PVar = CarteMontree.Stats.PVar - CarteCiblee.Stats.PA;
                Warning(CarteCiblee.Stats.Prenom + " est vivant(e) et a infligé " + CarteCiblee.Stats.PA.ToString() + " en retour.");
                if (CarteMontree.Stats.PVar <= 0)
                {
                    CarteMontree.Mourir();
                    Warning(CarteMontree.Stats.Prenom + " est mort(e).");
                }
            }
            else
            {
                Warning("Vous avez tué " + CarteCiblee.Stats.Prenom + " avec " + CarteMontree.Stats.Prenom);
                CarteCiblee.Mourir();
            }
        }
        CarteCiblee = null;
        EstEnTrainDAttaquer = false;
        CacherGrandeCarte();
        AfficherPM();
        ConditionDeVictoire();
    }

    public void CacherGrandeCarte()
    {
        grosseCarte.SeDecaler(CarteMontree.Stats, false);
        EnleverBonBoutons();
        CarteMontree.EstMontreeSurCarte = false;
        CarteMontree = null;
    }

    public bool VerifierTerrainACote(PlaceTerrain terrain) // J'ai raccourci
    {
        if (CarteMontree.PlaceDeTerrain.Id == 0 && (terrain.Id == 1 || terrain.Id == 2)) { return true; }
        else if (CarteMontree.PlaceDeTerrain.Id == 1 && (terrain.Id == 0 || terrain.Id == 2 || terrain.Id == 4)) { return true; }
        else if (CarteMontree.PlaceDeTerrain.Id == 2 && (terrain.Id == 0 || terrain.Id == 1 || terrain.Id == 3)) { return true; }
        else if (CarteMontree.PlaceDeTerrain.Id == 3 && (terrain.Id == 5 || terrain.Id == 2 || terrain.Id == 4)) { return true; }
        else if (CarteMontree.PlaceDeTerrain.Id == 4 && (terrain.Id == 5 || terrain.Id == 1 || terrain.Id == 3)) { return true; }
        else if (CarteMontree.PlaceDeTerrain.Id == 5 && (terrain.Id == 3 || terrain.Id == 4)) { return true; }
        return false;
    }

    public void SeDeplacer()
    {
        if (PMEnCours >= 1)
        {
            EstEnTrainDeSeDeplacer = true;
            FondColore.gameObject.transform.Translate(0, -50, 0f);
            ActionEnCoursTexte.text = "Choisissez où vous déplacer".ToString();
            EnleverBonBoutons();
            StartCoroutine(AttendreTerrainCiblee());
        }
        else { Warning("Pas assez de PM pour se déplacer."); }
    }
    IEnumerator AttendreTerrainCiblee()
    {
        ToutTerrain.transform.SetAsLastSibling();
        yield return new WaitUntil(() => TerrainCiblee != null);
        if (!VerifierTerrainACote(TerrainCiblee)) { Warning("Ce terrain est trop loin !"); }
        else if (TerrainCiblee.CartePlacee != null)
        { Warning("Il y a déjà quelqu'un ici !"); }
        else
        {
            CarteMontree.PlaceDeTerrain.CartePlacee = null;
            CarteMontree.PlaceDeTerrain = TerrainCiblee;
            TerrainCiblee.CartePlacee = CarteMontree;
            CarteMontree.GetComponent<RectTransform>().anchoredPosition = TerrainCiblee.GetComponent<RectTransform>().anchoredPosition;
            PMEnCours--;
            Warning(CarteMontree.Stats.Prenom + " s'est déplacé(e).");
            AfficherPM();
        }
        ToutTerrain.transform.SetAsFirstSibling();
        FondColore.gameObject.transform.Translate(0, 50, 0f);
        EstEnTrainDeSeDeplacer = false;
        TerrainCiblee = null;
        CacherGrandeCarte();
    }
    public void Retirer()
    {
        if (PMEnCours >= 1)
        {
            foreach (PlaceDeck slot in SlotsCartes)
            {
                if (slot.availableCarteSlots == true)
                {
                    CarteMontree.gameObject.SetActive(true);
                    CarteMontree.rectTransform.anchoredPosition = slot.rectTransform.anchoredPosition;
                    slot.availableCarteSlots = false;
                    CarteMontree.PlaceDeDeck = slot;
                    slot.CartePlacee = CarteMontree;
                    CarteMontree.PlaceDeTerrain.CartePlacee = null;
                    CarteMontree.PlaceDeTerrain = null;
                    CarteMontree.PositionBase = slot.rectTransform.anchoredPosition;
                    CarteMontree.EstCachee = false;
                    CarteMontree.EstEnJeu = false;
                    CarteMontree.Stratege = false;
                    PMEnCours--;
                    CacherGrandeCarte();
                    AfficherPM();
                    return;
                }
                else { Warning("Pas de place dans le deck."); }
            }
        }
        else { Warning("Pas assez de PM pour retirer de carte."); }
    }
    public void Pouvoir()
    {

    }

    public void MontrerBoutonsChoix(string situation) //Finie
    {
        switch (situation)
        {
            case "Allie":
                BoutonsDeJeu[0].Montrer(); //Attaquer
                BoutonsDeJeu[1].Montrer(); //Deplacer
                BoutonsDeJeu[2].Montrer(); //Retirer
                BoutonsDeJeu[3].Montrer(); //Pouvoir
                break;
            case "StrategeAllie":
                BoutonsDeJeu[0].Montrer();
                BoutonsDeJeu[4].Montrer(); //Retirer pour stratege
                BoutonsDeJeu[3].Montrer();
                break;
            case "Ennemi":
                BoutonsDeJeu[0].Montrer(); //Mauvaise attaque attention
                break;
            case "StrategeEnnemi":
                BoutonsDeJeu[0].Montrer(); //Mauvaise attaque attention
                break;
            case "StrategeSeul":
                BoutonsDeJeu[0].Montrer();
                BoutonsDeJeu[1].Montrer();
                break;
            default:
                break;
        }

    }
    public void EnleverBonBoutons()
    {
        foreach (BoutonChoix bouton in BoutonsDeJeu)
        {
            if (bouton.EstMontree) { bouton.Cacher(); }
        }
    }
    public void EchangeDeTerrain(Joueur J1, int n)
    {
        Carte carte;
        foreach (PlaceTerrain terrain in J1.Terrain)
        {
            if (terrain.CartePlacee != null)
            {
                carte = terrain.CartePlacee;
                if (terrain == J1.Terrain[0]) { carte.gameObject.transform.Translate(0, n * 660, 0f); } //Bouger la carte
                else { carte.gameObject.transform.Translate(0, n * 460, 0f); }

                if (carte.EstCachee == true && carte.Appartenance == JoueurActif)
                { carte.GetComponent<UnityEngine.UI.Image>().sprite = ImageDosCarte; } //Cacher carte
                else if ((carte.EstCachee == true && carte.Appartenance == JoueurPassif) || carte.Stratege == true)
                { carte.GetComponent<UnityEngine.UI.Image>().sprite = carte.ImageOriginale; } //Montrer carte
            }
            if (terrain == J1.Terrain[0]) { terrain.gameObject.transform.Translate(0, n * 660, 0f); }
            else { terrain.gameObject.transform.Translate(0, n * 460, 0f); }
        }
    }
    public void ConditionDeVictoire()
    {
        if (Pioche.Count == 0 && JoueurActif.CartesPossedees.Count == 0)
        { Warning("Le jeu est terminé. " + JoueurPassif.Prenom + " gagne avec " + JoueurPassif.CartesPossedees.Count + " carte(s) restantes."); }
        else if (Pioche.Count == 0 && JoueurPassif.CartesPossedees.Count == 0)
        { Warning("Le jeu est terminé. " + JoueurActif.Prenom + " gagne avec " + JoueurActif.CartesPossedees.Count + " carte(s) restantes."); }
    }
}
