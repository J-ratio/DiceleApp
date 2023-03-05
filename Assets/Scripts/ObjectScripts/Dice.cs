using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    internal bool select_tag;
    Vector3 drag_offset;
    public Vector2 initialPos;
    public Vector2Int slotPos;
    public int slotsolution;
    public int diceNumber;

    float speed = 1000f;
    Vector3 delta = new Vector3(0.1f, 0.1f, 0);
    Camera cam;
    public bool matched;

    //Added by charan
    public bool isTutorial = false;
    private GamePlaySwapping playParticle;
    private Board board;
    public ColorType colorType;
    private Vector3 targetScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float maxAlpha = 0.5f;
    Vector3 normalScale;
    [SerializeField] float highlightSpeed = 5;
    [SerializeField] float alphaSpeed = 5;
    static bool mouse;

    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    //....
    void Awake()
    {
        cam = Camera.main;
        Invoke("StoreInitialPos", 0.2f);
        normalScale = transform.localScale;
        targetScale = normalScale;
    }

    internal void Init(GamePlaySwapping l_playParticle, Board l_board)
    {
        playParticle = l_playParticle;
        board = l_board;
    }
    public Vector2 GetInitialPos()
    {
        return initialPos;
    }

    void StoreInitialPos()
    {
        initialPos = new Vector2(transform.position.x, transform.position.y);
    }
    
    //Added by charan
    private IEnumerator ScaleObject()
    {
        while (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * highlightSpeed);
            yield return null;
        }
    }

    private IEnumerator ScaleAlpha()
    {
        Image parent = GetComponentInParent<Image>();
        Color color = spriteRenderer.color;
        parent.enabled = false;
        color.a = 0f;
        spriteRenderer.color = color;
        while (color.a != 1)
        {
            color = spriteRenderer.color;
            color.a = Mathf.Lerp(color.a,1,Time.deltaTime* alphaSpeed);
            if (color.a > 0.9f) color.a = 1;
            spriteRenderer.color = color;
            yield return null;
        }
        parent.enabled = true;
    }
    private void OnMouseEnter()
    {
        if (!mouse) return;

        if (!select_tag)
        {
            targetScale = Vector3.one * maxScale;
            StopCoroutine("ScaleObject");
            StartCoroutine("ScaleObject");
        }

    }
    private void OnMouseExit()
    {
        targetScale = normalScale;

        StopCoroutine("ScaleObject");
        StartCoroutine("ScaleObject");
    }
    //....
    void OnMouseDown()
    {
        mouse = true;

        if (playParticle.isSwapping) return;

        if (isTutorial) return;

        if (!select_tag)
        {
            drag_offset = transform.position - GetMousePos();
            select_tag = true;
            boxCollider.enabled = false;
            spriteRenderer.sortingOrder++;
            //Added by charan
            if (!board.isTutorial)
                spriteRenderer.sortingOrder = 5;
            //....
            //transform.localScale += delta;
        }
    }

    private void ChangeAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    void OnMouseDrag()
    {
        if (playParticle.isSwapping) return;

        if (isTutorial) return;

        if (select_tag)
        {
            if (!board.isTutorial)
                spriteRenderer.sortingOrder = 5;//Added by charan

            transform.position = Vector3.MoveTowards(transform.position, GetMousePos() + drag_offset, speed * Time.deltaTime);

            ChangeAlpha(maxAlpha);//Added by charan
        }
    }

    void OnMouseUp()
    {
        mouse = false;
        //Added by charan
        if (!board.isTutorial)
            spriteRenderer.sortingOrder = 4;

        if (colorType != ColorType.Green)
            boxCollider.enabled = true;

        if (playParticle.isSwapping) return;

        if (isTutorial) return;
        
        if (board.CheckSwapDice(this)) return;
        //....
        //transform.localScale -= delta;
        select_tag = false;
        ChangeAlpha(1);//Added by charan
        ActionEvents.swapDice(this);

        //spriteRenderer.sortingOrder = 4;
    }

    Vector3 GetMousePos()
    {

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
    //Added by charan
    internal void ResetScale()
    {
        transform.localScale = normalScale;
        StartCoroutine("ScaleAlpha");
    }
    //...
}
