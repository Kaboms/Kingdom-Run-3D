using UnityEngine;

public class MobileEnemy : MonoBehaviour
{
	public GameObject MeshObject;

	private Transform _player;

	private Rigidbody _rigidbody;

	private Animator _animator;

	private ParticleSystem _particleSystem;

	private bool _start = false;
	private bool _destroyed = false;

	private int _activateDistance = 30;
	//--------------------------------------------------------------------------

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = MeshObject.GetComponent<Animator>();
		_particleSystem = GetComponent<ParticleSystem>();

		_animator.enabled = false;

		_player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	//--------------------------------------------------------------------------

	void FixedUpdate()
	{
		if (_destroyed)
			return;

		if (!_start && Vector3.Distance(_player.position, transform.position) < _activateDistance)
		{
			_rigidbody.velocity = transform.forward * Time.deltaTime * 250;
			_start = true;
			_animator.enabled = true;
		}
		else if (_start && Vector3.Distance(_player.position, transform.position) > _activateDistance + 5)
		{
			Destroy(this.gameObject);
		}
	}
	//--------------------------------------------------------------------------

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * _activateDistance);
	}
	//--------------------------------------------------------------------------

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("Ground"))
		{
			_destroyed = true;
			_rigidbody.velocity = Vector3.zero;

			_animator.enabled = false;
			GetComponent<Collider>().enabled = false;
			GetComponent<MeshRenderer>().enabled = false;

			_particleSystem.Play();

			Destroy(this.gameObject, _particleSystem.main.duration);
		}
	}
	//--------------------------------------------------------------------------

}
