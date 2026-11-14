using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    public UnityEvent<GameObject, Portal> PlayerEntered;

    void Awake()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(playerTag)) return;
        if (other.isTrigger) return;

        PlayerEntered?.Invoke(other.gameObject, this);
    }
}
