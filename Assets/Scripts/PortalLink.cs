using UnityEngine;
using System.Collections;

public class PortalLink : MonoBehaviour
{
    [SerializeField] private float teleportCooldown = 0.5f;
    [SerializeField] private Portal PortalA;
    [SerializeField] private Portal PortalB;
    private bool isTeleporting = false;

    void Awake()
    {
        if (PortalA == null || PortalB == null) return;

        PortalA.PlayerEntered.AddListener(TeleportPlayer);
        PortalB.PlayerEntered.AddListener(TeleportPlayer);
    }
    void TeleportPlayer(GameObject player, Portal entryPortal)
    {
        if (isTeleporting) return;
        isTeleporting = true;

        Rigidbody playerRigidbody = null;
        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            playerRigidbody = rb;
        }

        if (playerRigidbody == null) return;

        Portal exitPortal = (entryPortal == PortalA) ? PortalB : PortalA;

        playerRigidbody.position = exitPortal.transform.position;
        StartCoroutine(StartTeleportCooldown());
    }

    IEnumerator StartTeleportCooldown()
    {
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }
}
