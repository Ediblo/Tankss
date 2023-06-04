using UnityEngine;
using System.Collections;
using Unity.Netcode;
using UnityEngine.Networking;

public class ShellExplosion : NetworkBehaviour
{
    public LayerMask m_TankMask;
    public GameObject m_ShellExplosionPrefab;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for(int i=0; i<colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamageServerRpc(damage);
        }

        PlayExplosionServerRpc();
       
    }



    [ServerRpc (RequireOwnership = false)]
    private void PlayExplosionServerRpc()
    {
        m_ExplosionParticles.transform.parent = null;
        GameObject shellExplosion = Instantiate(m_ShellExplosionPrefab, transform.position, transform.rotation);
        shellExplosion.GetComponent<NetworkObject>().Spawn();

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        
        Destroy(shellExplosion.gameObject, shellExplosion.GetComponent<ParticleSystem>().main.duration);
               
        this.gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);

    }

 

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }
}