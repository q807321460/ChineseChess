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
	// 7中红色棋子的预置资源
	public GameObject prefabRedBing = null;
	public GameObject prefabRedJu = null;
	public GameObject prefabRedMa = null;
	public GameObject prefabRedPao = null;
	public GameObject prefabRedXiang = null;
	public GameObject prefabRedShi = null;
	public GameObject prefabRedShuai = null;

	public GameObject[] chessPieces = new GameObject[32]; // 32个棋子
	public GameObject pos_1_1 = null; // 列1 行1
	public GameObject pos_9_10 = null; // 列9 行10

	//public GameManager m_gameManager = null;

	public float m_piecesZ = -2.0f;
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
			case "redBing":
				prefabTemp = prefabRedBing;
				break;
			case "redJu":
				prefabTemp = prefabRedJu;
				break;
			case "redMa":
				prefabTemp = prefabRedMa;
				break;
			case "redPao":
				prefabTemp = prefabRedPao;
				break;
			case "redXiang":
				prefabTemp = prefabRedXiang;
				break;
			case "redShi":
				prefabTemp = prefabRedShi;
				break;
			case "redShuai":
				prefabTemp = prefabRedShuai;
				break;
		}
		prefabTemp.GetComponent<ChessPiece>().m_col = col;
		prefabTemp.GetComponent<ChessPiece>().m_row = row;
		prefabTemp.GetComponent<ChessPiece>().m_isSelected = false;
		GameObject chessPiece = Instantiate(prefabTemp, new Vector3(x, y, m_piecesZ), prefabTemp.transform.rotation);
		return chessPiece;
	}

	// Use this for initialization
	void Start() {
		m_colBegin = pos_1_1.transform.position.x;
		m_rowBegin = pos_1_1.transform.position.y;
		m_colEnd = pos_9_10.transform.position.x;
		m_rowEnd = pos_9_10.transform.position.y;
		m_colDelta = (m_colBegin - m_colEnd) / 8;
		m_rowDelta = (m_rowEnd - m_rowBegin) / 9;
		m_rotateX = (m_colEnd + m_colBegin) / 2;
		m_rotateY = (m_rowEnd + m_rowBegin) / 2;
		// 把这些参数传递给游戏管理类，以简化代码
		GameManager.instance.SendMessage("InitChessBoardParam", SendMessageOptions.DontRequireReceiver);
	}

	// Update is called once per frame
	void Update() {

	}

	// 初始化各种棋子
	public void InitChessPieces() {
		// 初始化黑子
		chessPieces[0] = CreateChessPieces(1, 4, "blackBing");
		chessPieces[1] = CreateChessPieces(3, 4, "blackBing");
		chessPieces[2] = CreateChessPieces(5, 4, "blackBing");
		chessPieces[3] = CreateChessPieces(7, 4, "blackBing");
		chessPieces[4] = CreateChessPieces(9, 4, "blackBing");
		chessPieces[5] = CreateChessPieces(1, 1, "blackJu");
		chessPieces[6] = CreateChessPieces(9, 1, "blackJu");
		chessPieces[7] = CreateChessPieces(2, 1, "blackMa");
		chessPieces[8] = CreateChessPieces(8, 1, "blackMa");
		chessPieces[9] = CreateChessPieces(2, 3, "blackPao");
		chessPieces[10] = CreateChessPieces(8, 3, "blackPao");
		chessPieces[11] = CreateChessPieces(3, 1, "blackXiang");
		chessPieces[12] = CreateChessPieces(7, 1, "blackXiang");
		chessPieces[13] = CreateChessPieces(4, 1, "blackShi");
		chessPieces[14] = CreateChessPieces(6, 1, "blackShi");
		chessPieces[15] = CreateChessPieces(5, 1, "blackShuai");
		// 初始化红子
		chessPieces[16] = CreateChessPieces(1, 7, "redBing");
		chessPieces[17] = CreateChessPieces(3, 7, "redBing");
		chessPieces[18] = CreateChessPieces(5, 7, "redBing");
		chessPieces[19] = CreateChessPieces(7, 7, "redBing");
		chessPieces[20] = CreateChessPieces(9, 7, "redBing");
		chessPieces[21] = CreateChessPieces(1, 10, "redJu");
		chessPieces[22] = CreateChessPieces(9, 10, "redJu");
		chessPieces[23] = CreateChessPieces(2, 10, "redMa");
		chessPieces[24] = CreateChessPieces(8, 10, "redMa");
		chessPieces[25] = CreateChessPieces(2, 8, "redPao");
		chessPieces[26] = CreateChessPieces(8, 8, "redPao");
		chessPieces[27] = CreateChessPieces(3, 10, "redXiang");
		chessPieces[28] = CreateChessPieces(7, 10, "redXiang");
		chessPieces[29] = CreateChessPieces(4, 10, "redShi");
		chessPieces[30] = CreateChessPieces(6, 10, "redShi");
		chessPieces[31] = CreateChessPieces(5, 10, "redShuai");
	}

	// 翻转棋盘
	public void ReverseChessPieces() {
		foreach (GameObject piece in chessPieces) {
			piece.transform.RotateAround(new Vector3(m_rotateX, m_rotateY, m_piecesZ), new Vector3(0, 0, 1), 180);
			piece.transform.Rotate(new Vector3(0, 0, 180));
			piece.GetComponent<ChessPiece>().m_col = 10 - piece.GetComponent<ChessPiece>().m_col;
			piece.GetComponent<ChessPiece>().m_row = 11 - piece.GetComponent<ChessPiece>().m_row;
		}
	}

	// 如果是黑方，则朝向不变
	public void InitBlackChessPieces() {
		InitChessPieces();
		//var reds = GameObject.FindGameObjectsWithTag("red");
		//foreach (GameObject red in reds) {
		//	red.GetComponent<ChessPiece>().m_isMine = false;
		//}
	}

	// 如果是红方，则翻转所有棋子
	public void InitRedChessPieces() {
		InitChessPieces();
		ReverseChessPieces();
	}

	// 收到断开websocket的消息时
	public void OnDisconnect() {
		foreach (GameObject go in chessPieces) {
			Destroy(go);
		}
	}

}
