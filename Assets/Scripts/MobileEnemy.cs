using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEnemy : MonoBehaviour
{
	public Transform Player;

	private Rigidbody _rigidbody;

	private Animator _animator;

	private bool _start = false;

	private int _activateDistance = 30;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();

		_animator.enabled = false;
	}

	void FixedUpdate()
	{
		if (!_start && Vector3.Distance(Player.position, transform.position) < _activateDistance)
		{
			_rigidbody.velocity = Vector3.back * Time.deltaTime * 250;
			_start = true;
			_animator.enabled = true;
		}
		else if (_start && Vector3.Distance(Player.position, transform.position) > _activateDistance + 5)
		{
			Destroy(this.gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine
		(
			transform.position,
			new Vector3
			(
				transform.position.x,
				transform.position.y,
				transform.position.z - (_activateDistance / 3)
			)
		);
	}


	private void OnCollisionEnter(Collision other)
	{
		if (!other.gameObject.CompareTag("Ground"))
			Destroy(this.gameObject);
	}
}
