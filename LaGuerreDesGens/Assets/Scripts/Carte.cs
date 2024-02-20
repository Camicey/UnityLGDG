using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Carte : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    public int Id;
    public CarteSettings Stats;
    public List<Carte> Liens = new List<Carte>();
    public bool EstCachee = false; //Est cachée à l'adversaire
    public bool EstMontreeSurCarte = false; //Est montrée sur la grande carte
    public bool EstEnJeu = false; //Est sur le plateau
    public bool Stratege = false;
    public bool AUtilisePouvoir = false;
    public int Immobile = 0;

    public Joueur Appartenance;
    public Vector2 PositionBase;
    public GameManager JeuEnCours;
    public PlaceDeck PlaceDeDeck;
    public PlaceTerrain PlaceDeTerrain;
    public Sprite ImageOriginale;


    // Start is called before the first frame update
    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        PositionBase = rectTransform.anchoredPosition;
        Stats.PVar = Stats.PV;
        JeuEnCours.ToutesLesCartes.Add(this);
    }

    public void Initialiser()
    {
        Stats.PVar = Stats.PV;
        Stats.PouvoirVar = Stats.Pouvoir;
        Stats.IdPouvoirVar = Stats.IdPouvoir;
        Stats.CoutPouvoirVar = Stats.CoutPouvoir;
        Stats.liensvar.Clear();
        foreach (CarteSettings lien in Stats.liens)
        {
            Stats.liensvar.Add(lien);
        }
        PlaceDeDeck = null;
        PlaceDeTerrain = null;
        Appartenance = null;
        EstMontreeSurCarte = false;
        EstCachee = false;
        EstEnJeu = false;
        Stratege = false;
        Immobile = 0;
        AUtilisePouvoir = false;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(-1200, 0);
        if (Stats.Famille == JeuEnCours.FamillesChoisies[0] || Stats.Famille == JeuEnCours.FamillesChoisies[1])
        { JeuEnCours.Pioche.Add(this); }
    }

    public void Agrandir() // Agrandir la carte.
    {
        if (EstEnJeu == false)
        {
            rectTransform.sizeDelta = new Vector2(780, 1170);
            this.gameObject.transform.Translate(90, 180, 1.5f);
            rectTransform.SetAsLastSibling(); // Met la carte devant les autres ouiiiii
        }
    }

    public void Retrecir()
    {
        if (EstEnJeu == false)
        {
            rectTransform.sizeDelta = new Vector2(600, 900);
            this.gameObject.transform.Translate(-90, -180, 0f);
        }
    }

    public void MettreBonBoutons()
    {
        if (JeuEnCours.JoueurActif.CartesPossedees.Contains(this))
        {
            if (Stratege == true && !JeuEnCours.StrategeSeul()) { JeuEnCours.MontrerBoutonsChoix("StrategeAllie"); }
            else if (Stratege == true && JeuEnCours.StrategeSeul()) { JeuEnCours.MontrerBoutonsChoix("StrategeSeul"); }
            else { JeuEnCours.MontrerBoutonsChoix("Allie"); }
        }
        else
        {
            if (Stratege == true) { JeuEnCours.MontrerBoutonsChoix("StrategeEnnemi"); }
            else { JeuEnCours.MontrerBoutonsChoix("Ennemi"); }
        }
    }

    public void Mourir()
    {
        if (Stats.IdPouvoir == 13 && JeuEnCours.Pioche.Count != 0)
        {
            EstEnJeu = false;
            JeuEnCours.Warning(Stats.Prenom + " est retourné dans la pioche.");
            JeuEnCours.Pioche.Add(this);
        }
        else
        {
            EstEnJeu = true;
            JeuEnCours.Warning(Stats.Prenom + " est mort(e).");
        }
        Appartenance.CartesPossedees.Remove(this);
        Appartenance = null;
        this.gameObject.transform.Translate(0, 1500, 0f);
        PlaceDeTerrain.CartePlacee = null;
        PlaceDeTerrain = null;

    }

    public void Retourner()
    {
        GetComponent<UnityEngine.UI.Image>().sprite = ImageOriginale;
        EstCachee = false;
    }

    //Tout en dessous c'est pour déplacer la carte

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetComponent<Carte>().EstEnJeu == false)
        {
            Debug.Log(GetComponent<Carte>().EstEnJeu);
            canvasGroup.alpha = .7f; // Opacité de la carte quand je clique dessus
            canvasGroup.blocksRaycasts = false;
        }
        JeuEnCours.Warning(" ");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GetComponent<Carte>().EstEnJeu == false)
        { rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; }
    }

    public void OnEndDrag(PointerEventData eventData)
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

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (JeuEnCours.EnTrainCibleCarte == true && EstEnJeu == true)
        {
            JeuEnCours.CarteCiblee = this;
        }
        else if (EstEnJeu == true && (EstCachee == false || JeuEnCours.JoueurActif == Appartenance)) //Si elle est montré ou qu'elle nous appartient
        {

            if (JeuEnCours.CarteMontree == null) //Je montre une carte
            {
                JeuEnCours.grosseCarte.SeDecaler(Stats, true);
                JeuEnCours.CarteMontree = this;
                if (JeuEnCours.Tour > 2 && Immobile == 0) { MettreBonBoutons(); }
                EstMontreeSurCarte = true;
            }
            else if (Id == JeuEnCours.grosseCarte.Carte.Id) //Je cache la carte
            {
                JeuEnCours.CacherGrandeCarte();
            }
            else // J'échange de carte
            {
                JeuEnCours.grosseCarte.SeDecaler(JeuEnCours.CarteMontree.Stats, false);
                JeuEnCours.CarteMontree.EstMontreeSurCarte = false;
                JeuEnCours.EnleverBonBoutons();
                if (JeuEnCours.Tour > 2 && Immobile == 0) { MettreBonBoutons(); }
                JeuEnCours.grosseCarte.SeDecaler(Stats, true);
                JeuEnCours.CarteMontree = this;
            }
            Debug.Log("Une carte est montrée :" + (JeuEnCours.CarteMontree != null));
        }
        else if (EstCachee == true)
        { JeuEnCours.Warning("Vous ne pouvez pas voir cette carte."); }
    }

    public void OnDrop(PointerEventData eventData)
    { Debug.Log("On dirait une erreur."); }

}