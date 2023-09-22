using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class Node
{
    /*
     * �� ��尡 ������
     * �θ���
     * x, y ��ǥ��
     * f h+g 
     * h ������ �� ���� ���� ��ֹ��� �����Ͽ� ��ǥ������ �Ÿ� 
     * g �������� ���� �̵��ߴ� �Ÿ�
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

    // �밢���� �̿� �� ������?
    public bool AllowDigonal = true;
    // �ڳʸ� �������� ���� ���� ��� �̵��� ���� ���� ��ֹ��� �ִ� �� �Ǵ�
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
        Debug.Log("����");
        SceneManager.LoadScene(0);
    }
    public void PathFinding()
    {
        Setposition();

        SizeX = top_R.x - bottomm_L.x + 1;
        SizeY = top_R.y - bottomm_L.y + 1;

        nodeArray = new Node[SizeX, SizeY];
        //��� ������ ��� ����
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
                //node�� ����ݴϴ�.
                nodeArray[i, j] = new Node(iswall, i + bottomm_L.x, j + bottomm_L.y);
            }
        }

        //���۰� �� ��� ��������Ʈ, ���� ����Ʈ ������� ����Ʈ �ʱ�ȭ �۾�


        Startnode = nodeArray[start_pos.x - bottomm_L.x, start_pos.y - bottomm_L.y];//�ε��� ��ȣ ����
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
                //���� ����Ʈ�� ���� F�� �۰� F�� ���ٸ� 
                //H�� ���� ���� ���� ���� ����
                if (OpenList[i].F <= Curnode.F && OpenList[i].H < Curnode.H)
                {
                    Curnode = OpenList[i];
                }
                //���� ����Ʈ���� ���� ����Ʈ�� �ű��
                OpenList.Remove(Curnode);
                CloseList.Add(Curnode);

                //curnode�� ��������� 
                if (Curnode == Endnode)//�����̶�� ��
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
                    //�밢������ �����̴� Cost ���
                    // �֢آע�
                    openListAdd(Curnode.x + 1, Curnode.y - 1);
                    openListAdd(Curnode.x - 1, Curnode.y + 1);
                    openListAdd(Curnode.x + 1, Curnode.y + 1);
                    openListAdd(Curnode.x - 1, Curnode.y - 1);


                }

                //�������� �����̴� Cost���
                // �� �� �� ��
                openListAdd(Curnode.x + 1, Curnode.y);
                openListAdd(Curnode.x - 1, Curnode.y);
                openListAdd(Curnode.x, Curnode.y + 1);
                openListAdd(Curnode.x, Curnode.y - 1);






            }//end of for

        }//end of while






    }
    public void openListAdd(int checkX, int checkY)
    {
        //����
        //�����¿� ������ ����� �ʰ�,
        //���� �ƴϸ鼭
        //��������Ʈ�� ������Ѵ�.

        if (checkX >= bottomm_L.x && checkX < top_R.x + 1//x�� ���ҷ���Ʈ�� ž����Ʈ �ȿ� �ְ�
            && checkY >= bottomm_L.y && checkY < top_R.y + 1
            && !nodeArray[checkX - bottomm_L.x, checkY - bottomm_L.y].isWall
            && !CloseList.Contains(nodeArray[checkX - bottomm_L.x, checkY - bottomm_L.y]))
        {
            //�밢�� ���� (�����̷δ� ����� ���� ����.)
            if (AllowDigonal)
            {
                if (nodeArray[Curnode.x - bottomm_L.x, checkY - bottomm_L.y].isWall &&
                    nodeArray[checkX - bottomm_L.x, Curnode.y - bottomm_L.y].isWall)
                {
                    return;
                }
            }
            //�ڳʸ� �������� ���� ���� �� (�̵� �� ���� ���� ��ֹ��� ������ �ȵ�.)
            if (DontCrossCorner)
            {
                if (nodeArray[Curnode.x - bottomm_L.x, checkY - bottomm_L.y].isWall || nodeArray[checkX - bottomm_L.x, Curnode.y - bottomm_L.y].isWall)
                {
                    return;
                }
            }

            //check�ϴ� ��带 �̿� ��忡 �ְ� ������ 10 �밢�� 14

            Node neightdorNode = nodeArray[checkX - bottomm_L.x, checkY - bottomm_L.y];
            int movecost = Curnode.G + (Curnode.x - checkX == 0 || Curnode.y - checkY == 0 ? 10 : 14);

            //�̵������ �̿���� G���� �۰ų�, �Ǵ� ���� ����Ʈ�� �̿���尡 ���ٸ� 

            if (movecost < neightdorNode.G || !OpenList.Contains(neightdorNode))
            {
                //G H parentnode�� ���� �� ���� ����Ʈ��  �߰�
                neightdorNode.G = movecost;
                neightdorNode.H = (Mathf.Abs(neightdorNode.x - Endnode.x) + Mathf.Abs(neightdorNode.y - Endnode.y)) * 10;


                neightdorNode.Parentnode = Curnode;

                OpenList.Add(neightdorNode);

            }
        }
    }

    private void OnDrawGizmos()
    {
        //�� ���� Debug�뵵�� �׸��� �׸� �� ����մϴ�.

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
            Debug.Log("�� ã�� ���� ���� �ϼ���");
        }
    }
    public void Player_Reset()
    {
        Vector3 startpos_v3 = new Vector3(start_pos.x, start_pos.y, 0);
        player.transform.position = startpos_v3;
    }
    private IEnumerator PlayerMove_co()//�����̴� �ڷ�ƾ
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
