using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Networking;

public class TankHealth : NetworkBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private NetworkVariable<float> m_CurrentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> m_Dead = new NetworkVariable<bool>();            


    private void Awake()
    {
      
    }


    private void OnEnable()
    {
        m_CurrentHealth.Value = m_StartingHealth;
        m_Dead.Value = false;

        SetHealthUIServerRpc();
    }

    [ServerRpc (RequireOwnership = false)]
    public void TakeDamageServerRpc(float amount)
    {
         m_CurrentHealth.Value -= amount;

        SetHealthUIServerRpc();

        if (m_CurrentHealth.Value <= 0f && !m_Dead.Value)
        {
            OnDeathServerRpc();
        }
    }


    [ServerRpc (RequireOwnership = false)]
    private void SetHealthUIServerRpc()
    {
        m_Slider.value = m_CurrentHealth.Value;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth.Value / m_StartingHealth);
    }

    [ServerRpc (RequireOwnership = false)]
    private void OnDeathServerRpc()
    {
        m_Dead.Value = true;

        GameObject tankExplosion = Instantiate(m_ExplosionPrefab, transform.position, transform.rotation);
        tankExplosion.GetComponent<NetworkObject>().Spawn();

        Destroy(tankExplosion, 1f);

        this.gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}