using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public Joueur J1;
    public Joueur J2;
    public Joueur JoueurActif;
    public Joueur JoueurPassif;
    public List<string> FamillesChoisies = new List<string>();
    public List<Carte> ToutesLesCartes = new List<Carte>(); //Toutes les cartes s'y trouvent
    public List<Carte> Pioche = new List<Carte>();
    public List<Carte> Defausse = new List<Carte>();
    public List<CarteSettings> CartesMontrable = new List<CarteSettings>(); //Toutes les cartes settings s'y trouvent
    public List<PlaceTerrain> ToutTerrain = new List<PlaceTerrain>();
    public List<BoutonChoix> BoutonsDeJeu = new List<BoutonChoix>();
    public GrosseCarte grosseCarte;
    public Carte CarteMontree;
    public Carte CarteCiblee; //Carte ciblée dans une attaque
    public int DegatCiblee; //Degat infligé pour la carte ciblée
    public PlaceTerrain TerrainCiblee;
    public GameObject DossierTerrain;
    public GameObject FondColore;
    public Image Selection;
    public Sprite ImageDosCarte;
    public int Tour { get; protected set; }
    public float PMEnCours;
    public Text PMEnCoursTexte;
    public Text WarningTexte; //Texte en haut à gauche qui décrit les actions des joueurs
    public Text ActionEnCoursTexte;
    public string TypePartie;
    public bool EnTrainCibleCarte;
    public bool EnTrainCibleTerrain;
    public JoueurManager JoueurManager;

    void Start()
    {
        Pioche.Clear();
        TypePartie = "";
    }
    public void NouvellePartie() // On récupèrera les joueurs et familles
    {
        if (JoueurActif == J2) { ChangerDeTour(); }
        JoueurActif = J1; // Ce sera une création de joueur à la place
        JoueurPassif = J2;
        Tour = -1;
        CarteCiblee = null;
        TerrainCiblee = null;
        if (CarteMontree != null) { CacherGrandeCarte(); }
        EnTrainCibleCarte = false;
        EnTrainCibleTerrain = false;
        Pioche.Clear();
        Defausse.Clear();

        //Tous les Start()
        JoueurActif.Start();
        JoueurPassif.Start();

        //Tous les Initialiser()
        foreach (PlaceTerrain slot in ToutTerrain) { slot.Initialiser(); }
        foreach (Carte carte in ToutesLesCartes) { carte.Initialiser(); } // Mettre les cartes dans la pioche
        if (CarteMontree != null) { CacherGrandeCarte(); }
        foreach (PlaceDeck slot in JoueurActif.Deck)
        {
            slot.gameObject.SetActive(true);
            slot.Initialiser();
        }
        foreach (PlaceDeck slot in JoueurPassif.Deck)
        {
            slot.gameObject.SetActive(false);
            slot.Initialiser();
        }

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

    public void Piocher() // Piocher une carte
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        JoueurManager = networkIdentity.GetComponent<JoueurManager>();

        if (Pioche.Count >= 1 && JoueurActif.APioche == false)
        {
            if (PMEnCours >= 1)
            {
                if (EnTrainCibleCarte == false && EnTrainCibleTerrain == false)
                {
                    Carte randCarte = Pioche[Random.Range(0, Pioche.Count)];
                    foreach (PlaceDeck slot in JoueurActif.Deck)
                    {
                        if (slot.availableCarteSlots == true)
                        {
                            PMEnCours--;
                            AfficherPM();
                            randCarte.gameObject.SetActive(true); // On fait apparaitre la carte
                            randCarte.rectTransform.anchoredPosition = slot.rectTransform.anchoredPosition; // On la place sur un slot disponible
                            slot.availableCarteSlots = false;
                            slot.CartePlacee = randCarte;
                            randCarte.PlaceDeDeck = slot; // Le slot de la carte est ce slot
                            randCarte.PositionBase = slot.rectTransform.anchoredPosition;
                            randCarte.Appartenance = JoueurActif; // La carte appartient au joueur qui l'a pioché
                            JoueurActif.CartesPossedees.Add(randCarte);
                            Pioche.Remove(randCarte); // On enlève la carte de la pioche
                            JoueurActif.APioche = true;
                            JoueurManager.CmdPiocher(randCarte.GetComponent<GameObject>());
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
        if (ConditionDeVictoire() == true) { SceneManager.LoadScene("Menu"); } // On fait quitter le jeu s'il est terminé
        Warning("On change de tour : " + Tour.ToString() + ". C'est au tour de : " + JoueurPassif.Prenom);
        JoueurActif.APioche = false;
        Tour++;
        if (CarteMontree != null) { CacherGrandeCarte(); }

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
        {
            carte.AUtilisePouvoir = false;
            if (carte.Immobile > 0) { carte.Immobile--; } // C'est pour le pouvoir 4
        }
        EchangeDeTerrain(JoueurActif);
        EchangeDeTerrain(JoueurPassif);

        if (Tour % 2 == 0) { JoueurActif = J2; JoueurPassif = J1; } // Changement de joueur
        else { JoueurActif = J1; JoueurPassif = J2; }
        EnleverBonBoutons();
        if (Tour == 1 || Tour == 2) { PMEnCours = 3; StartCoroutine(ChoixDeStratege()); }
        else if (Tour > 2 && JoueurActif.TrouverStratege() != null) { PMEnCours = JoueurActif.TrouverStratege().Stats.PM; } // POUR L'instant !! Après le stratège bouge mais on verra ca plus tard
        else if (Tour > 2 && JoueurActif.TrouverStratege() == null) { PMEnCours = 4; Piocher(); StartCoroutine(ChoixDeStratege()); }
        AfficherPM();
    }
    IEnumerator ChoixDeStratege() // Me permet d'arrêter l'action tant qu'il n'a pas choisit de stratège
    {
        FondColore.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 320);
        ActionEnCoursTexte.text = "Choisissez un stratège, glissez le dans la case du centre".ToString();
        yield return new WaitUntil(() => JoueurActif.Terrain[0].CartePlacee != null);
        FondColore.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 800);
        JoueurActif.Terrain[0].CartePlacee.Stratege = true;
        AfficherPM();
    }
    public bool StrategeSeul()
    {
        if (JoueurActif.CartesPossedees.Count == 1 && Pioche.Count == 0)
        { return true; }
        return false;
    }
    public void Warning(string texteDonne) { WarningTexte.text = texteDonne.ToString(); }
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
            EnTrainCibleCarte = true;
            EnleverBonBoutons();
            if (CarteMontree.Appartenance == JoueurActif) { StartCoroutine(AttendreCiblee()); }
        }
        else { Warning("Pas assez de PM pour attaquer"); }
    }
    IEnumerator AttendreCiblee()
    {
        Warning("Choisissez avec qui " + CarteMontree.Stats.Prenom + " va se battre.");
        yield return new WaitUntil(() => CarteCiblee != null);
        if (CarteCiblee.Appartenance == JoueurActif)
        { Warning("Cette carte vous appartient !"); }
        else if (!VerifierTerrainACote(CarteCiblee.PlaceDeTerrain.Id))
        { Warning("Cette carte est trop loin !"); }
        else if (CarteMontree.Stats.liensVar.Contains(CarteCiblee.Stats))
        {
            Warning(CarteMontree.Stats.Prenom + " apperçoit " + CarteCiblee.Stats.Prenom + " et refuse de se battre.");
            PMEnCours--;
            CarteCiblee.RetournerCarte();
            CarteMontree.RetournerCarte();
        }
        else
        {
            if (CarteCiblee.Stats.Type == "Robot")
            {
                CarteCiblee.Stats.PVar = CarteCiblee.Stats.PVar - 1;
                Warning(CarteMontree.Stats.Prenom + " a infligé 1 coup à " + CarteCiblee.Stats.Prenom);
            }
            else
            {
                CarteCiblee.Stats.PVar = CarteCiblee.Stats.PVar - CarteMontree.Stats.PA;
                Warning(CarteMontree.Stats.Prenom + " a infligé " + CarteMontree.Stats.PA.ToString() + " à " + CarteCiblee.Stats.Prenom);
            }
            if (CarteCiblee.Stats.PVar > 0)
            {
                if (CarteMontree.Stats.Type == "Robot")
                {
                    CarteMontree.Stats.PVar = CarteMontree.Stats.PVar - 1;
                    Warning(CarteCiblee.Stats.Prenom + " a infligé 1 coup à " + CarteMontree.Stats.Prenom);
                }
                else
                {
                    CarteMontree.Stats.PVar = CarteMontree.Stats.PVar - CarteCiblee.Stats.PA;
                    Warning(CarteCiblee.Stats.Prenom + " est vivant(e) a " + CarteCiblee.Stats.PVar + " PV et a infligé " + CarteCiblee.Stats.PA.ToString() + " en retour.");
                }
                if (CarteMontree.Stats.PVar <= 0)
                { CarteMontree.Mourir(); }
            }
            else { CarteCiblee.Mourir(); }
            PMEnCours--;
            CarteCiblee.RetournerCarte();
            CarteMontree.RetournerCarte();
        }
        CarteCiblee = null;
        EnTrainCibleCarte = false;
        CacherGrandeCarte();
        AfficherPM();
        ConditionDeVictoire();
    }
    public void SeDeplacer()
    {
        if (PMEnCours >= 1)
        {
            EnTrainCibleTerrain = true;
            EnleverBonBoutons();
            StartCoroutine(AttendreTerrainCiblee());
        }
        else { Warning("Pas assez de PM pour se déplacer."); }
    }
    IEnumerator AttendreTerrainCiblee()
    {
        DossierTerrain.transform.SetAsLastSibling();
        yield return new WaitUntil(() => TerrainCiblee != null);
        if (!VerifierTerrainACote(TerrainCiblee.Id)) { Warning("Ce terrain est trop loin !"); }
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
        DossierTerrain.transform.SetAsFirstSibling();
        EnTrainCibleTerrain = false;
        TerrainCiblee = null;
        CacherGrandeCarte();
    }
    public void Retirer() //On remet une carte dans son deck
    {
        if (PMEnCours >= 1)
        {
            foreach (PlaceDeck slot in JoueurActif.Deck)
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
                    CarteMontree.gameObject.transform.localScale = Vector3.one * 1f;
                    PMEnCours--;
                    CacherGrandeCarte();
                    AfficherPM();
                    if (JoueurActif.TrouverStratege() == null) { PMEnCours = 2; StartCoroutine(ChoixDeStratege()); }
                    return;
                }
                else { Warning("Pas de place dans le deck."); }
            }
        }
        else { Warning("Pas assez de PM pour retirer de carte."); }
    }
    public void Echanger()
    {
        if (PMEnCours >= 2)
        {
            EnTrainCibleCarte = true;
            EnleverBonBoutons();
            StartCoroutine(AttendreCibleeEchange());
        }
        else { Warning("Pas assez de PM pour échanger les cartes."); }
    }
    IEnumerator AttendreCibleeEchange() //Permet d'échanger deux cartes
    {
        Warning("Choisissez avec qui " + CarteMontree.Stats.Prenom + " va échanger de place.");
        yield return new WaitUntil(() => CarteCiblee != null);
        if (CarteCiblee.Appartenance != JoueurActif) // Vérifier que la carte est à côtée // Vérifier que la carte n'est pas liée
        { Warning("Cette carte ne vous appartient pas !"); }
        else if (!VerifierTerrainACote(CarteCiblee.PlaceDeTerrain.Id))
        { Warning("Cette carte est trop loin !"); }
        else if (CarteCiblee.Stratege == true)
        { Warning("On échange pas le stratège de place !"); }
        else
        {
            PMEnCours -= 2;
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
        EnTrainCibleCarte = false;
        CacherGrandeCarte();
        AfficherPM();
    }
    public bool VerifierTerrainACote(int terrain) // Vérifier que la carte est à côtée 
    {
        int IdMontree = CarteMontree.PlaceDeTerrain.Id;
        if (IdMontree == 0 && (terrain == 1 || terrain == 2 || ((terrain == 6 || terrain == 7) && StrategeSeul()))) { return true; }
        else if (IdMontree == 3 && (terrain == 5 || terrain == 4 || ((terrain == 8 || terrain == 9) && StrategeSeul()))) { return true; }
        else if (IdMontree == 1 && (terrain == 0 || (terrain == 2 && TypePartie != "Longue") || terrain == 4 || terrain == 6)) { return true; }
        else if (IdMontree == 2 && (terrain == 0 || (terrain == 1 && TypePartie != "Longue") || terrain == 5 || terrain == 7)) { return true; }
        else if (IdMontree == 5 && (terrain == 2 || (terrain == 4 && TypePartie != "Longue") || terrain == 3 || terrain == 9)) { return true; }
        else if (IdMontree == 4 && (terrain == 1 || (terrain == 5 && TypePartie != "Longue") || terrain == 3 || terrain == 8)) { return true; }
        //Terrains grandes parties
        else if (IdMontree == 6 && ((terrain == 0 && J1.Terrain[1].CartePlacee == null) || terrain == 1 || terrain == 7 || terrain == 8 || terrain == 10)) { return true; }
        else if (IdMontree == 7 && ((terrain == 0 && J1.Terrain[2].CartePlacee == null) || terrain == 2 || terrain == 6 || terrain == 9 || terrain == 10)) { return true; }
        else if (IdMontree == 8 && ((terrain == 3 && J1.Terrain[1].CartePlacee == null) || terrain == 4 || terrain == 6 || terrain == 9 || terrain == 10)) { return true; }
        else if (IdMontree == 9 && ((terrain == 3 && J1.Terrain[2].CartePlacee == null) || terrain == 5 || terrain == 7 || terrain == 8 || terrain == 10)) { return true; }
        else if (IdMontree == 10 && (terrain == 6 || terrain == 7 || terrain == 8 || terrain == 9)) { return true; }

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
                if (CarteMontree.AUtilisePouvoir == false)
                { BoutonsDeJeu[3].Montrer(); } //Pouvoir
                BoutonsDeJeu[5].Montrer(); //Echanger
                break;
            case "StrategeAllie":
                BoutonsDeJeu[0].Montrer(); //Attaquer
                BoutonsDeJeu[3].Montrer(); //Pouvoir
                BoutonsDeJeu[4].Montrer(); //Retirer pour stratege
                break;
            case "StrategeSeul":
                BoutonsDeJeu[0].Montrer(); //Attaquer
                BoutonsDeJeu[1].Montrer(); //Deplacer
                BoutonsDeJeu[3].Montrer(); //Pouvoir
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

    public void EchangeDeTerrain(Joueur J1) // On échange les terrains de côté 
    {
        Carte carte;
        foreach (PlaceTerrain terrain in J1.Terrain)
        {
            if (terrain.CartePlacee != null)
            {
                carte = terrain.CartePlacee;
                var posCarte = carte.gameObject.transform.position;
                posCarte.y = (posCarte.y - 540) * -1 + 540; // Incantation du démon pour passer de l'autre côté
                carte.gameObject.transform.position = posCarte; // On applique la transformation de la carte
                if (carte.EstCachee == true && carte.Appartenance == JoueurActif) // Si les cartes sont visibles
                { carte.CacherCarte(); }
                else if ((carte.EstCachee == true && carte.Appartenance == JoueurPassif) || carte.Stratege == true) // Si les cartes sont visibles
                { carte.Montrer(); }
            }
            var pos = terrain.gameObject.transform.position;
            pos.y = (pos.y - 540) * -1 + 540; // Transformation du terrain
            terrain.gameObject.transform.position = pos;
        }
    }

    public void CacherGrandeCarte()
    {
        grosseCarte.SeDecaler(CarteMontree.Stats, false); // On cache la carte
        EnleverBonBoutons();
        CarteMontree.Montrer();
        CarteMontree = null;
        Selection.rectTransform.anchoredPosition = new Vector2(-1200, 0); // On déplace la sélection
    }
    public void MontrerGrandeCarte(Carte carteMontree)
    {
        CarteMontree = carteMontree;
        grosseCarte.SeDecaler(CarteMontree.Stats, true); // Montrer la grande carte
        // Les deux premiers tours on ne laisse pas les joueurs agir et si la carte est immobilisée
        if (Tour > 2 && CarteMontree.Immobile == 0) { CarteMontree.MettreBonBoutons(); }
        if (TypePartie == "Longue") { Selection.rectTransform.sizeDelta = new Vector2(200, 290); } // Change la taille en fonction de la partie
        else { Selection.rectTransform.sizeDelta = new Vector2(265, 385); }
        Selection.rectTransform.anchoredPosition = CarteMontree.rectTransform.anchoredPosition; // Change sa position sur la carte montrée
        if (CarteMontree.Appartenance != JoueurActif) { Selection.color = Color.red; }
        else { Selection.color = Color.blue; } // Change la couleur en fonction d'ennemi (rouge) ou allié (bleu)
    }
    public bool ConditionDeVictoire() //Vérifie si le jeu est terminé
    {
        if (Pioche.Count == 0 && JoueurActif.CartesPossedees.Count == 0)
        {
            Warning("Le jeu est terminé. " + JoueurPassif.Prenom + " gagne avec " + JoueurPassif.CartesPossedees.Count + " carte(s) restantes.");
            return true;
        }
        else if (Pioche.Count == 0 && JoueurPassif.CartesPossedees.Count == 0)
        {
            Warning("Le jeu est terminé. " + JoueurActif.Prenom + " gagne avec " + JoueurActif.CartesPossedees.Count + " carte(s) restantes.");
            return true;
        }
        else if (Pioche.Count == 0 && VerifierEgalite())
        {
            Warning("Le jeu est terminé. Il y a égalité car " + JoueurPassif.CartesPossedees[0].Stats.Prenom + " et " + JoueurActif.CartesPossedees[0].Stats.Prenom + " ont un lien et refusent de se battre.");
            return true;
        }
        return false;
    }

    public bool VerifierEgalite()
    {
        foreach (Carte carte in JoueurPassif.CartesPossedees)
        {
            foreach (Carte carteLien in JoueurActif.CartesPossedees)
            {
                if (!carteLien.Stats.liensVar.Contains(carte.Stats))
                { return false; }
            }
        }
        foreach (Carte carte in JoueurActif.CartesPossedees)
        {
            foreach (Carte carteLien in JoueurPassif.CartesPossedees)
            {
                if (!carteLien.Stats.liensVar.Contains(carte.Stats))
                { return false; }
            }
        }
        return true;
    }

    public void Pouvoir()
    {
        if (CarteMontree.AUtilisePouvoir == true)
        { Warning("Ce personnage à déjà utilisé son pouvoir."); }
        else if (PMEnCours - CarteMontree.Stats.CoutPouvoirVar < 0)
        { Warning("Pas assez de PM pour utiliser ce pouvoir."); }
        else
        {
            switch (CarteMontree.Stats.IdPouvoirVar)
            {
                case 1:
                    StartCoroutine(ChangerPouvoir()); //ChoixDeGens
                    break;
                case 2: // Guérir 10
                    Cible("PV", 10, false);
                    break;
                case 3: // Guérir 20
                    Cible("PV", 20, false);
                    break;
                case 4: // Enlever 10 a lui même
                    StartCoroutine(Immobiliser());
                    break;
                case 5: // Couper tous les liens
                    StartCoroutine(CouperLiens());
                    break;
                case 6: // Créer deux liens
                    StartCoroutine(CreerLien());
                    break;
                case 7: //Téléporter
                    EnTrainCibleTerrain = true;
                    EnleverBonBoutons();
                    StartCoroutine(Teleporter());
                    break;
                case 8: // Echanger
                    CommencerPouvoir("Echanger à bas coût", true);
                    StartCoroutine(AttendreCibleeEchange());
                    break;
                case 9: // Défausse deux cartes
                    StartCoroutine(MourirCiblee());
                    break;
                case 10: // Retourner toutes les cartes j'ai essayé de raccourcir mais c'est difficile
                    foreach (PlaceTerrain terrain in JoueurActif.Terrain) // Je parcoure mon terrain
                    { if (terrain.CartePlacee != null) { terrain.CartePlacee.RetournerCarte(); } }
                    foreach (PlaceTerrain terrain in JoueurPassif.Terrain) // Je parcoure le terrain de l'ennemi
                    { if (terrain.CartePlacee != null) { terrain.CartePlacee.RetournerCarte(); } }
                    FinirPouvoir("Tout le monde a été découvert par " + CarteMontree.Stats.Prenom, false, true);
                    break;
                case 11: // Retourner la Famille Rose
                    foreach (PlaceTerrain terrain in JoueurActif.Terrain)
                    { if (terrain.CartePlacee != null && terrain.CartePlacee.Stats.Famille == "Rose") { terrain.CartePlacee.RetournerCarte(); } }
                    foreach (PlaceTerrain terrain in JoueurPassif.Terrain)
                    { if (terrain.CartePlacee != null && terrain.CartePlacee.Stats.Famille == "Rose") { terrain.CartePlacee.RetournerCarte(); } }
                    FinirPouvoir("Toutes les cartes roses ont été découvertes par " + CarteMontree.Stats.Prenom, false, true);
                    break;
                case 12: //Retourner Piege
                    foreach (PlaceTerrain terrain in JoueurActif.Terrain)
                    { if (terrain.CartePlacee != null && terrain.CartePlacee.Stats.Type == "Piege") { terrain.CartePlacee.RetournerCarte(); } }
                    foreach (PlaceTerrain terrain in JoueurPassif.Terrain)
                    { if (terrain.CartePlacee != null && terrain.CartePlacee.Stats.Type == "Piege") { terrain.CartePlacee.RetournerCarte(); } }
                    FinirPouvoir("Tout les pièges ont été découvert par " + CarteMontree.Stats.Prenom, false, true);
                    break;
                default:
                    break;
            }
        }
        ConditionDeVictoire();
    }
    //Tous les pouvoirs
    public IEnumerator CreerLien()
    {
        CommencerPouvoir("Choisissez avec qui va avoir un nouveau lien.", true);
        yield return new WaitUntil(() => CarteCiblee != null);
        Carte carteAttente = CarteCiblee;
        CarteCiblee.RetournerCarte();
        CarteCiblee = null;
        Warning("Choississez avec qui " + carteAttente.Stats.Prenom + " va avoir un lien.");
        yield return new WaitUntil(() => CarteCiblee != null);
        CarteCiblee.RetournerCarte();
        if (CarteCiblee.Id == carteAttente.Id)
        { FinirPouvoir("L'amour de soi est toujours important.", true, false); }
        else if (CarteCiblee.Stats.liensVar.Count < 4 && carteAttente.Stats.liensVar.Count < 4)
        {
            if (!CarteCiblee.Stats.liensVar.Contains(carteAttente.Stats)) { CarteCiblee.Stats.liensVar.Add(carteAttente.Stats); }
            if (!carteAttente.Stats.liensVar.Contains(CarteCiblee.Stats)) { carteAttente.Stats.liensVar.Add(CarteCiblee.Stats); }
            FinirPouvoir(CarteCiblee.Stats.Prenom + " a maintenant un lien avec " + carteAttente.Stats.Prenom, true, true);
        }
        else { FinirPouvoir("On ne peut pas avoir plus de 4 liens.", true, false); }
    }
    public IEnumerator Teleporter()
    {
        CommencerPouvoir("Choisissez avec qui va être immobilisé.", false);
        yield return new WaitUntil(() => TerrainCiblee != null);
        if (TerrainCiblee.CartePlacee != null)
        { FinirPouvoir("Il y a déjà quelqu'un ici !", false, false); }
        else
        {
            CarteMontree.PlaceDeTerrain.CartePlacee = null;
            CarteMontree.PlaceDeTerrain = TerrainCiblee;
            TerrainCiblee.CartePlacee = CarteMontree;
            CarteMontree.GetComponent<RectTransform>().anchoredPosition = TerrainCiblee.GetComponent<RectTransform>().anchoredPosition;
            FinirPouvoir(CarteMontree.Stats.Prenom + "s'est téléporté(e)", false, true);
        }

    }
    public IEnumerator Immobiliser()
    {
        CommencerPouvoir("Choisissez avec qui va être immobilisé.", true);
        yield return new WaitUntil(() => CarteCiblee != null);
        CarteMontree.Stats.PVar = CarteMontree.Stats.PVar - 10;
        if (CarteMontree.Stats.PVar <= 0) { CarteMontree.Mourir(); }
        CarteCiblee.Immobile = 2;
        FinirPouvoir(CarteCiblee.Stats.Prenom + " ne pourra pas agir au prochain tour.", true, true);
    }
    private void Cible(string p, int degat, bool surSoi)
    {
        DegatCiblee = degat; //Je donne la modification au GameManager, parce que les coroutines n'acceptent pas d'arguments
        if (p == "PV") { StartCoroutine(CiblePV()); }
    }
    public IEnumerator CiblePV()
    {
        CommencerPouvoir("Choisissez avec qui va avoir" + DegatCiblee + " PV.", true);
        yield return new WaitUntil(() => CarteCiblee != null);
        if (CarteCiblee.Stats.Type == "Robot")
        { FinirPouvoir("Il n'est pas possible de soigner les robots...", true, false); }
        else
        {
            CarteCiblee.Stats.PVar = CarteCiblee.Stats.PVar + DegatCiblee;
            if (CarteCiblee.Stats.PVar <= 0) { CarteCiblee.Mourir(); }
            else if (CarteCiblee.Stats.PVar > CarteCiblee.Stats.PV) { CarteCiblee.Stats.PVar = CarteCiblee.Stats.PV; }
            FinirPouvoir(CarteCiblee.Stats.Prenom + " a eu une modification de " + DegatCiblee + " PV.", true, true);
        }
    }
    public IEnumerator ChangerPouvoir()
    {
        CommencerPouvoir("Choisissez avec qui va perdre son pouvoir.", true);
        yield return new WaitUntil(() => CarteCiblee != null);
        CarteCiblee.Stats.PouvoirVar = "Plus de pouvoir.";
        CarteCiblee.Stats.IdPouvoirVar = 0;
        CarteCiblee.Stats.CoutPouvoirVar = 0;
        FinirPouvoir(CarteCiblee.Stats.Prenom + " a perdu son pouvoir.", true, true);
    }
    public IEnumerator MourirCiblee()
    {
        CommencerPouvoir("Choisissez avec qui va mourir avec " + CarteMontree.Stats.Prenom, true);
        yield return new WaitUntil(() => CarteCiblee != null);
        CarteCiblee.Mourir();
        CarteMontree.Mourir();
        FinirPouvoir(CarteCiblee.Stats.Prenom + " est mort avec " + CarteMontree.Stats.Prenom, true, true);
    }
    IEnumerator CouperLiens()
    {
        CommencerPouvoir("Choisissez qui va perdre tous ses liens par " + CarteMontree.Stats.Prenom, true);
        yield return new WaitUntil(() => CarteCiblee != null);
        CarteCiblee.Stats.liensVar.Clear();
        FinirPouvoir(CarteCiblee.Stats.Prenom + " a perdu tous ses liens.", true, true);
    }
    private void CommencerPouvoir(string texte, bool carteCiblee) //Utilisé à chaque début de pouvoir
    {
        if (carteCiblee == true) { EnTrainCibleCarte = true; }
        else { EnTrainCibleTerrain = true; DossierTerrain.transform.SetAsLastSibling(); }
        EnleverBonBoutons();
        Warning(texte);
    }
    private void FinirPouvoir(string texte, bool carteCiblee, bool enleverPM) //Utilisé à chaque fin de pouvoir
    {
        if (carteCiblee == true)
        {
            CarteCiblee.RetournerCarte();
            CarteMontree.RetournerCarte();
            CarteCiblee = null;
            EnTrainCibleCarte = false;
        }
        else if (TerrainCiblee != null)
        {
            TerrainCiblee = null;
            EnTrainCibleTerrain = false;
            DossierTerrain.transform.SetAsFirstSibling();
        }
        CarteMontree.AUtilisePouvoir = true;
        if (CarteMontree.Stats.IdPouvoir != 8 && enleverPM == true) { PMEnCours = PMEnCours - CarteMontree.Stats.CoutPouvoirVar; }
        Warning(texte);
        CacherGrandeCarte();
        AfficherPM();
    }
}