using System.Collections.Generic;
using UnityEngine;

namespace Noted.Game
{
    /// <summary>Sprays physical coins from where a task was finished.</summary>
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] Material coinMaterial;
        [SerializeField] GameFlow flow;
        [SerializeField] CharacterController player;

        readonly List<CoinPickup> live = new List<CoinPickup>();

        public int Count => live.Count;

        public void Spawn(Vector3 origin, int totalValue, int count)
        {
            if (count <= 0 || totalValue <= 0) return;
            int each = totalValue / count;
            int remainder = totalValue - each * count;
            for (int i = 0; i < count; i++)
            {
                int v = each + (i == 0 ? remainder : 0);
                var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                go.name = "Coin";
                go.transform.SetParent(transform, true);
                go.transform.position = origin + Random.insideUnitSphere * 0.15f;
                go.transform.localScale = new Vector3(0.28f, 0.025f, 0.28f);
                go.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), 0f, Random.Range(0f, 360f));
                if (coinMaterial != null) go.GetComponent<Renderer>().sharedMaterial = coinMaterial;

                // Swap the default collider for a small sphere, and keep the coin out of the player's way.
                Destroy(go.GetComponent<Collider>());
                var col = go.AddComponent<SphereCollider>();
                col.radius = 0.5f;
                var rb = go.AddComponent<Rigidbody>();
                rb.mass = 0.05f;
                rb.linearVelocity = new Vector3(Random.Range(-1.6f, 1.6f), Random.Range(2.5f, 4f), Random.Range(-1.6f, 1.6f));
                rb.angularVelocity = Random.insideUnitSphere * 12f;
                if (player != null) Physics.IgnoreCollision(col, player);

                var pickup = go.AddComponent<CoinPickup>();
                pickup.Init(flow, v);
                live.Add(pickup);
            }
        }

        public void Forget(CoinPickup coin)
        {
            live.Remove(coin);
        }

        public void ClearAll()
        {
            foreach (var c in live)
            {
                if (c != null) Destroy(c.gameObject);
            }
            live.Clear();
        }
    }
}
