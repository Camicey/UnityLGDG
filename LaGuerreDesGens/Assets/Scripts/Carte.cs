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
    public string Prenom;
    public float PM;
    public int PV;
    public int PVar;
    public int PA;
    public string Type;
    public string Famille;
    public int IdPouvoir;
    public float CoutPouvoir;
    public List<Carte> Liens = new List<Carte>();
    public bool EstCachee = false; //Est cachée à l'adversaire
    public bool EstMontree = false; //Est montrée sur la grande carte
    public bool EstEnJeu = false; //Est sur le plateau
    public bool Stratege = false;

    public Joueur Appartenance;
    public Vector2 PositionBase;
    public GameManager JeuEnCours;
    public PlaceDeck PlaceOccupee;
    public PlaceTerrain PlaceTerrainOccupee;
    public Sprite ImageOriginale;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        PositionBase = rectTransform.anchoredPosition;
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

    public CarteSettings ChercherCarteSettings(int id)
    {
        foreach (CarteSettings carteCherchee in JeuEnCours.CartesMontrable)
        {
            if (carteCherchee.Id == id)
            {
                return carteCherchee;
            }
        }
        return JeuEnCours.CartesMontrable[0];
    }

    public void MettreBonBoutons()
    {
        if (JeuEnCours.JoueurActif.CartesPossedees.Contains(this))
        {
            if (Stratege == true) { JeuEnCours.MontrerBoutonsChoix("StrategeAllie"); } // Il faudra rajouter carte seule a la fin ;-;
            else { JeuEnCours.MontrerBoutonsChoix("Allie"); }
        }
        else
        {
            if (Stratege == true) { JeuEnCours.MontrerBoutonsChoix("StrategeEnnemi"); }
            else { JeuEnCours.MontrerBoutonsChoix("Ennemi"); }
        }
    }

    //Tout en dessous c'est pour déplacer la carte

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnbeginDrag"); // Nous informe qu'on clique et commence à déplacer
        if (GetComponent<Carte>().EstEnJeu == false)
        {
            Debug.Log(GetComponent<Carte>().EstEnJeu);
            canvasGroup.alpha = .7f; // Opacité de la carte quand je clique dessus
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GetComponent<Carte>().EstEnJeu == false)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (GetComponent<Carte>().EstEnJeu == false)
        {
            rectTransform.anchoredPosition = PositionBase;
        }
        else if (PlaceOccupee != null)
        {

            PlaceOccupee.availableCarteSlots = true;
            PlaceOccupee.CartePlacee = null;
            PlaceOccupee = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (EstEnJeu == true && (EstCachee == false || JeuEnCours.JoueurActif == Appartenance)) //Si elle est montré ou qu'elle nous appartient
        {

            if (JeuEnCours.CarteMontree == null) //Je montre une carte
            {
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarteSettings(Id), true);
                JeuEnCours.CarteMontree = this;
                MettreBonBoutons();
                EstMontree = true;
            }
            else if (Id == JeuEnCours.grosseCarte.Carte.Id) //Je cache la carte
            {
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarteSettings(Id), false);
                JeuEnCours.CarteMontree = null;
                JeuEnCours.EnleverBonBoutons();
                EstMontree = false;
            }
            else // J'échange de carte
            {
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarteSettings(JeuEnCours.CarteMontree.Id), false);
                JeuEnCours.CarteMontree.EstMontree = false;
                JeuEnCours.EnleverBonBoutons();
                MettreBonBoutons();
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarteSettings(Id), true);
                JeuEnCours.CarteMontree = this;
            }
            Debug.Log("Une carte est montrée :" + (JeuEnCours.CarteMontree != null));
        }
        else
        {
            Debug.Log("Vous ne pouvez pas voir cette carte.");
        }
        /*
            if (JeuEnCours.EstEnTrainDeChoisirStratège == true && EstEnJeu == false)
            {

            }
        */
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On dirait une erreur.");
        //throw new System.NotImplementedException();
    }

}