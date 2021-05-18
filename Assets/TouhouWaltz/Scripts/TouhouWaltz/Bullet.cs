using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Extensions;


namespace IdlessChaye.TouhouWaltz
{
	public class Bullet : NetworkBehaviour
	{

		ulong _owner;
		NetworkObjectPool _poolToReturn;

		//public GameObject explosionParticle;

		public void Config(ulong owner, float lifetime, NetworkObjectPool poolToReturn)
		{
			_owner = owner;
			_poolToReturn = poolToReturn;

			if (IsServer)
			{
				// This is bad code don't use invoke.
				Invoke(nameof(DestroyBullet), lifetime);
			}
		}

		private void DestroyBullet()
		{
			if (!NetworkObject.IsSpawned)
			{
				return;
			}

			//Vector3 pos = transform.position;
			//pos.z = -2;
			//GameObject ex = Instantiate(explosionParticle, pos, Quaternion.identity);
			//Destroy(ex, 0.5f);

			NetworkObject.Despawn();
			_poolToReturn.ReturnNetworkObject(NetworkObject);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (!IsServer || !NetworkObject.IsSpawned)
				return;

			var player = collision.collider.GetComponent<Player>();
			if (player != null && player.PlayerID != _owner)
			{
				player.TakeDamage(1);
				DestroyBullet();
			}
		}
	}
}
