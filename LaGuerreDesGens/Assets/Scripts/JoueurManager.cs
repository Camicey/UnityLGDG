using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JoueurManager : NetworkBehaviour
{
    public GameObject PrefabCarte;
    public GameObject PrefabTerrain;
    public GameObject PrefabDeck;
    public GameManager JeuEnCours;
    public MainMenu MenuEnCours;
    public GameObject DossierCarte;
    public GameObject DossierDeck;
    public Canvas Canvas;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameObject Derniere = GameObject.Find("GameManagerObject");
        JeuEnCours = Derniere.GetComponent<GameManager>();
        Derniere = GameObject.Find("CanvaMenus");
        MenuEnCours = Derniere.GetComponent<MainMenu>();
        DossierCarte = GameObject.Find("DossierCarte");
        DossierDeck = GameObject.Find("SlotsDecks");
        Derniere = GameObject.Find("Canvas");
        Canvas = Derniere.GetComponent<Canvas>();
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    //Commands are methods requested by Clients to run on the Server
    [Command]
    public void CmdInstancier()
    {
        JeuEnCours.ToutesLesCartes.Clear();
        JeuEnCours.J1.Terrain.Clear();
        JeuEnCours.J2.Terrain.Clear();
        JeuEnCours.J1.Deck.Clear();
        JeuEnCours.J2.Deck.Clear();
        JeuEnCours.ToutTerrain.Clear();
        foreach (CarteSettings CarteS in JeuEnCours.CartesMontrable) //On instancie les cartes
        {
            GameObject Derniere = Instantiate(PrefabCarte, new Vector3(-500, -500, 0), Quaternion.identity, DossierCarte.transform);
            Derniere.GetComponent<Carte>().Stats = CarteS;
            Derniere.GetComponent<Carte>().JeuEnCours = JeuEnCours;
            Derniere.GetComponent<Carte>().canvas = Canvas;
            Derniere.GetComponent<Carte>().Commencer();
            NetworkServer.Spawn(Derniere, connectionToClient);
        }

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
            if (i > 5) { Derniere.GetComponent<PlaceTerrain>().GetComponent<RectTransform>().anchoredPosition = new Vector2(1200, 0); }
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
        Debug.Log(JeuEnCours.J1.Deck.Count);

        if (JeuEnCours.TypePartie == "Longue") { MenuEnCours.InitialiserLonguePartie(); }
        else { MenuEnCours.InitialiserPetitePartie(); }
        MenuEnCours.PlacerDeck(JeuEnCours.J1);
        MenuEnCours.PlacerDeck(JeuEnCours.J2);
        JeuEnCours.NouvellePartie();
        MenuEnCours.Ecran.SetActive(false);
    }

    [Command]
    public void CmdPiocher(GameObject carte)
    {
        RpcShowCard(carte, "Dealt");
    }

    [ClientRpc]
    void RpcShowCard(GameObject carte, string type)
    {
        //if the card has been "Dealt," determine whether this Client has authority over it, and send it either to the PlayerArea or EnemyArea, accordingly. For the latter, flip it so the player can't see the front!
        if (type == "Dealt")
        {
            if (isOwned)
            {
                //card.transform.SetParent(PlayerArea.transform, false);
                
            }
            else
            {
                //card.transform.SetParent(EnemyArea.transform, false);
                carte.GetComponent<Carte>().MettreCarteCachee();
            }
        }
        //if the card has been "Played," send it to the DropZone. If this Client doesn't have authority over it, flip it so the player can now see the front!
        else if (type == "Played")
        {
            //card.transform.SetParent(DropZone.transform, false);
            if (!isOwned)
            {
                carte.GetComponent<Carte>().MettreCarteCachee();
            }
        }
    }

}
