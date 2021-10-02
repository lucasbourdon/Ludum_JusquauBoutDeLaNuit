using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed & force : ")]
    public float moveSpeed;
    public float climbSpeed;
    public float jumpForce;

    [Header("Is it ? : ")]
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isAerial;
    [SerializeField] private bool isClimbing;

    [Header("General References : ")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask collisionLayers;
    public LayerMask decorLayers;
    public Rigidbody2D rb;
    //public Animator animator;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D playerCollider;

    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;
    private float verticalMovement;
    private bool facingRight = true;

    public static PlayerMovement instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerMovement dans la scène");
            return;
        }
        instance = this;
    }
    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
        verticalMovement = Input.GetAxis("Vertical") * climbSpeed * Time.fixedDeltaTime;


        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            isJumping = true;
        }

        Flip(rb.velocity.x);

        float characterVelocity = Mathf.Abs(rb.velocity.x);
        //animator.SetFloat("Speed", characterVelocity);
        //animator.SetBool("isClimbing", isClimbing);
    }



    // Start is called before the first frame update
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);
        isAerial = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, decorLayers);
        if (isAerial)
        {
            rb.gravityScale = 1;
        }
        MovePlayer(horizontalMovement, verticalMovement);
    }

    // Update is called once per frame
    void MovePlayer(float _horizontalMovement, float _verticalMovement)
    {
        if (!isClimbing)
        {
            Vector3 targetVelocity = new Vector2(_horizontalMovement, _verticalMovement);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

            if (isJumping)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                isJumping = false;
            }
        }
        else
        {
            Vector3 targetVelocity = new Vector2(0, _verticalMovement);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7 //check the int value in layer manager(User Defined starts at 8) 
            && !isAerial)
        {
            isAerial = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 7
            && isAerial)
        {
            isAerial = false;
        }
    }

    void Flip(float _velocity)
    {
        if (_velocity > 0.1f && !facingRight)
        {
            //spriteRenderer.flipX = false;
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
        else if (_velocity < -0.1f && facingRight)
        {
            //spriteRenderer.flipX = true;
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }


}
