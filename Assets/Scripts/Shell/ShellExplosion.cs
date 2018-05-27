using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
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
        // Find all the tanks in an area around the shell and damage them.
		Collider[] colliders = Physics.OverlapSphere(transform.position,m_ExplosionRadius,m_TankMask);
		for (int i = 0; i < colliders.Length; i++) {
			Rigidbody targetbody = colliders [i].GetComponent<Rigidbody> ();

			if (!targetbody)
				continue;

			targetbody.AddExplosionForce (m_ExplosionForce , transform.position , m_ExplosionRadius); 

			TankHealth targethealth = targetbody.GetComponent<TankHealth> ();
			if (!targethealth)
				continue;

			float damage = CalculateDamage (targetbody.position);
			targethealth.TakeDamage (damage);

		}

		m_ExplosionParticles.transform.parent = null;
		m_ExplosionParticles.Play ();
		m_ExplosionAudio.Play ();

		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
		Destroy (gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
		Vector3 exp_tar = targetPosition - transform.position;
		float dis = exp_tar.magnitude;
		float rel_dis = (m_ExplosionRadius - dis) / m_ExplosionRadius;
		float damage = rel_dis * m_MaxDamage;
		damage = Mathf.Max (0f, damage);
		return damage;
    }
}