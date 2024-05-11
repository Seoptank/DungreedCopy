using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 승석아 이 시발려나?????
public class ChangeCursor : MonoBehaviour
{
    [SerializeField]
    private Texture2D       attackCursorImg;
    [SerializeField]
    private Texture2D       attackCursorImg2;
    [SerializeField]
    private Texture2D       originCursorImg;
    
    private PlayerController player;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        Cursor.SetCursor(originCursorImg, new Vector2(originCursorImg.width *0.5f, originCursorImg.height * 0.5f), CursorMode.Auto);
    
    }
    private void Update()
    {
        if(!player.dontMovePlayer)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(attackCursorImg2, new Vector2(attackCursorImg2.width * 0.5f, attackCursorImg2.height * 0.5f), CursorMode.Auto);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(attackCursorImg, new Vector2(attackCursorImg.width * 0.5f, attackCursorImg.height * 0.5f), CursorMode.Auto);
            }
        }                                                              
        else                                                           
        {      
            Cursor.SetCursor(originCursorImg, new Vector2(originCursorImg.width * 0.5f, originCursorImg.height * 0.5f), CursorMode.Auto);
        }
    }
}
