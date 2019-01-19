using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Transform cachedTransform;
    private GameBaseService gs;
    public Transform cameraTransform;
    private Vector3 startingOffset;
    public Rigidbody2D rigidBody;
    public Collider2D colliderBox;
    public CharacterController2D physicsController;
    protected readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");



    protected Animator animator;

    protected Vector2 moveVector;
    protected TileBase currentSurface;


    private bool isJumping = false;

    public string endGameLayer = "EndGame";
    private int endGameLayerValue;

    public float movementSpeed = 1.0f;

    public float jumpMaxTime = 2.0f;
    private float currentJumpTime = 0.0f;
    public float startingJumpForce = 5.0f;
    private bool isGrounded = false;
    private Vector3 groundBelowPosition;

    private void Awake()
    {
        //not sure if this version of Unity already cache this
        cachedTransform = transform;
        endGameLayerValue = LayerMask.NameToLayer(endGameLayer);
        startingOffset = cameraTransform.localPosition - cachedTransform.localPosition;
    }

    // Use this for initialization
    void Start ()
    {
        if (ServiceLocator.Instance != null)
        {
            gs = ServiceLocator.Instance.GetServiceOfType<GameBaseService>(SERVICE_TYPE.GAMESERVICE);
            if (gs != null)
            {
                //register
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {

        if(CheckForJumpInput() && CheckForGrounded())
        {
            isJumping = true;
            moveVector += Vector2.up * startingJumpForce;
        }

        if (isJumping)
        {
            if (currentJumpTime < jumpMaxTime)
            {
                currentJumpTime += Time.deltaTime;
                moveVector += Vector2.up;
            }
            else
            {
                moveVector = new Vector2(moveVector.x, -9.81f);
                isJumping = false;
                currentJumpTime = 0.0f;
            }
        }

        if (Input.GetKey(KeyCode.A) && !CheckForLeftWall())
        {
            moveVector += Vector2.left * movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) && !CheckForRightWall())
        {
            moveVector += Vector2.right * movementSpeed * Time.deltaTime;
        }
        else if (moveVector.x < 0)
        {
            moveVector = new Vector2(0, moveVector.y);
        }
        else if(moveVector.x > 0)
        {
            moveVector = new Vector2(0, moveVector.y);
        }


        cameraTransform.localPosition = cachedTransform.localPosition + startingOffset;
    }

    private void FixedUpdate()
    {
        physicsController.Move(moveVector * Time.deltaTime);
        //m_Animator.SetFloat(m_HashHorizontalSpeedPara, m_MoveVector.x);
        //m_Animator.SetFloat(m_HashVerticalSpeedPara, m_MoveVector.y);

       
    }

    public bool CheckForJumpInput()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public bool CheckForGrounded()
    {
        //bool wasGrounded = animator.GetBool(m_HashGroundedPara);
        bool wasGrounded = false;
        bool grounded = physicsController.IsGrounded;
        //Debug.Log("IsGrounded:"+ grounded);
        if (grounded)
        {
            FindCurrentSurface();

            if (!wasGrounded && moveVector.y < -1.0f)
            {//only play the landing sound if falling "fast" enough (avoid small bump playing the landing sound)
                //play landing sound here
                
            }
            moveVector = new Vector2(moveVector.x, 0);
        }
        else
        {
            currentSurface = null;
        }
        isGrounded = grounded;
        //animator.SetBool(m_HashGroundedPara, grounded);

        return grounded;
    }

    public bool CheckForLeftWall()
    {
        return physicsController.IsHittingLeftWall;
    }

    public bool CheckForRightWall()
    {
        return physicsController.IsHittingRightWall;
    }

    public void FindCurrentSurface()
    {
        Collider2D groundCollider = physicsController.GroundColliders[0];

        if (groundCollider == null)
            groundCollider = physicsController.GroundColliders[1];

        if (groundCollider == null)
            return;

        TileBase b = PhysicsHelper.FindTileForOverride(groundCollider, transform.position, Vector2.down);
        if (b != null)
        {
            currentSurface = b;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogFormat("OnCollisionEnter laver:{0}  layer:{1}", collision.gameObject.layer, endGameLayerValue);
        if (collision.gameObject.layer == endGameLayerValue)
        {
           if(gs != null)
            {
                gs.EndGame(true);
            }
        }
    }

    

}
