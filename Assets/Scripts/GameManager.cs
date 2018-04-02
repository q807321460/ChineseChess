using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.PlayerConnection;
//using System.Collections.Generic;
using WebSocketSharp;


public class GameManager : NetworkBehaviour {

	public enum NET_TYPE {NONE, SERVER, CLIENT};
	public NET_TYPE m_netType = NET_TYPE.NONE;
	public GameObject m_chessBoard = null;

	void Start() {
		//		NetworkManager.singleton.StartMatchMaker(); // 启用Unet网络对战功能 47.100.97.154
//		FindInternetMatch ("test");
		var ws = new WebSocket ("ws://47.100.97.154:8088/ALiCloudServer/WebsocketTest/KongHanwen");
		ws.OnOpen += (sender, e) => {
			print (e.ToString ());
		};
		ws.OnMessage += (sender, e) => {
			print (e.Data);
		};
		ws.OnError += (sender, e) => {
			print (e.Message);
		};
		ws.OnClose += (sender, e) => {
			print (e.Reason);
		};
		ws.ConnectAsync ();
		ws.SendAsync = (flag) => {
			print("send finish");
		};
	}

	// 创建房间
	public void CreateInternetMatch(string matchName) {
		NetworkManager.singleton.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, 0, OnInternetMatchCreate);
	}

	// 创建房间后使用
	private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
		if (success) {
			Debug.Log("创建房间成功");
			MatchInfo hostInfo = matchInfo;
			NetworkServer.Listen(hostInfo, 9999);
			NetworkManager.singleton.StartHost(hostInfo);
			m_netType = NET_TYPE.SERVER;
			m_chessBoard.SendMessage ("InitServerChessPieces", SendMessageOptions.DontRequireReceiver);
		} else {
			Debug.LogError("创建房间失败");
		}
	}

	// 搜索指定房间列表
	public void FindInternetMatch(string matchName) {
		NetworkManager.singleton.matchMaker.ListMatches(0, 10, matchName, false, 0, 0, OnInternetMatchList);
	}

	// 搜索房间列表后返回
	private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
		if (success) {
			if (matches.Count != 0) {
				Debug.Log("返回了一个匹配房间列表");
				NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
			} else {
				Debug.Log("没有找到房间!");
				CreateInternetMatch ("test");
			}
		} else {
			Debug.LogError("无法连接至房间");
		}
	}

	// 在客户端使用，连入房间后进入这里
	private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo) {
		if (success) {
			Debug.Log("Able to join a match");
			MatchInfo hostInfo = matchInfo;
			NetworkManager.singleton.StartClient(hostInfo);
//			NetworkManager.singleton.onclient
			m_netType = NET_TYPE.CLIENT;
			m_chessBoard.SendMessage ("InitClientChessPieces", SendMessageOptions.DontRequireReceiver);
		} else {
			Debug.LogError("加入房间失败");
		}
	}

	public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		print ("OnServerAddPlayer");
	}

	public virtual void OnServerConnect(NetworkConnection connection) {
		string text = "Client " + connection.connectionId + " Connected!";
		print (text);
	}

	public virtual void OnServerDisconnect(NetworkConnection connection) {
		string text = "Client " + connection.connectionId + " Disconnect!";
		print (text);
	}

	public override void OnStartServer() {
		print ("OnStartServer");
	}

}
