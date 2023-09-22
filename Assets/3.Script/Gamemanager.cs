using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class Node
{
    /*
     * 이 노드가 벽인지
     * 부모노드
     * x, y 좌표값
     * f h+g 
     * h 추정값 즉 가로 세로 장애물을 무시하여 목표까지의 거리 
     * g 시작으로 부터 이동했던 거리
     * 
     
     
     */

    public bool isWall;
    public Node Parentnode;
    public int x, y;
    public int G;
    public int H;
    public int F
    {
        get
        {
            return G + H;
        }
    }

    public Node(bool iswall, int x, int y)
    {
        this.isWall = iswall;
        this.x = x;
        this.y = y;
    }




}
public class Gamemanager : MonoBehaviour
{
    public GameObject Start_Pos, End_Pos;
    public GameObject Bottom_L_ob, Top_R_ob;


    [SerializeField] private Vector2Int bottomm_L, top_R, start_pos, end_pos;

    public List<Node> Final_nodeList;

    // 대각선을 이용 할 것인지?
    public bool AllowDigonal = true;
    // 코너를 가로질러 가지 않을 경우 이동중 수직 수평 장애물이 있는 지 판단
    public bool DontCrossCorner = true;

    private int SizeX, SizeY;
    Node[,] nodeArray;

    Node Startnode, Endnode, Curnode;

    List<Node> OpenList, CloseList;

    public GameObject player;

    public void Setposition()
    {
        bottomm_L = new Vector2Int((int)Bottom_L_ob.transform.position.x, (int)Bottom_L_ob.transform.position.y);
        top_R = new Vector2Int((int)Top_R_ob.transform.position.x, (int)Top_R_ob.transform.position.y);
        start_pos = new Vector2Int((int)Start_Pos.transform.position.x, (int)Start_Pos.transform.position.y);
        end_pos = new Vector2Int((int)End_Pos.transform.position.x, (int)End_Pos.transform.position.y);


    }

    public void butten_e()
    {
        Debug.Log("들어옴");
        SceneManager.LoadScene(0);
    }
    public void PathFinding()
    {
        Setposition();

        SizeX = top_R.x - bottomm_L.x + 1;
        SizeY = top_R.y - bottomm_L.y + 1;

        nodeArray = new Node[SizeX, SizeY];
        //모든 노드들을 담는 과정
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeY; j++)
            {
                bool iswall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomm_L.x, j + bottomm_L.y), 0.4f))
                {
                    if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Wall")))
                    {
                        iswall = true;
                    }
                }
                //node를 담아줍니다.
                nodeArray[i, j] = new Node(iswall, i + bottomm_L.x, j + bottomm_L.y);
            }
        }

        //시작과 끝 노드 열린리스트, 닫힌 리스트 최종경로 리스트 초기화 작업


        Startnode = nodeArray[start_pos.x - bottomm_L.x, start_pos.y - bottomm_L.y];//인덱스 번호 기준
        Endnode = nodeArray[end_pos.x - bottomm_L.x, end_pos.y - bottomm_L.y];


        OpenList = new List<Node>();
        CloseList = new List<Node>();
        Final_nodeList = new List<Node>();

        OpenList.Add(Startnode);

        while (OpenList.Count > 0)
        {
            Curnode = OpenList[0];

            for (int i = 0; i < OpenList.Count; i++)
            {
                //열린 리스트중 가장 F가 작고 F가 같다면 
                //H가 작은 것을 현재 노드로 설정
                if (OpenList[i].F <= Curnode.F && OpenList[i].H < Curnode.H)
                {
                    Curnode = OpenList[i];
                }
                //열린 리스트에서 닫힌 리스트로 옮기기
                OpenList.Remove(Curnode);
                CloseList.Add(Curnode);

                //curnode가 도착지라면 
                if (Curnode == Endnode)//도착이라는 뜻
                {
                    Node targetnode = Endnode;

                    while (targetnode != Startnode)
                    {
                        Final_nodeList.Add(targetnode);
                        targetnode = targetnode.Parentnode;
                    }
                    Final_nodeList.Add(Startnode);
                    Final_nodeList.Reverse();
                    return;

                }
                if (AllowDigonal)
                {
                    //대각선으로 움직이는 Cost 계산
                    // ↗↖↙↘
                    openListAdd(Curnode.x + 1, Curnode.y - 1);
                    openListAdd(Curnode.x - 1, Curnode.y + 1);
                    openListAdd(Curnode.x + 1, Curnode.y + 1);
                    openListAdd(Curnode.x - 1, Curnode.y - 1);


                }

                //직선으로 움직이는 Cost계산
                // ↑ → ↓ ←
                openListAdd(Curnode.x + 1, Curnode.y);
                openListAdd(Curnode.x - 1, Curnode.y);
                openListAdd(Curnode.x, Curnode.y + 1);
                openListAdd(Curnode.x, Curnode.y - 1);






            }//end of for

        }//end of while






    }
    public void openListAdd(int checkX, int checkY)
    {
        //조건
        //상하좌우 범위를 벗어나지 않고,
        //벽도 아니면서
        //닫힌리스트에 없어야한다.

        if (checkX >= bottomm_L.x && checkX < top_R.x + 1//x가 바텀레프트와 탑라이트 안에 있고
            && checkY >= bottomm_L.y && checkY < top_R.y + 1
            && !nodeArray[checkX - bottomm_L.x, checkY - bottomm_L.y].isWall
            && !CloseList.Contains(nodeArray[checkX - bottomm_L.x, checkY - bottomm_L.y]))
        {
            //대각선 허용시 (벽사이로는 통과가 되지 않음.)
            if (AllowDigonal)
            {
                if (nodeArray[Curnode.x - bottomm_L.x, checkY - bottomm_L.y].isWall &&
                    nodeArray[checkX - bottomm_L.x, Curnode.y - bottomm_L.y].isWall)
                {
                    return;
                }
            }
            //코너를 가로질러 가지 않을 시 (이동 중 수직 수평 장애물이 있으면 안됨.)
            if (DontCrossCorner)
            {
                if (nodeArray[Curnode.x - bottomm_L.x, checkY - bottomm_L.y].isWall || nodeArray[checkX - bottomm_L.x, Curnode.y - bottomm_L.y].isWall)
                {
                    return;
                }
            }

            //check하는 노드를 이웃 노드에 넣고 직선은 10 대각선 14

            Node neightdorNode = nodeArray[checkX - bottomm_L.x, checkY - bottomm_L.y];
            int movecost = Curnode.G + (Curnode.x - checkX == 0 || Curnode.y - checkY == 0 ? 10 : 14);

            //이동비용이 이웃노드 G보다 작거나, 또는 열린 리스트에 이웃노드가 없다면 

            if (movecost < neightdorNode.G || !OpenList.Contains(neightdorNode))
            {
                //G H parentnode를 설정 후 열린 리스트에  추가
                neightdorNode.G = movecost;
                neightdorNode.H = (Mathf.Abs(neightdorNode.x - Endnode.x) + Mathf.Abs(neightdorNode.y - Endnode.y)) * 10;


                neightdorNode.Parentnode = Curnode;

                OpenList.Add(neightdorNode);

            }
        }
    }

    private void OnDrawGizmos()
    {
        //씬 뷰의 Debug용도로 그림을 그릴 때 사용합니다.

        if (Final_nodeList != null)
        {

            for (int i = 0; i < Final_nodeList.Count - 1; i++)
            {

                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector2(Final_nodeList[i].x, Final_nodeList[i].y), new Vector2(Final_nodeList[i + 1].x, Final_nodeList[i + 1].y));
            }
        }
    }
    
    public void Player_start()
    {
        if(Final_nodeList.Count>0)
        {
            StartCoroutine(PlayerMove_co());
        }
        else
        {
            Debug.Log("길 찾기 부터 먼저 하세욥");
        }
    }
    public void Player_Reset()
    {
        Vector3 startpos_v3 = new Vector3(start_pos.x, start_pos.y, 0);
        player.transform.position = startpos_v3;
    }
    private IEnumerator PlayerMove_co()//움직이는 코루틴
    {
        for (int i = 0; i < Final_nodeList.Count;)
        {
            Vector3 Targerpos = new Vector3(Final_nodeList[i].x, Final_nodeList[i].y, 0);

            while (Vector3.Distance(player.transform.position, Targerpos) >= 0.01f)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, Targerpos, 5f * Time.deltaTime);
                yield return null;
            }
            player.transform.position = Targerpos;
            i++;
            yield return null;

        }


    }

}
