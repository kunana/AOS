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

    //여기부터 명우가 넣은거
    public bool isInTheBush = false;
    public bool isInTheSightRange = false;
    public bool isInTheBushMyEnemyToo = false;
    private string _playerTeam = null;
    public string playerTeam
    {
        get
        {
            if(_playerTeam == null)
            {
                if (gameObject.name.Contains("Blue_Bot"))
                    print("");
                _playerTeam = PhotonNetwork.player.GetTeam().ToString();
                if (_playerTeam == "none") _playerTeam = "red"; //나중에 지워야할 디버그용 코드
                if (_playerTeam.Equals("red"))
                    _playerTeam = "Red";
                else
                    _playerTeam = "Blue";
            }
            return _playerTeam;
        }
    }
    private string _team = null;
    public string team
    {
        get
        {
            if (_team == null)
            {
                if (gameObject.tag.Equals("Player"))
                {
                    _team = gameObject.GetComponent<ChampionBehavior>().Team;
                }
                else if (gameObject.tag.Equals("Minion"))
                {
                    if (gameObject.name.Contains("Red"))
                        _team = "Red";
                    else
                        _team = "Blue";
                }
                else if (gameObject.tag.Equals("Tower"))
                {
                    _team = gameObject.GetComponent<TowerBehaviour>().Team;
                }
            }
            return _team;
        }
    }
    //여까지명우가넣은거

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
                isInTheSightRange = false;//명우가넣은거
                Check();
                //Hide();
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
                isInTheSightRange = true;//명우가넣은거
                Check();
                //Show();
            }
            else
            {
                isInTheSightRange = false;//명우가넣은거
                Check();
                //Hide();
            }
        }
    }

    private List<FogOfWarEntity> enteties = new List<FogOfWarEntity>();
    private int colliderCount = 0;

    private void OnTriggerEnter(Collider collider)
    {
        if (FogOfWar3D.currentlyRevealed == faction)
        {
            isInTheSightRange = true;//명우가넣은거
            Check();
            //Show();
            return;
        }

        FogOfWarEntity e = collider.gameObject.GetComponent<FogOfWarEntity>();

        if (e != null)
        {
            if (e.faction != faction)
            {
                enteties.Add(e);
                colliderCount++;
                isInTheSightRange = true;//명우가넣은거
                Check();
                //Show();
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (FogOfWar3D.currentlyRevealed == faction)
        {
            isInTheSightRange = true;//명우가넣은거
            Check();
            //Show();
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
                isInTheSightRange = false;//명우가넣은거
                Check();
                //Hide();
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
                    isInTheSightRange = true;//명우가넣은거
                    Check();
                    //Show();
                }
                else
                {
                    isInTheSightRange = false;//명우가넣은거
                    Check();
                    //Hide();
                }
            }
            else
            {
                isInTheSightRange = true;//명우가넣은거
                Check();
                //Show();
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
                    isInTheSightRange = false;//명우가넣은거
                    Check();
                    //Hide();
                }
                else
                {
                    sphereCollider.radius = visionRange;
                    isInTheSightRange = true;//명우가넣은거
                    Check();
                    //Show();
                }
            }
        }
    }

    public void Hide()
    {
        if (!isBeingRevealed)
            return;

        //if (!isInTheSightRange || (isInTheBush && !isInTheBushMyEnemyToo))//명우가추가
        //{
        int length = meshrenderers.Length;
        for (int i = 0; i < length; i++)
        {
            meshrenderers[i].enabled = false;
        }

        int sLength = skinnedMeshRenderer.Length;//명우가추가
        for (int i = 0; i < sLength; ++i)
        {//명우가추가
            skinnedMeshRenderer[i].enabled = false;//명우가추가
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
        //}
    }

    public void Show()
    {
        if (isBeingRevealed)
            return;

        //if (isInTheSightRange && (!isInTheBush || isInTheBushMyEnemyToo))//명우가추가
        //{
        int length = meshrenderers.Length;
        for (int i = 0; i < length; i++)
        {
            meshrenderers[i].enabled = true;
        }

        int sLength = skinnedMeshRenderer.Length;//명우가추가
        for (int i = 0; i < sLength; ++i)
        {//명우가추가
            skinnedMeshRenderer[i].enabled = true;//명우가추가
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
        //}
    }

    public void Check()
    {
        if (team != null)
        {
            if (team.Equals(playerTeam))
            {//아군은 늘 보임
                Show();
            }
            else if (isInTheSightRange)
            {//적이 시야 범위에 있음
                if (isInTheBush)
                {//시야 범위 내의 부쉬 안에 있음
                    if (isInTheBushMyEnemyToo)
                    {//그 부쉬에 우리 팀도 있으면 보임
                        Show();
                    }
                    else
                    {//그 부쉬에 우리 팀이 없으면 안보임
                        Hide();
                    }
                }
                else
                {//시야 범위 내인데 부쉬에도 없으면 보임
                    Show();
                }
            }
            else
            {//적이 시야 범위에 없으면 안보임
                Hide();
            }
        }
    }
}