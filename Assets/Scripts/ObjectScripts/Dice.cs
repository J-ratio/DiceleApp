using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    bool select_tag;
    Vector3 drag_offset;
    public Vector2 initialPos;
    public Vector2Int slotPos;
    public int slotsolution;
    public int diceNumber;

    float speed = 1000f;
    Vector3 delta = new Vector3 (0.1f,0.1f,0);
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        Invoke("StoreInitialPos",0.2f);
    }

    public Vector2 GetInitialPos(){
        return initialPos;
    }

    void StoreInitialPos()
    {
        initialPos = new Vector2(transform.position.x,transform.position.y);
    }



    void OnMouseDown()
    {

        if(!select_tag)
        {
            drag_offset = transform.position - GetMousePos();
            select_tag = true;
            transform.localScale +=delta;
        }
        GetComponent<SpriteRenderer>().sortingOrder++;
    }

    void OnMouseDrag()
    {

        if(select_tag)
            transform.position = Vector3.MoveTowards(transform.position,GetMousePos() + drag_offset,speed*Time.deltaTime);
    }

    void OnMouseUp()
    {

        transform.localScale -= delta;
        select_tag = false;
        ActionEvents.swapDice(this);
        GetComponent<SpriteRenderer>().sortingOrder--;
    }

    Vector3 GetMousePos()
    {

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    } 

    void OnDisable()
    {
    }
}
