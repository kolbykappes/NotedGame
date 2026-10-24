using UnityEngine;

namespace Noted.Game
{
    /// <summary>A loose Uptime coin. Walking within reach collects it; nothing is lost if the player leaves it.</summary>
    public class CoinPickup : MonoBehaviour
    {
        GameFlow flow;
        int value;
        float bornAt;

        public int Value => value;

        public void Init(GameFlow owner, int coinValue)
        {
            flow = owner;
            value = coinValue;
            bornAt = Time.time;
        }

        void Update()
        {
            if (flow == null || flow.PlayerTransform == null) return;
            if (Time.time - bornAt < 0.5f) return;
            float radius = flow.CoinPickupRadius;
            var d = flow.PlayerTransform.position + Vector3.up * 0.9f - transform.position;
            if (d.sqrMagnitude <= radius * radius || transform.position.y < -10f)
            {
                flow.CollectCoin(this);
            }
        }
    }
}
