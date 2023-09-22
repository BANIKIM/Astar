using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Gamemanager pathfindingManager; // Gamemanager 스크립트 참조
    public GameObject Start_Pos;
    private List<Node> path; // A* 알고리즘을 통해 찾은 경로
    private int currentPathIndex = 0; // 현재 경로 인덱스

    private void Start()
    {
        path = new List<Node>();
        pathfindingManager.PathFinding(); // A* 알고리즘을 호출하여 경로 계산
        path = pathfindingManager.Final_nodeList;

    }

    private void Update()
    {
        // 경로가 없거나 모두 이동했다면 이동 중지
        if (path == null || currentPathIndex >= path.Count)
        {
            return;
        }

        // 현재 이동할 목표 지점
        Node targetNode = path[currentPathIndex];

        // 플레이어의 현재 위치
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);

        // 목표 지점까지의 방향 벡터
        Vector2 moveDirection = new Vector2(targetNode.x - currentPosition.x, targetNode.y - currentPosition.y).normalized;

        // 플레이어를 목표 지점으로 이동시킴
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 목표 지점에 도착했을 때 다음 경로 지점으로 이동
        if (Vector2.Distance(currentPosition, new Vector2(targetNode.x, targetNode.y)) < 0.1f)
        {
            currentPathIndex++;
        }
    }
   
    
}
