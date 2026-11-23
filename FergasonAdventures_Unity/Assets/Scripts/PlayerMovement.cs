using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float groundPoundSpeed = 25f;
    [SerializeField] float diveSpeed = 10f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] Vector2 deathKick = new Vector2(0f, 5f);

    bool isAlive = true;
    bool isGroundPounding = false;

    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CircleCollider2D feetCollider;
    BoxCollider2D myBoxCollider;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        feetCollider = GetComponent<CircleCollider2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        CheckLanding();
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

    public void OnSprint(InputValue value)
    {
        if (!isAlive) { return; }

        if (value.isPressed)
        {
            moveSpeed = moveSpeed * 1.5f;
        }
        else
        {
            moveSpeed = moveSpeed / 1.5f;
        }
    }

    public void OnGroundPound(InputValue value)
    {
        if (!isAlive || isGroundPounding || !value.isPressed) { return; }

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        isGroundPounding = true;
        myRigidBody.linearVelocity = new Vector2(0f, -groundPoundSpeed);
    }

    void Jump()
    {
        if (!isAlive) { return; }

        Vector2 jumpVelocity = new Vector2(myRigidBody.linearVelocity.x, jumpSpeed);
        myRigidBody.linearVelocity = jumpVelocity;

        AudioSource.PlayClipAtPoint(jumpSFX, myBoxCollider.transform.position);
        myAnimator.SetBool("isJumping", true);
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
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        if (!isAlive) { return; }

        isAlive = false;
        isGroundPounding = false;

        myAnimator.SetBool("isJumping", false);
        myAnimator.SetBool("isRunning", false);
        myAnimator.SetTrigger("Dying");

        AudioSource.PlayClipAtPoint(deathSFX, myBoxCollider.transform.position);

        myRigidBody.linearVelocity = deathKick;
        myBoxCollider.enabled = false;
        feetCollider.enabled = false;

        FindAnyObjectByType<GameSession>().ProcessPlayerDeath();
    }

    void CheckLanding()
    {
        if (!isAlive) { return; }

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) &&
            myRigidBody.linearVelocity.y <= 0f)
        {
            isGroundPounding = false;
            myAnimator.SetBool("isJumping", false);
        }
    }
}