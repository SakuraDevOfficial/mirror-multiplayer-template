using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private GameObject nameMenu;
        private void Update()
        {
            if (Input.anyKeyDown)
            {
                nameMenu.SetActive(true);
                enabled = false;
            }
        }
    }
}