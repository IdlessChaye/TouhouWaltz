using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Extensions;
using MLAPI.NetworkVariable;


namespace IdlessChaye.TouhouWaltz
{
	public class Bullet : NetworkBehaviour
	{
		public enum BulletType
		{
			Big,
			Middle,
			Small
		}

		public ulong owner;
		NetworkObjectPool _poolToReturn;

		NetworkVariableFloat _size = new NetworkVariableFloat();
		

		public void Config(ulong owner, float lifetime, NetworkObjectPool poolToReturn, BulletType type)
		{
			this.owner = owner;
			_poolToReturn = poolToReturn;
			InitByType(type);

			if (IsServer)
			{
				// This is bad code don't use invoke.
				Invoke(nameof(DestroyBullet), lifetime);
			}
		}

		private void InitByType(BulletType type)
		{
			switch (type)
			{
				case BulletType.Big:
					_size.Value = Const.BulletSizeBig;
					break;
				case BulletType.Middle:
					_size.Value = Const.BulletSizeMiddle;
					break;
				case BulletType.Small:
					_size.Value = Const.BulletSizeSmall;
					break;
			}
		}

		public override void NetworkStart()
		{
			transform.localScale = _size.Value * Vector3.one;
		}

		public void DestroyBullet()
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

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!IsServer || !NetworkObject.IsSpawned)
				return;

			var player = collision.GetComponent<Player>();
			if (player != null && player.PlayerID != owner)
			{
				player.TakeDamage(1);
				DestroyBullet();
			}
		}
	}
}
