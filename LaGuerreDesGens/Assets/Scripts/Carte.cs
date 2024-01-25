using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public bool EstCachee;
    public bool EstMontree;
    public bool EstEnJeu;
    public bool Stratege;

    //public Joueur Appartenance;
    public Vector2 PositionBase;
    public GameManager JeuEnCours;
    public PlaceDeck PlaceOccupee;
    public PlaceTerrain PlaceTerrainOccupee;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        EstEnJeu = false;
        EstCachee = false;
        EstMontree = false;
        Stratege = false;
        PositionBase = rectTransform.anchoredPosition;
    }
    /*
        public void Retirer()
        {
            EstEnJeu = false;
            EstCachee = false;
            //carteAssocie.SeDecaler(false);
            //jeuEnCours.Retire(this);
        }
    */
    public void Agrandir()
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

    public CarteSettings ChercherCarte()
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
        else
        {
            PlaceOccupee.availableCarteSlots = true;
            PlaceOccupee = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");


        if (EstEnJeu == true) // && EstMontree == false
        {
            if (JeuEnCours.CarteMontreeEnCours == false)
            {
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarte(), true);
                //jeuEnCours.EnleverBouton();
                Debug.Log("Montrer");
            }
            else if (Id == JeuEnCours.grosseCarte.Carte.Id)
            {
                JeuEnCours.grosseCarte.SeDecaler(ChercherCarte(), false);
                //jeuEnCours.RemettreBouton();
                Debug.Log("Cacher");
            }
            else { return; }
            EstMontree = !EstMontree;
            JeuEnCours.CarteMontreeEnCours = EstMontree;
            Debug.Log("Une carte est montrée :" + JeuEnCours.CarteMontreeEnCours);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

}