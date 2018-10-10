using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FogOfWarEntity : Photon.MonoBehaviour
{
    //Keep track of Objects to activate/ deactivate when revealed
    public Image[] spriteImages = new Image[0];
    public SpriteRenderer[] spriteRenderer = new SpriteRenderer[0];
    public MeshRenderer[] meshrenderers = new MeshRenderer[0];
    public SkinnedMeshRenderer[] skinnedMeshRenderer = new SkinnedMeshRenderer[0];
    public ParticleSystem[] particleSystems = new ParticleSystem[0];

    public float visionRange = 6f;
    public FogOfWar.Players faction = FogOfWar.Players.Player00;
    private FogOfWar.Players previousFaction = FogOfWar.Players.Player00;

    [Range(0f, 255f)]
    public int upVision = 10;

    private Revealer revealer;
    private Revealer3D revealer3D;

    private bool isBeingRevealed = true;

    public bool dynamicBlock = false;

    private Rigidbody rbody;
    private SphereCollider sphereCollider;

    private void Start()
    {
       if (FogOfWar.fogAlignment == FogOfWar.FogAlignment.DDDMode)
        {
            previousFaction = FogOfWar3D.currentlyRevealed;
            rbody = gameObject.AddComponent<Rigidbody>();
            rbody.useGravity = false;
            rbody.isKinematic = true;
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;

            if (FogOfWar3D.currentlyRevealed != faction)
            {
                sphereCollider.radius = 0f;
                Hide();
            }
            else
            {
                sphereCollider.radius = visionRange;
            }

            revealer3D = new Revealer3D(visionRange,
            faction,
            transform);

            FogOfWar3D.RegisterRevealer(revealer3D);
        }
        else
        {
            revealer = new Revealer(visionRange,
            faction,
            upVision,
            gameObject);

            FogOfWar.RegisterRevealer(revealer);

            if (dynamicBlock)
            {
                FogOfWar.RegisterVisionBlocker(gameObject);
            }

            if (faction == FogOfWar.RevealFaction)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    private List<FogOfWarEntity> enteties = new List<FogOfWarEntity>();
    private int colliderCount = 0;

    private void OnTriggerEnter(Collider collider)
    {
        if (FogOfWar3D.currentlyRevealed == faction)
        {
            Show();
            return;
        }

        FogOfWarEntity e = collider.gameObject.GetComponent<FogOfWarEntity>();

        if (e != null)
        {
            if (e.faction != faction)
            {
                enteties.Add(e);
                colliderCount++;
                Show();
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (FogOfWar3D.currentlyRevealed == faction)
        {
            Show();
            return;
        }

        FogOfWarEntity e = collider.gameObject.GetComponent<FogOfWarEntity>();

        if (e != null)
        {
            int instanceID = e.GetInstanceID();
            for (int i = 0; i < colliderCount; i++)
            {
                if (instanceID == enteties[i].GetInstanceID())
                {
                    enteties.RemoveAt(i);
                    colliderCount--;
                }
            }

            if (colliderCount <= 0)
            {
                Hide();
            }
        }
    }

    public void OnEnable()
    {
        if (FogOfWar.fogAlignment == FogOfWar.FogAlignment.DDDMode)
        {
            if (revealer3D != null)
            {
                FogOfWar3D.RegisterRevealer(revealer3D);
            }
        }
        else
        {
            if (revealer != null)
            {
                FogOfWar.RegisterRevealer(revealer);
            }
        }
    }


    public void OnDisable()
    {
        if (FogOfWar.fogAlignment == FogOfWar.FogAlignment.DDDMode)
        {
            if (revealer3D != null)
            {
                FogOfWar3D.UnregisterRevealer(revealer3D.sceneReference.GetInstanceID());
            }
        }
        else
        {
            if (revealer != null)
            {
                FogOfWar.UnRegisterRevealer(revealer.sceneReference.GetInstanceID());
            }
        }
    }

    void Update()
    {
        if (FogOfWar.fogAlignment == FogOfWar.FogAlignment.Horizontal || FogOfWar.fogAlignment == FogOfWar.FogAlignment.Vertical)
        {
            if (revealer == null)
                return;

            revealer.visionRange = visionRange;

            if (faction != FogOfWar.RevealFaction)
            {
                if (FogOfWar.IsPositionRevealedByFaction(transform.position, FogOfWar.RevealFactionInt))
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
            else
            {
                Show();
            }
        }
        else
        {
            if (previousFaction != FogOfWar3D.currentlyRevealed)
            {
                previousFaction = FogOfWar3D.currentlyRevealed;
                if (faction != FogOfWar3D.currentlyRevealed)
                {
                    sphereCollider.radius = 0f;
                    Hide();
                }
                else
                {
                    sphereCollider.radius = visionRange;
                    Show();
                }
            }
        }
    }

    public void Hide()
    {
        if (!isBeingRevealed)
            return;

        int length = meshrenderers.Length;
        for (int i = 0; i < length; i++)
        {
            meshrenderers[i].enabled = false;
        }
        int sLength = skinnedMeshRenderer.Length;
        for (int i = 0; i < sLength; ++i)
        {
            skinnedMeshRenderer[i].enabled = false;
        }

        int imgLength = spriteImages.Length;
        for (int i = 0; i < imgLength; i++)
        {
            spriteImages[i].enabled = false;
        }

        int ptkLength = particleSystems.Length;
        for (int i = 0; i < ptkLength; i++)
        {
            var em = particleSystems[i].emission;
            em.enabled = false;
        }

        int sprtLength = spriteRenderer.Length;
        for (int i = 0; i < sprtLength; i++)
        {
            var sprt = spriteRenderer[i].enabled = false;
        }

        isBeingRevealed = false;
    }

    public void Show()
    {
        if (isBeingRevealed)
            return;

        int length = meshrenderers.Length;
        for (int i = 0; i < length; i++)
        {
            meshrenderers[i].enabled = true;
        }
        int sLength = skinnedMeshRenderer.Length;
        for (int i = 0; i < sLength; ++i)
        {
            skinnedMeshRenderer[i].enabled = true;
        }
        int imgLength = spriteImages.Length;
        for (int i = 0; i < imgLength; i++)
        {
            spriteImages[i].enabled = true;
        }

        int ptkLength = particleSystems.Length;
        for (int i = 0; i < ptkLength; i++)
        {
            var em = particleSystems[i].emission;
            em.enabled = true;
        }

        int sprtLength = spriteRenderer.Length;
        for (int i = 0; i < sprtLength; i++)
        {
            var sprt = spriteRenderer[i].enabled = true;
        }

        isBeingRevealed = true;
    }
}
