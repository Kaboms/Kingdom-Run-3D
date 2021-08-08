using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public Camera Camera;

	public int Speed = 10;
	public int JumpForce = 10;
	public int Gravity = 20;

	public Text ScoreText;
	public GameObject HintText;

	public UnityEvent DeadEvent;

	private Rigidbody _rigidbody;
	private Animator _animator;
	private Vector3 _velocity;

	private Vector2 _touchPos;

	private float _xPos = 0;

	private const float _xStep = 2f;

	private const float _maxX = _xStep;
	private const float _minX = -_maxX;

	private bool _grounded = true;
	private bool _jump = false;

	private bool _dead = false;

	private bool _start = false;

	private float _score = 0;
	private int _goldCount = 0;

	private Vector3 _cameraOffset;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();

		ScoreText.text = _score.ToString();

		_cameraOffset = Camera.transform.position;
	}

	private void Update()
	{
		if (_dead)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			if (_start)
			{
				_touchPos = Input.mousePosition;
			}
			else
			{
				_start = true;
				_animator.SetBool("Start", true);
				HintText.SetActive(false);
			}
		}

		if (Input.GetMouseButtonUp(0) && _touchPos != Vector2.zero)
		{
			//Mouse move distance must be >= than 1% of screen size
			float mouseXMoveDistance = Mathf.Abs(_touchPos.x - Input.mousePosition.x);
			mouseXMoveDistance = (mouseXMoveDistance > 0) ? mouseXMoveDistance / Screen.width : 0;

			float mouseYMoveDistance = Mathf.Abs(_touchPos.y - Input.mousePosition.y);
			mouseYMoveDistance = (mouseYMoveDistance > 0) ? mouseYMoveDistance / Screen.height : 0;

			if (mouseXMoveDistance > mouseYMoveDistance && mouseXMoveDistance >= 0.1f)
			{
				if (_touchPos.x < Input.mousePosition.x && _xPos + _xStep <= _maxX)
				{
					// Move right
					_xPos += _xStep;
				}
				else if (_touchPos.x > Input.mousePosition.x && _xPos - _xStep >= _minX)
				{
					// Move left
					_xPos -= _xStep;
				}
			}
			else if (mouseYMoveDistance >= 0.1f)
			{
				if (_touchPos.y < Input.mousePosition.y && _grounded)
				{
					//Jump
					_animator.SetBool("Jump", true);
					_jump = true;
				}
			}
		}
	}

	private void FixedUpdate()
	{
		Run();

		UpdateScore();

		UpdateCamera();

		if (_jump & _grounded)
		{
			_velocity.y = JumpForce;
			_grounded = false;
		}
		else if (!_grounded)
		{
			_velocity.y -= Gravity * Time.deltaTime;
		}

		_rigidbody.velocity = _velocity;

	}

	private void UpdateScore()
	{
		if (!_start || _dead)
			return;

		_score += Time.deltaTime * (_goldCount * 0.1f + 1);
		ScoreText.text = Mathf.RoundToInt(_score).ToString();
	}

	//Move camera ignoring Y-Axis
	private void UpdateCamera()
	{
		Camera.transform.position = new Vector3(transform.position.x + _cameraOffset.x, _cameraOffset.y, transform.position.z + _cameraOffset.z);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Ground"))
		{
			_jump = false;
			_grounded = true;
			_velocity.y = 0;
			_animator.SetBool("Jump", false);
		}

		if (other.gameObject.CompareTag("Enemy"))
		{
			Dead();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Gold"))
		{
			Destroy(other.gameObject);
			++_goldCount;
		}

		if (other.CompareTag("Finish"))
		{
			OnRestart();
		}
	}

	private void Run()
	{
		if (_dead || !_start)
			return;

		Vector3 newPos = Vector3.MoveTowards(new Vector3(transform.position.x, 0, 0), new Vector3(_xPos, 0, 0), Time.deltaTime * 10);

		transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

		_velocity.z = Time.deltaTime * Speed;
	}

	private void Dead()
	{
		_dead = true;
		_animator.SetBool("Dead", _dead);
		_velocity.z = 0;

		DeadEvent.Invoke();
	}

	public void OnRestart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
