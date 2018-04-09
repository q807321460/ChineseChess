using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class GameManager : MonoBehaviour {

	// 未准备，红方，黑方
	public enum PLAYER_TYPE {
		NONE,
		RED,
		BLACK
	};
	public PLAYER_TYPE m_playerType = PLAYER_TYPE.NONE;
	// 初始化为未准备状态
	public GameObject m_chessBoard = null;
	public WebSocket m_ws = null;
	public string m_url = null;
	// 47.100.97.154
	public InputField m_txtPlayerName = null;
	public Button m_btnConnect = null;
	public Button m_btnDisconnect = null;
	// 消息堆栈
	public Stack<Dictionary<string, object>> m_stackMessage = new Stack<Dictionary<string, object>>();
	// 是否是当前用户的回合
	public bool m_isMyTurn = false;
	// 当前有没有选择一个自己的棋子
	public bool m_isSelecting = false;
	// 当前选中的棋子
	public ChessPiece m_selectedPiece = null;
	public float m_colBegin, m_colEnd, m_rowBegin, m_rowEnd, m_colDelta, m_rowDelta;

	public static GameManager instance;
	void Awake() {
		instance = this;
	}

	// 将棋盘对象的参数复制到这里
	public void InitChessBoardParam() {
		m_colBegin = m_chessBoard.GetComponent<ChessBoard>().m_colBegin;
		m_colEnd = m_chessBoard.GetComponent<ChessBoard>().m_colEnd;
		m_colDelta = m_chessBoard.GetComponent<ChessBoard>().m_colDelta;
		m_rowBegin = m_chessBoard.GetComponent<ChessBoard>().m_rowBegin;
		m_rowEnd = m_chessBoard.GetComponent<ChessBoard>().m_rowEnd;
		m_rowDelta = m_chessBoard.GetComponent<ChessBoard>().m_rowDelta;
	}

	public void OnConnectClicked() {
		if (m_ws != null && m_ws.IsAlive) {
			print("已连入，不能重新连入");
			return;
		}
		string url = "ws://192.168.0.108:8088/ALiCloudServer/ChineseChess/" + m_txtPlayerName.text;
		m_ws = new WebSocket(url);
		m_ws.OnOpen += (sender, e) => {
			print("打开websocket");
		};
		m_ws.OnMessage += (sender, e) => {
			OnMessage(e.Data);
		};
		m_ws.OnError += (sender, e) => {
			print("websocket错误 : " + e.Message);
		};
		m_ws.OnClose += (sender, e) => {
			print("关闭websocket");
		};
		m_ws.Connect();
	}

	public void OnDisconnectClicked() {
		m_ws.Close();
		m_chessBoard.SendMessage("OnDisconnect", SendMessageOptions.DontRequireReceiver);
		m_playerType = PLAYER_TYPE.NONE;
		m_isMyTurn = false;
	}

	void Start() {
		m_txtPlayerName.text = "1";
		m_btnConnect.onClick.AddListener(OnConnectClicked);
		m_btnDisconnect.onClick.AddListener(OnDisconnectClicked);
	}

	void Update() {
		DoMessage();
		HandleInput();
	}

	// 处理用户的输入，一般是点击棋盘上的某个位置，或者是棋子
	public void HandleInput() {
		// 如果没有开始游戏，则不处理触摸事件
		if (m_playerType == PLAYER_TYPE.NONE)
			return;
		// 如果不是自己的回合，则不处理触摸事件
		if (!m_isMyTurn)
			return;
		if (Input.GetMouseButtonUp(0)) {
			Vector3 tapPosition = Vector3.zero;
#if UNITY_ANDROID
			Touch T = Input.GetTouch (0);
			if (T.phase == TouchPhase.Began)
				tapPosition = T.position;
			else
				return;
#else
			tapPosition = Input.mousePosition;
#endif
			// 如果是首次选择，则获取点击到的棋子
			if (!m_isSelecting) {
				Ray ray = Camera.main.ScreenPointToRay(tapPosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					m_selectedPiece = hit.collider.gameObject.GetComponent<ChessPiece>();
					print("点击的棋子颜色 " + m_selectedPiece.m_color + "类型 " + m_selectedPiece.m_type);
					// 这里不用判断棋子的归属，所有玩家都可以移动所有的棋子
					m_isSelecting = true;
					m_selectedPiece.m_isSelected = true;
				}
			} else { // 某则选中某个位置，将当前选中的子移动到指定的位置去
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(tapPosition);
				int col = CalcCol(worldPosition.x);
				int row = CalcRow(worldPosition.y);
				// 判断当前选中的位置是否合法
				if (!IsValidPosition(col, row))
					return;
				// 判断当前选中的位置是否有其他棋子
				var pieces = m_chessBoard.GetComponent<ChessBoard>().chessPieces;
				foreach (GameObject obj in pieces) {
					ChessPiece piece = obj.GetComponent<ChessPiece>();
					if (piece.m_col == col && piece.m_row == row) {
						if (m_selectedPiece.m_color == piece.m_color)
							return;
						else
							Destroy(obj);
						break;
					}
				}
				m_selectedPiece.transform.position = new Vector3(CalcX(col), CalcY(row), -2f); // 在本地移动棋子
				m_selectedPiece.GetComponent<ChessPiece>().m_col = col;
				m_selectedPiece.GetComponent<ChessPiece>().m_row = row;
				m_isSelecting = false; // 不再处于选中棋子状态
				m_isMyTurn = false; // 不再是自己的回合了
				// 计算当前选中棋子的index值
				int index = -1; 
				var obs = m_chessBoard.GetComponent<ChessBoard>().chessPieces;
				for (int i = 0;i < 32; i++) {
					var ob = obs[i];
					if (ob != null && ob.GetComponent<ChessPiece>().m_isSelected) {
						index = i;
						ob.GetComponent<ChessPiece>().m_isSelected = false;
						break;
					}
				}
				// 向服务器发送一个移动请求，并转告另一个棋子
				SendMoveMessage(index, col, row);
			}
		}
	}


	// 处理服务器发送来的所有消息（也可能有本地消息）
	public void DoMessage() {
		if (m_stackMessage.Count == 0)
			return;
		Dictionary<string, object> dic = m_stackMessage.Pop();
		string sType = dic["message_type"].ToString();
		print("接收到的数据类型为：" + sType);
		switch (sType) {
			case "player_type": // 玩家类型设置
				string player_type = dic["message_detail"].ToString();
				print("玩家设定的类型为：" + player_type);
				if (player_type == "red") {
					m_playerType = PLAYER_TYPE.RED; // 后进入的用红子
					m_chessBoard.SendMessage("InitRedChessPieces", SendMessageOptions.DontRequireReceiver);
					m_isMyTurn = true; // 红子先走
				} else if (player_type == "black") {
					m_playerType = PLAYER_TYPE.BLACK;
					m_chessBoard.SendMessage("InitBlackChessPieces", SendMessageOptions.DontRequireReceiver);
					m_isMyTurn = false; // 黑子后走
				} else {
					print("玩家数只能有两位，断开");
					m_ws.Close();
				}
				break;
			case "move": // 监听到其他玩家移动棋子
				string move = dic["message_detail"].ToString();
				string[] strings = move.Split('-');
				int index = int.Parse(strings[0]);
				int col = int.Parse(strings[1]);
				int row = int.Parse(strings[2]);
				print(move);
				GameObject piece = m_chessBoard.GetComponent<ChessBoard>().chessPieces[index];
				piece.transform.position = new Vector3(CalcX(col), CalcY(row), -2f);
				piece.GetComponent<ChessPiece>().m_col = col;
				piece.GetComponent<ChessPiece>().m_row = row;
				m_isMyTurn = true;
				break;
		}
	}

	// 从服务器获取到websocket推送的时候执行的函数
	// 这里不能直接发送消息给其他对象，因此需要先将消息存在本地栈中，刷新帧的时候读取
	public void OnMessage(string message) {
		print(message);
		Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
		m_stackMessage.Push(dic);
	}

	// 销毁对象时，一般是游戏强退的时候调用
	public void OnDestroy() {
		if (m_ws != null)
			m_ws.Close();
	}

	public void SendMoveMessage(int index, int dstCol, int dstRow) {
		if (index == -1)
			return;
		string str = string.Format("\"{0:D}-{1:D}-{2:D}\"", index, dstCol, dstRow);
		string message = "{"
			+ "\"message_type\" : \"move\""
			+ " , "
			+ "\"message_detail\" : " + str
			+ "}";
		m_ws.Send(message);
	}

	public int CalcCol(float x) {
		return (int)Math.Round((m_colBegin - x) / m_colDelta, 0) + 1;
	}

	public int CalcRow(float y) {
		return (int)Math.Round((y - m_rowBegin) / m_rowDelta, 0) + 1;
	}

	public float CalcX(int col) {
		return m_colBegin - m_colDelta * (col - 1);
	}

	public float CalcY(int row) {
		return m_rowBegin + m_rowDelta * (row - 1);
	}

	public bool IsValidPosition(int col, int row) {
		if (col < 1 || col > 9 || row < 1 || row > 10)
			return false;
		return true;
	}
}
