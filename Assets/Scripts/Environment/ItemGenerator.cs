using System;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class ItemGenerator : MonoBehaviour
    {
        public Item[] itemsToGenerate;
        public int maxItemCount;
        public float generationTime;
        public Rect bounds;
        public GameObject itemPrefab;
        private void Start()
        {
            InvokeRepeating(nameof(Generate),0,generationTime);
        }

        private void Generate()
        {
            if(transform.childCount >= maxItemCount) return;
            Item item = itemsToGenerate[Random.Range(0, itemsToGenerate.Length)];
            var pos = new Vector3(Random.Range(bounds.xMin, bounds.xMax), Random.Range(bounds.yMin, bounds.yMax));
            var itemRenderer = Instantiate(itemPrefab, pos, Quaternion.identity, transform).GetComponent<ItemRenderer>();
            itemRenderer.item = item;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.center.y, 0), new Vector3(bounds.width, bounds.height,1));
        }
    }
}