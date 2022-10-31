using UnityEngine;

public class PlayerDynamic : MonoBehaviour
{
    Collider2D cl;
    Rigidbody2D rb;
    public float speed = 5;
    public float jumpForce = 20;
    public float scalex = 2f;
    public float scaley = 2f;
    public LayerMask layer;
    float jumpBuffer = 0f;
    bool isGrounded = false;
    bool pressed_jump = false;
    float gravity = -9.81f;

    float max_y_char = -20f;
    float max_y_line = -20f;

    void Start()
    {
        cl = gameObject.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
        isGroundedRay();
        if (Input.GetKeyDown(KeyCode.H))
        {
            Time.timeScale = Time.timeScale == 0.4f ? 1f : 0.4f;
        }

        manageJump();

    }

    void FixedUpdate()
    {
        Vector2 current = cl.bounds.center;
        int segments = 400;
        Vector3 next = new Vector3(current.x, current.y, 0) + new Vector3(rb.velocity.x, rb.velocity.y, 0) * Time.deltaTime;
        gravity = rb.velocity.y;
        for (int i = 0; i < segments; i++)
        {
            Color color = i % 2 == 0 ? Color.green : Color.red;
            Debug.DrawLine(current, next, color);
            current = next;
            next = new Vector3(current.x, current.y, 0) + new Vector3(rb.velocity.x * scalex, gravity * scaley, 0) * Time.deltaTime;
            gravity += -9.81f * Time.deltaTime;
            max_y_line = current.y > max_y_line ? current.y : max_y_line;
        }
        max_y_char = cl.bounds.center.y > max_y_char ? cl.bounds.center.y : max_y_char;
        Debug.DrawLine(new Vector2(cl.bounds.center.x -2f, max_y_line), new Vector2(cl.bounds.center.x + 2f, max_y_line), Color.white);
        Debug.DrawLine(new Vector2(cl.bounds.center.x -2f, max_y_char), new Vector2(cl.bounds.center.x + 2f, max_y_char), Color.black);

        float vel_x = Input.GetAxisRaw("Horizontal") * speed;

        rb.velocity = new Vector2(vel_x, rb.velocity.y);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;

        // manageJump
        if (isGrounded && jumpBuffer < 0.17f && pressed_jump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log(jumpBuffer);
        }
        pressed_jump = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private void manageJump()
    {
        if (!isGrounded)
        {
            jumpBuffer += 1.4f * Time.deltaTime;
            // Jump buffer
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpBuffer = 0f;
                pressed_jump = true;
            }
        }

        // Normal jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            max_y_char = -20f;
            max_y_line = -20f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    bool isGroundedRay()
    {
        Color color;
        float extended = .1f;
        RaycastHit2D rcHit = Physics2D.Raycast(cl.bounds.center, Vector2.down, cl.bounds.extents.y + extended, layer);
        color = rcHit.collider != null ? Color.green : Color.red;
        Debug.DrawLine(cl.bounds.center, new Vector2(cl.bounds.center.x, cl.bounds.center.y) + Vector2.down * (cl.bounds.extents.y + extended), color);
        return rcHit.collider != null;
    }

    private void OnGUI()
    {
        GUI.contentColor = Color.red;
        GUI.Label(new Rect(10, 130, 100, 20), string.Format("Buffer: {0:0.0}", jumpBuffer));
        GUI.Label(new Rect(10, 150, 100, 20), string.Format("pressed: {0:}", pressed_jump));
    }
}
