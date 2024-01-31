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

    public CarteSettings ChercherCarteSettings()
    {
        foreach (CarteSettings carteCherchee in JeuEnCours.CartesMontrable)
        {
            if (carteCherchee.Id == Id)
            {
                return carteCherchee;
            }
        }
        return JeuEnCours.CartesMontrable[0];
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
        if (EstEnJeu == true && (EstCachee == false || JeuEnCours.JoueurActif == Appartenance)) // 
        {

            if (JeuEnCours.CarteMontreeEnCours == false) //Je montre une carte
            {
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarteSettings(), true);
                //jeuEnCours.EnleverBouton();
                Debug.Log("Montrer");
                EstMontree = true;
            }
            else if (Id == JeuEnCours.grosseCarte.Carte.Id)
            {
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarteSettings(), false);
                //jeuEnCours.RemettreBouton();
                Debug.Log("Cacher");
                EstMontree = false;
            }
            else { return; }
            JeuEnCours.CarteMontreeEnCours = EstMontree;
            Debug.Log("Une carte est montrée :" + JeuEnCours.CarteMontreeEnCours);
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