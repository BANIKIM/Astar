using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Gamemanager pathfindingManager; // Gamemanager ��ũ��Ʈ ����
    public GameObject Start_Pos;
    private List<Node> path; // A* �˰����� ���� ã�� ���
    private int currentPathIndex = 0; // ���� ��� �ε���

    private void Start()
    {
        path = new List<Node>();
        pathfindingManager.PathFinding(); // A* �˰����� ȣ���Ͽ� ��� ���
        path = pathfindingManager.Final_nodeList;

    }

    private void Update()
    {
        // ��ΰ� ���ų� ��� �̵��ߴٸ� �̵� ����
        if (path == null || currentPathIndex >= path.Count)
        {
            return;
        }

        // ���� �̵��� ��ǥ ����
        Node targetNode = path[currentPathIndex];

        // �÷��̾��� ���� ��ġ
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);

        // ��ǥ ���������� ���� ����
        Vector2 moveDirection = new Vector2(targetNode.x - currentPosition.x, targetNode.y - currentPosition.y).normalized;

        // �÷��̾ ��ǥ �������� �̵���Ŵ
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // ��ǥ ������ �������� �� ���� ��� �������� �̵�
        if (Vector2.Distance(currentPosition, new Vector2(targetNode.x, targetNode.y)) < 0.1f)
        {
            currentPathIndex++;
        }
    }
   
    
}
