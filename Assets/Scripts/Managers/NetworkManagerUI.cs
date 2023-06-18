using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBTN;
    [SerializeField] private Button hostBTN;
    [SerializeField] private Button clientBTN;

    private void Awake(){
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        serverBTN.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        hostBTN.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        clientBTN.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });

    }
}
