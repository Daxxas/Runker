using System;
using Game.Level;
using UnityEngine;

namespace UI
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private GameObject levelStartUI;
        [SerializeField] private GameObject levelEndUI;
        
        private void Start()
        {
            LevelManager.onLevelStart += () => DisplayStartLevelUI(false);
            LevelManager.onLevelEnd += () => DisplayEndLevelUI(true);
        }
        
        private void DisplayStartLevelUI(bool display)
        {
            levelStartUI.SetActive(display);
        }
        
        private void DisplayEndLevelUI(bool display)
        {
            levelEndUI.SetActive(display);
        }
    }
}