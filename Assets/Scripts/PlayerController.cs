using UnityEngine;
using UnityEngine.Events;

// Control player by user
public class PlayerController : MonoBehaviour
{
	public Camera Camera;

	public int Speed = 10;
	public int JumpForce = 10;
	public int Gravity = 20;

	public UnityEvent DeathEvent;

	private Rigidbody _rigidbody;
	private Animator _animator;
	private Vector3 _velocity;

	private Vector2 _touchPos;

	private float _dashDirection = 0;
	private Vector3 _dashDistancePass = Vector3.zero;
	private float _dashTarget = 0;
	private const float _dashDistance = 2f;

	// Center of the platform
	private Vector3 _normalizePos = Vector3.zero;

	// Rotate player
	private float _newAngle = 0;

	private bool _grounded = true;
	private bool _jump = false;
	private bool _onRotatePlatform = false;
	private bool _startRun = false;

	private CameraController _cameraController;
	//--------------------------------------------------------------------------

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();

		_cameraController = Camera.main.GetComponent<CameraController>();
	}
	//--------------------------------------------------------------------------

	private void Update()
	{
		PlayerControl();

		if (transform.position.y < -5)
			Die();
	}
	//--------------------------------------------------------------------------

	private void FixedUpdate()
	{
		Run();

		_grounded = false;
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 1, ~LayerMask.NameToLayer("Ground")))
		{
			_grounded = true;
		}
	}
	//--------------------------------------------------------------------------

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			_jump = false;
			_velocity.y = 0;
		}

		if (other.gameObject.CompareTag("Enemy"))
		{
			Die();
		}
	}
	//--------------------------------------------------------------------------

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("RotatePlatform"))
		{
			_onRotatePlatform = true;

			_dashTarget = 0;
			_dashDistancePass = Vector3.zero;
			_dashDirection = 0;

			_normalizePos = other.transform.position;
			_normalizePos.y = 0;
		}
	}
	//--------------------------------------------------------------------------

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("RotatePlatform"))
		{
			_onRotatePlatform = false;
		}
	}
	//--------------------------------------------------------------------------

	private void PlayerControl()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (_startRun)
			{
				_touchPos = Input.mousePosition;
			}
			else
			{
				_startRun = true;
				_animator.SetBool("Start", true);
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
				Dash();
			}
			else if (mouseYMoveDistance >= 0.1f && _grounded)
			{
				if (_touchPos.y < Input.mousePosition.y)
				{
					//Jump
					_animator.SetTrigger("Jump");
					_jump = true;
				}
				else
				{
					_animator.SetTrigger("Roll");
				}
			}
		}
	}
	//--------------------------------------------------------------------------

	private void Dash()
	{
		if (_dashDirection != 0)
			return;

		if (_onRotatePlatform)
		{
			_newAngle = transform.eulerAngles.y;
			if (_touchPos.x < Input.mousePosition.x)
			{
				_newAngle += 90;
			}
			else
			{
				_newAngle -= 90;
			}
			_onRotatePlatform = false;

			transform.position = new Vector3(_normalizePos.x, transform.position.y, _normalizePos.z);

			_cameraController.Rotate(_newAngle);
		}
		else
		{
			if (_touchPos.x < Input.mousePosition.x && _dashTarget + _dashDistance <= _dashDistance)
			{
				// Move right
				_dashDirection = 1;
				_dashTarget += _dashDistance;
			}
			else if (_touchPos.x > Input.mousePosition.x && _dashTarget - _dashDistance >= -_dashDistance)
			{
				// Move left
				_dashDirection = -1;
				_dashTarget -= _dashDistance;
			}
		}
	}
	//--------------------------------------------------------------------------

	private void Run()
	{
		if (!_startRun)
			return;

		HandleDash();

		Vector3 forward_velocity = transform.forward * Time.deltaTime * Speed;
		_velocity = new Vector3(forward_velocity.x, _velocity.y, forward_velocity.z);

		float smoothTime = Time.deltaTime * 2;

		if (_jump)
		{
			_velocity = Vector3.SmoothDamp(_velocity, new Vector3(_velocity.x, JumpForce, _velocity.z), ref _velocity, smoothTime);
			if (_velocity.y >= JumpForce)
				_jump = false;
		}
		else if (!_grounded)
		{
			_velocity.y = -Gravity;
		}

		_rigidbody.velocity = _velocity;
	}
	//--------------------------------------------------------------------------

	private void HandleDash()
	{
		Vector3 distance = transform.right * _dashDirection * Time.deltaTime * 10;
		transform.position += distance;

		_dashDistancePass += distance;
		if (Vector3.Distance(_dashDistancePass, transform.right * _dashTarget) <= 0.1f)
			_dashDirection = 0;

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _newAngle, 0), Time.deltaTime * 30);
		if (transform.rotation != Quaternion.Euler(0, _newAngle, 0))
			transform.position = Vector3.Slerp(transform.position, _normalizePos, Time.deltaTime * 10);
	}
	//--------------------------------------------------------------------------

	private void Die()
	{
		_animator.SetBool("Dead", true);
		_velocity = Vector3.zero;
		_velocity.y = -Gravity;
		_rigidbody.velocity = _velocity;
		DeathEvent.Invoke();

		Destroy(this);
	}
	//--------------------------------------------------------------------------
}