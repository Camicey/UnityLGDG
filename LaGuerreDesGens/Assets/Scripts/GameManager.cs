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
    public bool EstEnTrainDePouvoir = false;
    public Sprite ImageDosCarte;
    public Text WarningTexte;
    public Text ActionEnCoursTexte;
    public Text PMEnCoursTexte;
    public List<BoutonChoix> BoutonsDeJeu = new List<BoutonChoix>();
    public List<Carte> Defausse = new List<Carte>();
    public List<Carte> ToutesLesCartes = new List<Carte>();
    public List<string> FamillesChoisies = new List<string>();
    public string stringToEdit = "Hello \nI've got 2 lines...";


    public GameObject FondColore;

    void Start()
    {
        //Demander les infos des joueurs
        //Demander quelles familles ils veulent jouer
        //NouvellePartie(); //Normalement on aura des choix donc il changera de place
        //OnGUI();
        FamillesChoisies.Add("Rose");
        FamillesChoisies.Add("Bleue");
        PMEnCoursTexte.text = "0".ToString();
        Warning("Bienvenue sur le jeu !");
    }
    void OnGUI()
    {
        //stringToEdit = GUI.TextArea(new Rect(500, 200, 1000, 200), stringToEdit, 200);
        //stringToEdit.FontSize = "52";
    }
    public void NouvellePartie() // On récupèrera les joueurs et familles
    {
        JoueurActif = J1; //Ce sera une création de joueur à la place
        JoueurPassif = J2;
        Tour = -1;
        CarteCiblee = null;
        TerrainCiblee = null;
        CarteMontree = null;
        EstEnTrainDAttaquer = false;
        EstEnTrainDeSeDeplacer = false;
        EstEnTrainDePouvoir = false;
        SlotsCartes = JoueurActif.Deck;
        Pioche.Clear();
        Defausse.Clear();

        //Tous les start()
        JoueurActif.Start();
        JoueurPassif.Start();
        foreach (PlaceDeck slot in JoueurActif.Deck)
        {
            slot.gameObject.SetActive(true);
            slot.Start();
        }
        foreach (PlaceDeck slot in JoueurPassif.Deck)
        {
            slot.gameObject.SetActive(false);
            slot.Start();
        }
        foreach (PlaceTerrain slot in JoueurActif.Terrain) { slot.Start(); }
        foreach (PlaceTerrain slot in JoueurPassif.Terrain) { slot.Start(); }
        foreach (Carte carte in ToutesLesCartes) // Mettre les cartes dans la pioche
        { carte.Initialiser(); }

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
        foreach (Carte carte in ToutesLesCartes)
        { carte.AUtilisePouvoir = false; }
        EchangeDeTerrain(JoueurActif, 1);
        EchangeDeTerrain(JoueurPassif, -1);

        if (CarteMontree != null) { CacherGrandeCarte(); } //Retirer la carte montrée

        if (Tour % 2 == 0) { JoueurActif = J2; JoueurPassif = J1; } //Changement de joueur
        else { JoueurActif = J1; JoueurPassif = J2; }

        SlotsCartes = JoueurActif.Deck;
        EnleverBonBoutons();
        if (Tour == 1 || Tour == 2) { PMEnCours = 3; StartCoroutine(ChoixDeStratege()); }
        else if (Tour > 2 && JoueurActif.TrouverStratege() != null) { PMEnCours = JoueurActif.TrouverStratege().Stats.PM; } // POUR L'instant !! Après le stratège bouge mais on verra ca plus tard
        else if (Tour > 2 && JoueurActif.TrouverStratege() == null) { PMEnCours = 2; Piocher(); StartCoroutine(ChoixDeStratege()); }
        AfficherPM();
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
        if (JoueurActif.TrouverStratege() != null)
        { PMEnCoursTexte.text = PMEnCours.ToString() + " / " + JoueurActif.TrouverStratege().Stats.PM.ToString(); }
        else { PMEnCoursTexte.text = "Stratege".ToString(); }
    }

    public void Attaquer()
    {
        if (PMEnCours >= 1)
        {
            EstEnTrainDAttaquer = true;
            //Ecran derrière, possibilité de tapper. Tant qu'on ne choisit pas
            FondColore.gameObject.transform.Translate(0, -50, 0f);
            ActionEnCoursTexte.text = "Choisissez une cible, une personne à attaquer".ToString();
            EnleverBonBoutons();
            if (CarteMontree.Appartenance == JoueurActif) { StartCoroutine(AttendreCiblee()); }
            else
            { FondColore.gameObject.transform.Translate(0, 50, 0f); }
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
        else if (CarteMontree.Liens.Contains(CarteCiblee))
        {
            Warning(CarteMontree.Stats.Prenom + " apperçoit " + CarteCiblee.Stats.Prenom + " et refuse de se battre.");
            PMEnCours--;
            CarteCiblee.GetComponent<UnityEngine.UI.Image>().sprite = CarteCiblee.ImageOriginale;
            CarteCiblee.EstCachee = false;
            CarteMontree.EstCachee = false;
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
                    if (JoueurActif.TrouverStratege() == null) { PMEnCours = 1; StartCoroutine(ChoixDeStratege()); }
                    return;
                }
                else { Warning("Pas de place dans le deck."); }
            }
        }
        else { Warning("Pas assez de PM pour retirer de carte."); }
    }
    public void Pouvoir()
    {
        if (PMEnCours - CarteMontree.Stats.CoutPouvoir >= 0 && CarteMontree.AUtilisePouvoir == false)
        {

            EstEnTrainDePouvoir = true;
            switch (CarteMontree.Stats.IdPouvoir)
            {
                case 0:
                    break;
                case 1:
                    //ChoixDeGens
                    //ChangerPouvoir(0, Cible: Carte);
                    break;
                case 2:
                    //ChoixDeGens
                    //Cible("PV", 10, false);
                    break;
                case 3:
                    //ChoixDeGens
                    //Cible("PV", 20, false);
                    break;
                case 4:
                    //ChoixDeGens
                    //Cible(“PV”, -10, False);
                    //Imobilise(1);
                    break;
                case 5:
                    //ChoixDeGens
                    //CouperLien(Cible: Carte);
                    break;
                case 6:
                    //ChoixDeGens
                    //CreerLien(Cible1, Cible2);
                    break;
                case 7:
                    //SeDeplacer(Case Départ, CaseArrivee);
                    break;
                case 8: //Echanger Ne marche pas
                    EstEnTrainDAttaquer = true;
                    FondColore.gameObject.transform.Translate(0, -50, 0f);
                    ActionEnCoursTexte.text = "Choisissez avec qui vous voulez échanger votre carte".ToString();
                    EnleverBonBoutons();
                    StartCoroutine(AttendreCibleeEchange());
                    break;
                case 9:
                    //ChoixDeGens
                    CarteMontree.Mourir();
                    //CarteCiblee.Mourir();
                    break;/*
                case 10: // Retourner toutes les cartes
                    foreach (PlaceTerrain terrain in JoueurActif.Terrain)
                    { if (terrain.CartePlacee != null) { terrain.CartePlacee.Retourner(); } }
                    foreach (PlaceTerrain terrain in JoueurPassif.Terrain)
                    { if (terrain.CartePlacee != null) { terrain.CartePlacee.Retourner(); } }
                    break;
                case 11: // Retourner Famille
                    //Retourner(Famille);
                    break;
                case 12: //Retourner Piege
                    foreach (PlaceTerrain terrain in JoueurActif.Terrain)
                    { if (terrain.CartePlacee != null && terrain.CartePlacee.Stats.Type == "Piege") { terrain.CartePlacee.Retourner(); } }
                    foreach (PlaceTerrain terrain in JoueurPassif.Terrain)
                    { if (terrain.CartePlacee != null && terrain.CartePlacee.Stats.Type == "Piege") { terrain.CartePlacee.Retourner(); } }
                    break;*/
                case 13:
                    //RevenirPioche();
                    break;
                default:
                    break;
            }
            if (CarteMontree.Stats.IdPouvoir != 8)
            {
                PMEnCours = PMEnCours - CarteMontree.Stats.CoutPouvoir;
                CarteMontree.AUtilisePouvoir = true;
            }
            else { }

            EstEnTrainDePouvoir = false;
            AfficherPM();
        }
        else { Warning("Pas assez de PM pour utiliser ce pouvoir."); }
    }

    public void Echanger()
    {
        if (PMEnCours >= 2)
        {
            EstEnTrainDAttaquer = true;
            FondColore.gameObject.transform.Translate(0, -50, 0f);
            ActionEnCoursTexte.text = "Choisissez avec qui vous voulez échanger votre carte".ToString();
            EnleverBonBoutons();
            StartCoroutine(AttendreCibleeEchange());
        }
        else { Warning("Pas assez de PM pour échanger les cartes."); }
    }

    IEnumerator AttendreCibleeEchange()
    {
        Warning("Choisissez avec qui " + CarteMontree.Stats.Prenom + " va échanger de place.");
        yield return new WaitUntil(() => CarteCiblee != null);
        FondColore.gameObject.transform.Translate(0, 50, 0f);
        if (CarteCiblee.Appartenance != JoueurActif) // Vérifier que la carte est à côtée // Vérifier que la carte n'est pas liée
        { Warning("Cette carte ne vous appartient pas !"); }
        else if (!VerifierTerrainACote(CarteCiblee.PlaceDeTerrain))
        { Warning("Cette carte est trop loin !"); }
        else if (CarteCiblee.Stratege == true)
        { Warning("On échange pas le stratège de place !"); }
        else
        {
            PMEnCours -= 2; // Pour le pouvoir de Edwin
            if (CarteMontree.Stats.IdPouvoir == 8 && CarteMontree.AUtilisePouvoir == false)
            { PMEnCours++; CarteMontree.AUtilisePouvoir = true; }
            Warning(CarteMontree.Stats.Prenom + " a échanger de place avec " + CarteCiblee.Stats.Prenom);

            TerrainCiblee = CarteMontree.PlaceDeTerrain;

            CarteMontree.PlaceDeTerrain.CartePlacee = CarteCiblee;
            CarteCiblee.PlaceDeTerrain.CartePlacee = CarteMontree;

            CarteMontree.PlaceDeTerrain = CarteCiblee.PlaceDeTerrain;
            CarteCiblee.PlaceDeTerrain = TerrainCiblee;

            CarteMontree.GetComponent<RectTransform>().anchoredPosition = CarteMontree.PlaceDeTerrain.GetComponent<RectTransform>().anchoredPosition;
            CarteCiblee.GetComponent<RectTransform>().anchoredPosition = TerrainCiblee.GetComponent<RectTransform>().anchoredPosition;

            Warning(CarteMontree.Stats.Prenom + " et " + CarteCiblee.Stats.Prenom + " ont échangés de place.");
            AfficherPM();
        }
        CarteCiblee = null;
        TerrainCiblee = null;
        EstEnTrainDAttaquer = false;
        CacherGrandeCarte();
        AfficherPM();
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
    public void MontrerBoutonsChoix(string situation)
    {
        switch (situation)
        {
            case "Allie":
                BoutonsDeJeu[0].Montrer(); //Attaquer
                BoutonsDeJeu[1].Montrer(); //Deplacer
                BoutonsDeJeu[2].Montrer(); //Retirer
                BoutonsDeJeu[3].Montrer(); //Pouvoir
                BoutonsDeJeu[5].Montrer(); //Echanger
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
    public void CacherGrandeCarte()
    {
        grosseCarte.SeDecaler(CarteMontree.Stats, false);
        EnleverBonBoutons();
        CarteMontree.EstMontreeSurCarte = false;
        CarteMontree = null;
    }
    public void ConditionDeVictoire()
    {
        if (Pioche.Count == 0 && JoueurActif.CartesPossedees.Count == 0)
        { Warning("Le jeu est terminé. " + JoueurPassif.Prenom + " gagne avec " + JoueurPassif.CartesPossedees.Count + " carte(s) restantes."); }
        else if (Pioche.Count == 0 && JoueurPassif.CartesPossedees.Count == 0)
        { Warning("Le jeu est terminé. " + JoueurActif.Prenom + " gagne avec " + JoueurActif.CartesPossedees.Count + " carte(s) restantes."); }
    }
}
