using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clase09
{
    public class SceneReferences : MonoBehaviour
    {
        public static event Action<SceneReferences> onLoadedScene;

        [field: SerializeField] public Transform previousState { get; private set; }
        [field: SerializeField] public List<GameObject> gameObjects { get; private set; } = new List<GameObject>();

        private void Start ()
        {
            onLoadedScene?.Invoke(this);
        }

        public void SetActiveGo (bool state)
        {
            foreach (GameObject worldObject in gameObjects)
                worldObject.SetActive(state);
        }
    }
}