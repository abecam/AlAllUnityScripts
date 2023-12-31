using UnityEngine;
using Mirror;
using TMPro;

namespace TGB.SniperVsRunner
{
    public class PlayerScore : NetworkBehaviour
    {
        [Header("Components")]
        public TextMeshPro scoreText;

        [SyncVar]
        public int index;

        [SyncVar]
        public uint score;

        private void Update()
        {
            scoreText.SetText($"P{index}: {score:0000000}");
        }

        void OnGUI()
        {
            GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score:0000000}");
        }
    }
}
