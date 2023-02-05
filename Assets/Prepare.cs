using UnityEngine;

public class Prepare : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnDefencePrepareButtonClick(int bodyPartIndex)
    {
        Debug.Log($"Body part index: {bodyPartIndex}");
    }
}