using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform Player;

	private float _angle;

	private Vector3 _offset;
	private Vector3 _newOffset;
	//--------------------------------------------------------------------------

	public void Rotate(float angle)
	{
		_angle = angle;

		_newOffset = Quaternion.Euler(0, _angle, 0) * _offset; ;
	}
	//--------------------------------------------------------------------------

	private void Awake()
	{
		_offset = transform.position;
		_newOffset = _offset;

		if (Player == null)
		{
			GameObject player_gameObject = GameObject.FindGameObjectWithTag("Player");
			if (player_gameObject != null)
				Player = player_gameObject.transform;
		}
	}
	//--------------------------------------------------------------------------

	private void Update()
	{
		transform.position = new Vector3(Player.position.x + _offset.x, _offset.y, Player.position.z + _offset.z);
	}
	//--------------------------------------------------------------------------

	private void FixedUpdate()
	{
		_offset = Vector3.SmoothDamp(_offset, _newOffset, ref _offset, Time.deltaTime);

		transform.rotation = Quaternion.Slerp
		(
			transform.rotation,
			Quaternion.Euler(transform.rotation.eulerAngles.x, _angle, transform.rotation.eulerAngles.z),
			Time.deltaTime * 30
		);
	}
	//--------------------------------------------------------------------------
}
