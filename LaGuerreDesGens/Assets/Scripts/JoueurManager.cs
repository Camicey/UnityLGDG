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
        base.OnStartServer(); //Je sais pas pourquoi il est là
        Debug.Log("Player has been spawned on the server!");
    }

    /*
        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("Connected to Server!");
        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("Disconnected from Server!");
        }
    */
    //Les Commands sont les méthodes requêtées par les Clients pour les jouer sur le Server
    [Command]
    public void CmdInstancier()
    {
        //D'abbord, on fait en sorte que tout soit vide pour éviter les bugs
        JeuEnCours.ToutesLesCartes.Clear();
        JeuEnCours.J1.Terrain.Clear();
        JeuEnCours.J2.Terrain.Clear();
        JeuEnCours.J1.Deck.Clear();
        JeuEnCours.J2.Deck.Clear();
        JeuEnCours.ToutTerrain.Clear();

        //Puis on procède aux instanciations
        foreach (CarteSettings CarteS in JeuEnCours.CartesMontrable) //On instancie les cartes
        {
            GameObject Derniere = Instantiate(PrefabCarte, new Vector3(0, 0, 0), Quaternion.identity, DossierCarte.transform); //-500 -500
            NetworkServer.Spawn(Derniere, connectionToClient);
            Derniere.GetComponent<Carte>().Stats = CarteS;
            Derniere.GetComponent<Carte>().JeuEnCours = JeuEnCours;
            Derniere.GetComponent<Carte>().canvas = Canvas;
            Derniere.GetComponent<Carte>().Commencer();
        }

        for (int i = 0; i <= 10; i++) //On instancie les terrains
        {
            GameObject Derniere = Instantiate(PrefabTerrain, new Vector3(-100, -200, 0), Quaternion.identity, JeuEnCours.DossierTerrain.transform);
            NetworkServer.Spawn(Derniere, connectionToClient);
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
            NetworkServer.Spawn(Derniere, connectionToClient);
            Derniere.GetComponent<PlaceDeck>().Id = i;
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

        if (JeuEnCours.TypePartie == "Longue") { MenuEnCours.InitialiserLonguePartie(); }
        else { MenuEnCours.InitialiserPetitePartie(); }
        MenuEnCours.PlacerDeck(JeuEnCours.J1);
        MenuEnCours.PlacerDeck(JeuEnCours.J2);
        JeuEnCours.NouvellePartie();
        MenuEnCours.Ecran.SetActive(false);
    }

    /* 
    [Command]
    public void CmdPiocher(GameObject carte)
    {
        RpcShowCard(carte, "Dealt");
    }
       
             [ClientRpc] //Permet au serveur de communiquer un changement à tous les clients
             void RpcShowCard(GameObject carte, string type)
             {
                 //Si la carte est "Dealt," il détermine si le client à une autorité dessus, et l'envoie au bon endroit en fonction.
                 if (type == "Dealt")
                 {
                     if (isOwned)
                     {
                         //Déposer la carte dans le deck allié
                     }
                     else
                     {
                         //Déposer la carte dans le deck ennemi
                         carte.GetComponent<Carte>().CacherCarte();
                     }
                 }
                 //Si la carte est jouée, et qu'elle a autorité dessus on la met chez l'allié, sinon, on la met chez l'ennemi
                 else if (type == "Played")
                 {
                     if (isOwned)
                     {
                         //Dépose la carte du côté allié
                     }
                     else
                     {
                         //Dépose la carte du côté ennemi 
                         carte.GetComponent<Carte>().CacherCarte();
                     }
                 }

     }*/
}
