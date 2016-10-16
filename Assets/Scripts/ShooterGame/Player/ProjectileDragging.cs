using UnityEngine;

public class ProjectileDragging : MonoBehaviour
{
    public float maxStretch = 3.0f;
    public LineRenderer playerLineRenderer;

    private SpringJoint2D spring;
    private Transform catapult;
    private Ray rayToMouse;
    private Ray leftCatapultToProjectile;
    private float maxStretchSqr;
    private bool clickedOn;
    private Vector2 prevVelocity;


    void Awake()
    {
        Debug.Log("Start");
        spring = GetComponent<SpringJoint2D>();
        catapult = spring.connectedBody.transform;
    }

    void Start()
    {
        Debug.Log("Start");
        LineRendererSetup();
        rayToMouse = new Ray(catapult.position, Vector3.zero);
        leftCatapultToProjectile = new Ray(playerLineRenderer.transform.position, Vector3.zero);
        maxStretchSqr = maxStretch * maxStretch;
    }

    void Update()
    {
        if (clickedOn)
        {
            Debug.Log("Clicked On");
            Dragging();
        }

        if (spring != null)
        {
            if (!GetComponent<Rigidbody2D>().isKinematic && prevVelocity.sqrMagnitude > GetComponent<Rigidbody2D>().velocity.sqrMagnitude)
            {
                Destroy(spring);
                GetComponent<Rigidbody2D>().velocity = prevVelocity;
            }

            if (!clickedOn)
                prevVelocity = GetComponent<Rigidbody2D>().velocity;

            LineRendererUpdate();

        }
        else
        {
            Debug.Log("Not Dragging");
            playerLineRenderer.enabled = false;
        }
    }

    void LineRendererSetup()
    {
        playerLineRenderer.SetPosition(0, playerLineRenderer.transform.position);
        playerLineRenderer.sortingLayerName = "Foreground";
        playerLineRenderer.sortingOrder = 3;
    }

    void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        spring.enabled = false;
        clickedOn = true;
    }

    void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
        spring.enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        clickedOn = false;
    }

    void Dragging()
    {
        Debug.Log("Dragging");
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 catapultToMouse = mouseWorldPoint - catapult.position;

        if (catapultToMouse.sqrMagnitude > maxStretchSqr)
        {
            rayToMouse.direction = catapultToMouse;
            mouseWorldPoint = rayToMouse.GetPoint(maxStretch);
        }

        mouseWorldPoint.z = 0f;
        transform.position = mouseWorldPoint;
    }

    void LineRendererUpdate()
    {
        Vector2 catapultToProjectile = transform.position - playerLineRenderer.transform.position;
        leftCatapultToProjectile.direction = catapultToProjectile;
        Vector3 holdPoint = leftCatapultToProjectile.GetPoint(catapultToProjectile.magnitude);
        playerLineRenderer.SetPosition(1, holdPoint);
    }
}
