using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;

    public void ResetToDefault()
    {
        foreach (var gameObject in gameObjects)
            gameObject.GetComponent<Resettable>().ResetToDefault();
    }
}