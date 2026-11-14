using UnityEngine;
using System.Collections;

public class PortalLink : MonoBehaviour
{
    [SerializeField] private float teleportCooldown = 0.5f;
    [SerializeField] private Portal PortalA;
    [SerializeField] private Portal PortalB;
    [SerializeField] private ParticleSystem portalParticles;
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

        SpawnParticles(entryPortal);

        Rigidbody playerRigidbody = null;
        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            playerRigidbody = rb;
        }

        if (playerRigidbody == null) return;

        Portal exitPortal = (entryPortal == PortalA) ? PortalB : PortalA;

        playerRigidbody.position = exitPortal.transform.position;
        SpawnParticles(exitPortal);
        StartCoroutine(StartTeleportCooldown());
    }

    IEnumerator StartTeleportCooldown()
    {
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }

    void SpawnParticles(Portal targetPortal)
    {
        if (portalParticles == null) return;
        
        portalParticles.transform.position = targetPortal.transform.position;
        portalParticles.transform.rotation = targetPortal.transform.rotation;
        portalParticles.Play();
    }
}
