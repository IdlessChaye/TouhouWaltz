using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Extensions;

namespace IdlessChaye.TouhouWaltz
{
	public class Boom : NetworkBehaviour
	{
		public ulong owner;
		NetworkObjectPool _poolToReturn;

		public void Config(ulong owner, float lifetime, NetworkObjectPool poolToReturn)
		{
			this.owner = owner;
			_poolToReturn = poolToReturn;

			if (IsServer)
			{
				// This is bad code don't use invoke.
				Invoke(nameof(DestroyBoom), lifetime);
			}
		}

		private void DestroyBoom()
		{
			if (!NetworkObject.IsSpawned)
			{
				return;
			}

			NetworkObject.Despawn();
			_poolToReturn.ReturnNetworkObject(NetworkObject);
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (!IsServer || !NetworkObject.IsSpawned)
				return;

			var bullet = collision.GetComponent<Bullet>();
			if (bullet != null && bullet.owner != owner)
			{
				bullet.DestroyBullet();
			}
		}
	}
}
