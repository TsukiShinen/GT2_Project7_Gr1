using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarController : Entity
{
    [Header("Movement")]
    public float MoveSpeed;
    public float Acceleration;
    public float Decceleration;
    public float VelPower;
    [Space(10)]
    public float FrictionAmount;

    [Header("Jump")]
    public float JumpForce;
    [Range(0f, 1f)]
    public float JumpCutMultiplier;
    [Space(10)]
    public float JumpCoyoteTime;
    public float JumpBufferTime;
    [Space(10)]
    public float FallGravityMultiplier;

    [Header("Check")]
    public Transform GroundCheckPoint;
    public Vector2 GroundCheckSize;
    [Space(10)]
    public LayerMask GroundLayer;

    public float LastGroundedTime { get; set; }
    public float LastJumpTime { get; set; }
    public float GravityScale { get; private set; }

    public JarIdleState IdleState { get; private set; }
    public JarRunState RunState { get; private set; }
    public JarJumpState JumpState { get; private set; }
    public JarFallState FallState { get; private set; }

    public BoxCollider2D BoxCollider { get; private set; }

    private GameObject _player;

    public override void Awake()
    {
        base.Awake();

        IdleState = new JarIdleState(this);
        RunState = new JarRunState(this);
        JumpState = new JarJumpState(this);
        FallState = new JarFallState(this);

        BoxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.Space))
        {
            LastJumpTime = JumpBufferTime;
        }

        LastJumpTime -= Time.deltaTime;
        LastGroundedTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Rigidbody.velocity = new Vector2(0, 0);
            _player.transform.parent = null;
            _player.SetActive(true);
            BoxCollider.isTrigger = true;
            Rigidbody.gravityScale = 0;
            transform.GetChild(2).gameObject.SetActive(false);
            transform.gameObject.tag = "Untagged";
            Animator.SetBool("Control", false);
            ChangeState(null);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // Check
        if (Physics2D.OverlapBox(GroundCheckPoint.position, GroundCheckSize, 0, GroundLayer))
        {
            LastGroundedTime = JumpCoyoteTime;
        }
    }

    public void PlayerControlJar()
    {
        lifeBar = _player.gameObject.GetComponent<Entity>().lifeBar;
        _player.transform.parent = transform;
        _player.SetActive(false);
        BoxCollider.isTrigger = false;
        Rigidbody.gravityScale = 1;
        GravityScale = Rigidbody.gravityScale;
        transform.GetChild(2).gameObject.SetActive(true);
        transform.gameObject.tag = "Enemy";
        Animator.SetBool("Control", true);
        ChangeState(IdleState);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player = collision.gameObject;
    }

}
