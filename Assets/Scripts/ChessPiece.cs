using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour {
	//public bool m_isMine; // 是否属于当前玩家
	public enum CHESS_PIECE_COLOR {RED, BLACK};
	public CHESS_PIECE_COLOR m_color;
	public enum CHESS_PIECE_TYPE {BING, JU, MA, PAO, XIANG, SHI, SHUAI};
	public CHESS_PIECE_TYPE m_type; // 该子的类型
	public int m_col, m_row; // 该子当前的行和列(相对于自己方向上的位置)
	public bool m_isSelected; // 是否被选中

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
