using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Extensions;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using IdlessChaye.IdleToolkit;

namespace IdlessChaye.TouhouWaltz
{
	public class Player : NetworkBehaviour
	{
		#region Singleton
		private static Player _instance;
		public static Player Instance => _instance;
		#endregion

		[SerializeField]
		private GameObject bulletSmall;
		[SerializeField]
		private GameObject center;


		private NetworkVariableULong _playerID = new NetworkVariableULong(
			new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly 
				,ReadPermission = NetworkVariablePermission.Everyone}
			);
		public ulong PlayerID => _playerID.Value;


		#region Movement
		private NetworkVariable<Vector2> _direction = new NetworkVariable<Vector2>(
			new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }
			);
		private NetworkVariable<bool> _isLowMoveSpeed = new NetworkVariable<bool>(
			new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }
			);

		[SerializeField]
		private NetworkVariableFloat _highMoveSpeed = new NetworkVariableFloat(Const.HighMoveSpeedDefault);
		[SerializeField]
		private NetworkVariableFloat _lowMoveSpeed = new NetworkVariableFloat(Const.LowMoveSpeedDefault);
		#endregion


		public NetworkVariableInt HP = new NetworkVariableInt(Const.HPDefault);
		public NetworkVariableInt Boom = new NetworkVariableInt(Const.BoomDefault);
		public NetworkVariableInt Power = new NetworkVariableInt(Const.PowerDefault);
		public NetworkVariableUInt Death = new NetworkVariableUInt(0);


		private Camera _mainCam;
		private Rigidbody2D _rig;
		private NetworkObjectPool _pool;
		private Transform _transform;


		void OnHealth(int oldValue, int newValue)
		{
			print("ID: " + PlayerID + "value: " + newValue);
		}

		void OnEnable()
		{
			HP.OnValueChanged += OnHealth;
		}

		void OnDisable()
		{
			HP.OnValueChanged -= OnHealth;
		}

		private void Start()
		{
			_mainCam = Camera.main;
			_rig = GetComponent<Rigidbody2D>();
			_transform = this.transform;
		}

		public override void NetworkStart()
		{
			if (IsLocalPlayer)
			{
				_playerID.Value = OwnerClientId;
				_instance = this;
			}

			if (IsServer)
			{
				_pool = NetworkManager.Singleton.GetComponent<NetworkObjectPool>();
			}
		}

		void Update()
		{
			if (IsServer)
			{
				UpdateServer();
			}
			
			if (IsLocalPlayer)
			{ 
				UpdateLocalPlayer();
			}
		}

		private void LateUpdate()
		{
			if (IsLocalPlayer)
			{ 
				Vector3 pos = transform.position;
				pos.z = -50;
				_mainCam.transform.position = pos;
			}
		}

		void UpdateServer()
		{
			if (!IsServer)
				return;
			
		}

		void UpdateLocalPlayer()
		{
			if (!IsLocalPlayer)
				return;

			#region Movement Management

			Vector2 direction = new Vector2();

			if (Input.GetKey(Const.KeyMoveLeft))
			{
				direction.x = -1;
			}
			if (Input.GetKey(Const.KeyMoveRight))
			{
				direction.x = 1;
			}
			if (Input.GetKey(Const.KeyMoveUp))
			{
				direction.y = 1;
			}
			if (Input.GetKey(Const.KeyMoveDown))
			{
				direction.y = -1;
			}
			_direction.Value = direction;

			bool isLowMoveSpeed = false;
			if (Input.GetKey(Const.KeyLowMoveSpeed))
			{
				isLowMoveSpeed = true;
			}
			_isLowMoveSpeed.Value = isLowMoveSpeed;
			Util.TrySetActive(center, isLowMoveSpeed);

			if (_isLowMoveSpeed.Value)
			{
				_rig.velocity = _direction.Value * _lowMoveSpeed.Value;
			}
			else
			{
				_rig.velocity = _direction.Value * _highMoveSpeed.Value;
			}

			#endregion

			if (Input.GetKeyDown(KeyCode.Space))
			{
				Fire(Bullet.BulletType.Big);
			}
		}

		public void Fire(Bullet.BulletType type)
		{
			Vector2 bulletDir = Vector2.zero;
			var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
			float t = (0 - ray.origin.z) / ray.direction.z;
			Vector3 rayPos = t * ray.direction.normalized + ray.origin;
			Vector2 dirVec = (Vector2)(rayPos - _transform.position);
			bulletDir = dirVec.normalized;
			float speed = dirVec.magnitude / Const.JudgeLengthBulletSpeed * Const.MaxBulletSpeed;
			speed = Mathf.Clamp(speed, Const.MinBulletSpeed, Const.MaxBulletSpeed);
			FireServerRpc(bulletDir, speed, type);
		}

	
		[ServerRpc]
		private void FireServerRpc(Vector2 dir,float speed, Bullet.BulletType type)
		{
			Vector3 up = new Vector3(dir.x, dir.y, 0);

			if (Power.Value == 4)
			{
				Fire(Quaternion.Euler(0, 0, 20) * up, speed, type);
				Fire(Quaternion.Euler(0, 0, -20) * up, speed, type);
				Fire(up, speed, type);
				Fire(Quaternion.Euler(0, 0, 160) * up, speed, type);
				Fire(Quaternion.Euler(0, 0, -160) * up, speed, type);
			}
			else if (Power.Value == 3)
			{
				Fire(Quaternion.Euler(0, 0, 20) * up, speed, type);
				Fire(Quaternion.Euler(0, 0, -20) * up, speed, type);
				Fire(up, speed, type);
			}
			else if (Power.Value == 2)
			{
				Fire(Quaternion.Euler(0, 0, -10) * up, speed, type);
				Fire(Quaternion.Euler(0, 0, 10) * up, speed, type);
			}
			else if (Power.Value == 1)
			{
				Fire(up, speed, type);
			}
			else if (Power.Value == 0)
			{
				Fire(up, speed, type);
			}
		}

		private void Fire(Vector3 direction, float speed, Bullet.BulletType type)
		{
			if (!IsServer)
				return;

			GameObject bullet = _pool.GetNetworkObject(bulletSmall);
			bullet.transform.position = transform.position + direction;

			var bulletRb = bullet.GetComponent<Rigidbody2D>();
			Vector2 velocity = (Vector2)(direction) * speed;
			bulletRb.velocity = velocity;

			bullet.GetComponent<Bullet>().Config(PlayerID, 10, _pool, type);

			bullet.GetComponent<NetworkObject>().Spawn(null, true);
		}


		public void TakeDamage(uint damage)
		{
			if (!IsServer)
				return;

			int hp = HP.Value;
			hp -= 1;
			if (hp <= 0)
			{
				hp = Const.HPDefault;
				Boom.Value = Const.BoomDefault;
				Power.Value = Const.PowerDefault;
				Death.Value = Death.Value + 1;
				TakeDamageClientRpc(damage);
			}
			HP.Value = hp;
		}

		[ClientRpc]
		private void TakeDamageClientRpc(uint damage)
		{
			if (!IsOwner)
				return;

			transform.position = Vector3.zero;
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		}
	}
}
