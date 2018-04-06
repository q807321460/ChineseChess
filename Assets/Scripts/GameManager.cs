using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Unity自带的Json解析方式，灵活性实在太差了，我怎么能预先知道json字符串的内容呢？所以用Newtonsoft.Json好了
//public class WebsocektMessage {
//	public string type;
//	public object message;
//}

public class GameManager : MonoBehaviour {

	public enum NET_TYPE { NONE, PLAYER_RED, PLAYER_BLACK }; // 未准备，红方，黑方
	public NET_TYPE m_netType = NET_TYPE.NONE;
	public GameObject m_chessBoard = null;
	public WebSocket m_ws = null;
	public string m_url = "ws://127.0.0.1:8088/ALiCloudServer/WebsocketTest/";
	public InputField m_txtPlayerName = null;
	public Button m_btnConnect = null;
	public Button m_btnDisconnect = null;
	public Stack<Dictionary<string, object>> m_stackMessage = new Stack<Dictionary<string, object>>();

	private static volatile GameManager instance;
	private static object syncRoot = new Object();
	public static GameManager Instance {
		get {
			if (instance == null) {
				lock (syncRoot) {
					if (instance == null)
						instance = new GameManager();
				}
			}
			return instance;
		}
	}

	public void OnConnectClicked() {
		if (m_ws != null && m_ws.IsAlive) {
			print("已连入，不能重新连入");
			return;
		}
		m_url += m_txtPlayerName.text;
		m_ws = new WebSocket(m_url); // 47.100.97.154
		m_ws.OnOpen += (sender, e) => {
			print("OnOpen");
			m_chessBoard.SendMessage("InitServerChessPieces", SendMessageOptions.DontRequireReceiver);
		};
		m_ws.OnMessage += (sender, e) => {
			OnMessage(e.Data);
		};
		m_ws.OnError += (sender, e) => {
			print("OnError : " + e.Message);
		};
		m_ws.OnClose += (sender, e) => {
			print("OnClose");
		};
		m_ws.Connect();
		m_ws.Send("test");
	}

	public void OnDisconnectClicked() {
		m_ws.Close();
	}

	void Start() {
		m_txtPlayerName.text = "Kong";
		m_btnConnect.onClick.AddListener(OnConnectClicked);
		m_btnDisconnect.onClick.AddListener(OnDisconnectClicked);
	}

	void Update() {
		DoMessage();
	}

	public void DoMessage() {
		if (m_stackMessage.Count == 0)
			return;
		Dictionary<string, object> dic = m_stackMessage.Pop();
		string sType = dic["type"].ToString();
		print("接收到的数据类型为：" + sType);
		// 服务器检测到有新玩家连入时，则返回当前玩家数，如果当前连入量大于2，则主动断开
		switch (sType) {
			case "player_count":
				int player_count = int.Parse(dic["message"].ToString());
				print("当前服务器人数为：" + player_count);
				if (player_count == 1) {
					m_netType = NET_TYPE.PLAYER_BLACK; // 先进入的用黑子，这里可以根据需求修改
					m_chessBoard.SendMessage("InitBlackChessPieces", SendMessageOptions.DontRequireReceiver);
				} else if (player_count == 2) {
					m_netType = NET_TYPE.PLAYER_RED; // 后进入的用红子
					m_chessBoard.SendMessage("InitRedChessPieces", SendMessageOptions.DontRequireReceiver);
				} else {
					print("玩家数只能有两位，断开");
					m_ws.Close();
				}
				break;
		}
	}

	// 这里不能直接发送消息给其他对象，因此需要先将消息存在本地栈中，刷新帧的时候读取
	public void OnMessage(string message) {
		print (message);
		Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
		m_stackMessage.Push(dic);
	}

	public void OnDestroy() {
		m_ws.Close ();
	}
}
