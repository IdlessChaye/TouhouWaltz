using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Extensions;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

namespace IdlessChaye.TouhouWaltz
{
	public class Player : NetworkBehaviour
	{
		private Transform _mainCamTransform;

		private NetworkVariable<Vector2> _direction = new NetworkVariable<Vector2>(
			new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }
			);
		private Vector2 _lastDir;
		private NetworkVariable<bool> _isLowMoveSpeed = new NetworkVariable<bool>(
			new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }
			);
		private bool _lastIsLowMoveSpeed;
		private Rigidbody2D _rig;

		[SerializeField]
		private NetworkVariableFloat _HighMoveSpeed = new NetworkVariableFloat(5);
		[SerializeField]
		private NetworkVariableFloat _LowMoveSpeed = new NetworkVariableFloat(1);

		private void Start()
		{
			_mainCamTransform = Camera.main.transform;
			_rig = GetComponent<Rigidbody2D>();
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

			if (direction != _lastDir)
			{
				_lastDir = direction;
				_direction.Value = direction;
			}


			bool isLowMoveSpeed = false;
			if (Input.GetKey(Const.KeyLowMoveSpeed))
			{
				isLowMoveSpeed = true;
			}

			if (_lastIsLowMoveSpeed != isLowMoveSpeed)
			{
				_lastIsLowMoveSpeed = isLowMoveSpeed;
				_isLowMoveSpeed.Value = isLowMoveSpeed;
			}

			if (_isLowMoveSpeed.Value)
			{
				_rig.velocity = _direction.Value * _LowMoveSpeed.Value;
			}
			else
			{
				_rig.velocity = _direction.Value * _HighMoveSpeed.Value;
			}

			#endregion

		}


	}
}
