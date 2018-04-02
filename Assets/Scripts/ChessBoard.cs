using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChessBoard : MonoBehaviour {

	// 7种黑色棋子的预置资源
	public GameObject prefabBlackBing = null;
	public GameObject prefabBlackJu = null;
	public GameObject prefabBlackMa = null;
	public GameObject prefabBlackPao = null;
	public GameObject prefabBlackXiang = null;
	public GameObject prefabBlackShi = null;
	public GameObject prefabBlackShuai = null;

	public GameObject[] chessPieces = new GameObject[32];
//	public GameObject[] blackJu = new GameObject[2];
//	public GameObject[] blackMa = new GameObject[2];
//	public GameObject[] blackPao = new GameObject[2];
//	public GameObject[] blackXiang = new GameObject[2];
//	public GameObject[] blackShi = new GameObject[2];
//	public GameObject blackShuai = null;

	public GameObject pos_1_1 = null; // 列1 行1
	public GameObject pos_9_10 = null; // 列9 行10
	public float m_z = 0f;
	public float m_colBegin, m_colEnd, m_rowBegin, m_rowEnd, m_colDelta, m_rowDelta, m_rotateX, m_rotateY;

	public GameObject CreateChessPieces(int col, int row, string type) {
		float x = m_colBegin - m_colDelta * (col - 1);
		float y = m_rowBegin + m_rowDelta * (row - 1);
		GameObject prefabTemp = null;
		switch (type) {
		case "blackBing":
			prefabTemp = prefabBlackBing;
			break;
		case "blackJu":
			prefabTemp = prefabBlackJu;
			break;
		case "blackMa":
			prefabTemp = prefabBlackMa;
			break;
		case "blackPao":
			prefabTemp = prefabBlackPao;
			break;
		case "blackXiang":
			prefabTemp = prefabBlackXiang;
			break;
		case "blackShi":
			prefabTemp = prefabBlackShi;
			break;
		case "blackShuai":
			prefabTemp = prefabBlackShuai;
			break;
		}
		GameObject chessPiece = Instantiate (prefabTemp, new Vector3(x, y, m_z), prefabTemp.transform.rotation);
		return chessPiece;
	}

	// Use this for initialization
	void Start () {
		m_colBegin = pos_1_1.transform.position.x;
		m_rowBegin = pos_1_1.transform.position.y;
		m_colEnd = pos_9_10.transform.position.x;
		m_rowEnd = pos_9_10.transform.position.y;
		m_colDelta = (m_colBegin - m_colEnd) / 8;
		m_rowDelta = (m_rowEnd - m_rowBegin) / 9;
		m_rotateX = (m_colEnd + m_colBegin) / 2;
		m_rotateY = (m_rowEnd + m_rowBegin) / 2;
		m_z = pos_1_1.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// my function
	public void InitChessPieces() {
		// 初始化黑子
		chessPieces[0] = CreateChessPieces (1, 4, "blackBing");
		chessPieces[1] = CreateChessPieces (3, 4, "blackBing");
		chessPieces[2] = CreateChessPieces (5, 4, "blackBing");
		chessPieces[3] = CreateChessPieces (7, 4, "blackBing");
		chessPieces[4] = CreateChessPieces (9, 4, "blackBing");
		chessPieces[5] = CreateChessPieces (1, 1, "blackJu");
		chessPieces[6] = CreateChessPieces (9, 1, "blackJu");
		chessPieces[7] = CreateChessPieces (2, 1, "blackMa");
		chessPieces[8] = CreateChessPieces (8, 1, "blackMa");
		chessPieces[9] = CreateChessPieces (2, 3, "blackPao");
		chessPieces[10] = CreateChessPieces (8, 3, "blackPao");
		chessPieces[11] = CreateChessPieces (3, 1, "blackXiang");
		chessPieces[12] = CreateChessPieces (7, 1, "blackXiang");
		chessPieces[13] = CreateChessPieces (4, 1, "blackShi");
		chessPieces[14] = CreateChessPieces (6, 1, "blackShi");
		chessPieces[15] = CreateChessPieces (5, 1, "blackShuai");
		// 初始化红子
		chessPieces[16] = CreateChessPieces (1, 4, "blackBing");
		chessPieces[17] = CreateChessPieces (3, 4, "blackBing");
		chessPieces[18] = CreateChessPieces (5, 4, "blackBing");
		chessPieces[19] = CreateChessPieces (7, 4, "blackBing");
		chessPieces[20] = CreateChessPieces (9, 4, "blackBing");
		chessPieces[21] = CreateChessPieces (1, 1, "blackJu");
		chessPieces[22] = CreateChessPieces (9, 1, "blackJu");
		chessPieces[23] = CreateChessPieces (2, 1, "blackMa");
		chessPieces[24] = CreateChessPieces (8, 1, "blackMa");
		chessPieces[25] = CreateChessPieces (2, 3, "blackPao");
		chessPieces[26] = CreateChessPieces (8, 3, "blackPao");
		chessPieces[27] = CreateChessPieces (3, 1, "blackXiang");
		chessPieces[28] = CreateChessPieces (7, 1, "blackXiang");
		chessPieces[29] = CreateChessPieces (4, 1, "blackShi");
		chessPieces[30] = CreateChessPieces (6, 1, "blackShi");
		chessPieces[31] = CreateChessPieces (5, 1, "blackShuai");
	}

	public void ReverseChessPieces() {
		var obs = GameObject.FindGameObjectsWithTag ("ChessPiecesTag");
		foreach (GameObject ob in obs) {
			ob.transform.RotateAround (new Vector3 (m_rotateX, m_rotateY, m_z), new Vector3 (0, 0, 1), 180);
			ob.transform.Rotate (new Vector3 (0, 0, 180));
		}
	}

	// 如果是服务器，则黑子朝向不变
	public void InitServerChessPieces() {
		print ("初始化服务器端的棋子");
		InitChessPieces ();
	}

	public void InitClientChessPieces() {
		print ("初始化客户端的棋子");
		InitChessPieces ();
		ReverseChessPieces ();
	}

}
