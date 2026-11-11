using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 5f);

    bool isAlive = true;

    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    BoxCollider2D feetCollider;
    CapsuleCollider2D myCapsuleCollider;
    float gravityScaleAtStart;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }

    void Update()
    {
        Run();
        FlipSprite();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if (!isAlive)
        {
            return;
        }

        if (value.isPressed)
        {
            AudioSource.PlayClipAtPoint(jumpSFX, myCapsuleCollider.transform.position);
            Jump();
        }
    }

    void Run()
    {
        if (!isAlive) { return; }

        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidBody.linearVelocity.y);
        myRigidBody.linearVelocity = playerVelocity;

        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.linearVelocity.x) > Mathf.Epsilon;

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnimator.SetBool("isRunning", hasHorizontalSpeed);
        }
    }

    void Jump()
    {
        if (!isAlive) { return; }

        Vector2 jumpVelocity = new Vector2(myRigidBody.linearVelocity.x, jumpSpeed);
        myRigidBody.linearVelocity = jumpVelocity;
    }

    void FlipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.linearVelocity.x) > Mathf.Epsilon;

        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.linearVelocity.x), 1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") ||
            collision.collider.CompareTag("Hazard"))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");

            AudioSource.PlayClipAtPoint(deathSFX, myCapsuleCollider.transform.position);

            myRigidBody.linearVelocity = deathKick;
            myCapsuleCollider.enabled = false;
            feetCollider.enabled = false;

            FindAnyObjectByType<GameSession>().ProcessPlayerDeath();
        }
    }
}