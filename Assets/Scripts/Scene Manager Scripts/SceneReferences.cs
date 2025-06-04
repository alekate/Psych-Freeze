using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clase09
{
    public class SceneReferences : MonoBehaviour
    {
        ////////// EVENTO QUE SE DISPARA CUANDO ESTA ESCENA ESTÁ LISTA //////////

        public static event Action<SceneReferences> onLoadedScene;

        ////////// DATOS DE ESTA ESCENA //////////

        [field: SerializeField] public Transform previousState { get; private set; } // Donde aparece el jugador al entrar
        [field: SerializeField] public List<GameObject> gameObjects { get; private set; } = new List<GameObject>(); // Cosas para activar/desactivar

        ////////// CUANDO LA ESCENA SE INICIA, AVISA QUE ESTÁ LISTA //////////

        private void Start()
        {
            onLoadedScene?.Invoke(this); // Avisa al GameManager u otros que esta escena ya está cargada
        }

        ////////// ACTIVA O DESACTIVA TODOS LOS OBJETOS DE ESTA ESCENA //////////

        public void SetActiveGo(bool state)
        {
            foreach (GameObject worldObject in gameObjects)
                worldObject.SetActive(state);
        }
    }
}
