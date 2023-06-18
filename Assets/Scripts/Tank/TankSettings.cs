using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class TankSettings : NetworkBehaviour
{
    private MeshRenderer[] renderers;

    public List<Color> colors = new List<Color>();
    public List<Transform> positions = new List<Transform>();

    private void Awake()
    {
        renderers = this.GetComponentsInChildren<MeshRenderer>();
    }

    public override void OnNetworkSpawn()
    {
      
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = colors[(int) OwnerClientId];
        }
        
        transform.position = positions[(int)OwnerClientId].position;
    }
}
