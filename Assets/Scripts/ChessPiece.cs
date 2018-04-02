using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChessPiece : NetworkBehaviour {

	public bool m_isMine; // 是否属于当前玩家
	public bool m_isBlack; // 是红子还是黑子
	public enum CHESS_PIECE_TYPE {TYPE_BING, TYPE_JU, TYPE_MA, TYPE_PAO, TYPE_XIANG, TYPE_SHI, TYPE_SHUAI};
	public CHESS_PIECE_TYPE m_type; // 该子的类型
	public int m_col, m_row; // 该子当前的行和列(相对于自己方向上的位置)

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
