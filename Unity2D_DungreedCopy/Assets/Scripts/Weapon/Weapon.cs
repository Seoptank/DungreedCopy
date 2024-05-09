using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;

    public Transform player;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }
    // Update is called once per frame
    void Update()
    {
        PlayerController player = PlayerController.instance;

        if(!player.dontMovePlayer && !player.isDie && !PlayerDungeonData.instance.isMoving)
        {
            UpdateSight();
        }
    }

    void UpdateSight()
    {
        // ���콺 ��ġ�� �����ͼ� ���� ��ǥ�� ��ȯ
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �÷��̾� ��ġ���� ���콺 ��ġ������ ���� ����
        Vector3 directionToMouse = mousePosition - player.position;

        // ������Ʈ�� ���콺 �������� z�� ȸ��
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // ���콺 x��ǥ�� �о� scale���� -1�� �ٲپ� �̹��� ����
        Vector2 scale = transform.localScale;
        if (directionToMouse.x < 0)
        {
            scale.y = -1;
        }
        else
        {
            scale.y = 1;
        }
        transform.localScale = scale;
    }
}
