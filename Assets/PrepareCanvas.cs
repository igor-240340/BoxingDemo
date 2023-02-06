using Unity.Netcode;
using UnityEngine;

public class PrepareCanvas : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnDefencePrepareButtonClick(int bodyPartIndex)
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().defenceScheme[bodyPartIndex] =
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().defenceScheme[bodyPartIndex] ^ 1;

        Debug.Log($"Defence body part index: {bodyPartIndex}");
    }

    public void OnAttackPrepareButtonClick(int bodyPartIndex)
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().attackScheme[bodyPartIndex]++;
        
        Debug.Log($"Attack body part index: {bodyPartIndex}");
    }
}