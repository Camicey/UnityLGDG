using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public class Carte : NetworkBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] public Canvas canvas;
    private CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    public int Id;
    public CarteSettings Stats;
    public bool EstCachee = false; //Est cachée à l'adversaire
    public bool EstEnJeu = false; //Est sur le plateau
    public bool Stratege = false; //Est un stratège
    public bool AUtilisePouvoir = false; //La carte ne peut utiliser son pouvoir qu'une fois par tour
    public int Immobile = 0; //La carte ne peut pas agir pendant ce nombre de tour
    public Joueur Appartenance; //Joueur auquel appartient la carte
    public Vector2 PositionBase; //Position sur laquelle la carte revient si elle est posée sur un endroit non autorisé
    public GameManager JeuEnCours;
    public PlaceDeck PlaceDeDeck;
    public PlaceTerrain PlaceDeTerrain;

    //Tous les paramètres de chaque carte.

    public Text PrenomT;

    public Image ImageT;

    public Text PMT;
    public Text PVT;
    public Text PAT;

    public Text PouvoirT;
    public Text CoutPouvoirT;
    public Image FamilleImageT;
    public Image TypeImageT;
    public Text LiensT;


    public void Montrer() //Pour faire apparaître les informations sur la carte
    {
        PrenomT.text = Stats.Prenom;
        ImageT.sprite = Stats.Image;
        ImageT.enabled = true;
        PMT.text = Stats.PM.ToString();
        PVT.text = Stats.PVar.ToString();
        PAT.text = Stats.PA.ToString();
        PouvoirT.text = Stats.PouvoirVar;
        CoutPouvoirT.text = Stats.CoutPouvoirVar.ToString() + "PM";
        FamilleImageT.sprite = Stats.FamilleImage;
        TypeImageT.sprite = Stats.TypeImage;
        TypeImageT.enabled = true;
        LiensT.text = MontrerLiens();
    }
    public string MontrerLiens() //Afficher les liens sur la carte
    {
        string description = " ";
        foreach (CarteSettings lien in Stats.liensVar)
        { description += lien.Prenom + "\n"; }
        if (description == " ") { description = "Personne"; }
        return description;
    }

    public void Commencer() //N'est fait qu'une fois au début
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        PositionBase = rectTransform.anchoredPosition;
        JeuEnCours.ToutesLesCartes.Add(this);
    }
    public void Initialiser()
    {
        Id = Stats.Id;
        Stats.PVar = Stats.PV;
        Stats.PouvoirVar = Stats.Pouvoir;
        Stats.IdPouvoirVar = Stats.IdPouvoir;
        Stats.CoutPouvoirVar = Stats.CoutPouvoir;
        Stats.liensVar.Clear();
        foreach (CarteSettings lien in Stats.liens)
        { Stats.liensVar.Add(lien); }
        PlaceDeDeck = null;
        PlaceDeTerrain = null;
        Appartenance = null;
        EstCachee = false;
        EstEnJeu = false;
        Stratege = false;
        Immobile = 0;
        AUtilisePouvoir = false;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(-1200, 0);
        if (Stats.Famille == JeuEnCours.FamillesChoisies[0] || Stats.Famille == JeuEnCours.FamillesChoisies[1])
        { JeuEnCours.Pioche.Add(this); } //Mettre les cartes des familles choisies dans la pioche
        Montrer();
    }
    public void Mourir()
    {
        if (Stats.IdPouvoir == 13 && JeuEnCours.Pioche.Count != 0)
        {
            Initialiser();
            JeuEnCours.Warning(Stats.Prenom + " est retourné dans la pioche.");
        }
        else
        {
            JeuEnCours.Warning(Stats.Prenom + " est mort(e).");
            Appartenance.CartesPossedees.Remove(this); // On enlève la carte du joueur a qui il appartient 
            Appartenance = null;
            gameObject.transform.Translate(0, 1500, 0f);
            PlaceDeTerrain.CartePlacee = null;
            PlaceDeTerrain = null;
        }
    }

    public void Agrandir() // Agrandir la carte. 
    {
        if (EstEnJeu == false)
        {
            gameObject.transform.Translate(86, 176, 1.5f);
            gameObject.transform.localScale = Vector3.one * 1.3f;
            rectTransform.SetAsLastSibling(); // Met la carte devant les autres
        }
    }
    public void Retrecir() //Rétrécir la carte. 
    {
        if (EstEnJeu == false)
        {
            gameObject.transform.Translate(-86, -176, 0f);
            gameObject.transform.localScale = Vector3.one * 1f;
        }
    }

    public void CacherCarte()
    {
        PrenomT.text = " ";
        ImageT.enabled = false;
        PMT.text = " ";
        PVT.text = " ";
        PAT.text = " ";
        PouvoirT.text = " ";
        CoutPouvoirT.text = " ";
        FamilleImageT.sprite = JeuEnCours.ImageDosCarte;
        TypeImageT.enabled = false;
        LiensT.text = " ";
    }
    public void RetournerCarte()
    {
        Montrer();
        EstCachee = false;
    }

    public void MettreBonBoutons()
    {
        if (JeuEnCours.JoueurActif.CartesPossedees.Contains(this))
        {
            if (Stratege == true && !JeuEnCours.StrategeSeul()) { JeuEnCours.MontrerBoutonsChoix("StrategeAllie"); }
            else if (Stratege == true && JeuEnCours.StrategeSeul()) { JeuEnCours.MontrerBoutonsChoix("StrategeSeul"); }
            else { JeuEnCours.MontrerBoutonsChoix("Allie"); }
        }
        else // Cette partie n'est pas utile maintenant mais le sera pour la suite du jeu
        {
            if (Stratege == true) { JeuEnCours.MontrerBoutonsChoix("StrategeEnnemi"); }
            else { JeuEnCours.MontrerBoutonsChoix("Ennemi"); }
        }
    }

    //Tout en dessous c'est pour déplacer la carte, ce sont des fonctions implémentées par Unity de base

    public void OnBeginDrag(PointerEventData eventData) //Quand je commence à déplacer la carte
    {
        if (GetComponent<Carte>().EstEnJeu == false)
        {
            canvasGroup.alpha = .7f; // Opacité de la carte quand je clique dessus
            canvasGroup.blocksRaycasts = false;
        }
    }
    public void OnDrag(PointerEventData eventData) //Quand je déplace la carte
    {
        if (GetComponent<Carte>().EstEnJeu == false)
        { rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; }
    }
    public void OnEndDrag(PointerEventData eventData) //Quand je finis de déplacer la carte
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (GetComponent<Carte>().EstEnJeu == false)
        {
            rectTransform.anchoredPosition = PositionBase;
        }
        else if (PlaceDeDeck != null)
        {
            PlaceDeDeck.availableCarteSlots = true;
            PlaceDeDeck.CartePlacee = null;
            PlaceDeDeck = null;
        }
    }
    public void OnPointerDown(PointerEventData eventData) //Quand je clique sur la carte
    {
        if (JeuEnCours.EnTrainCibleCarte == true && EstEnJeu == true) { JeuEnCours.CarteCiblee = this; }
        else if (EstEnJeu == true && (EstCachee == false || JeuEnCours.JoueurActif == Appartenance)) //Si elle est montré ou qu'elle nous appartient
        {
            Montrer();
            if (JeuEnCours.CarteMontree == null) // Je montre une carte
            { JeuEnCours.MontrerGrandeCarte(this); }
            else if (Id == JeuEnCours.grosseCarte.Carte.Id) // Je cache la carte
            { JeuEnCours.CacherGrandeCarte(); }
            else // J'échange de carte
            {
                JeuEnCours.grosseCarte.SeDecaler(JeuEnCours.CarteMontree.Stats, false);
                JeuEnCours.EnleverBonBoutons();
                JeuEnCours.MontrerGrandeCarte(this);
            }
        }
        else if (EstCachee == true)
        { JeuEnCours.Warning("Vous ne pouvez pas voir cette carte."); }
    }
}