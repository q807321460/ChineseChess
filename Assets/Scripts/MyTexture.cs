using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTexture : MonoBehaviour {

	private static GameObject m_pMainObject;
	private static MyTexture m_pContainer;
	public static MyTexture getInstance() {
		if (m_pContainer == null) {
			m_pContainer = m_pMainObject.GetComponent<MyTexture> ();
		}
		return m_pContainer;
	}
	private Dictionary<string, Object[]> m_pAtlasDic; // 图集的集合

	private void initData() {
		MyTexture.m_pMainObject = gameObject;
		m_pAtlasDic = new Dictionary<string, Object[]> ();
	}

	// 删除图集缓存
	public void DeleteAtlas(string spriteAtlasPath) {
		if (m_pAtlasDic.ContainsKey(spriteAtlasPath)) {
			m_pAtlasDic.Remove (spriteAtlasPath);
		}
	}

	// 从objects中找出sprite
	private Sprite SpriteFromAtlas(Object[] atlas, string spriteName) {
		for (int i = 0; i < atlas.Length; i++) {
			if (atlas[i].GetType() == typeof(UnityEngine.Sprite)) {
				if (atlas[i].name == spriteName) {
					return (Sprite)atlas [i];
				}
			}
		}
		Debug.LogWarning ("图片名:" + spriteName + ";在图集中找不到");
		return null;
	}

	// 从缓存中查找图集，并找出sprite
	private Sprite FindSpriteFromBuffer(string spriteAtlasPath, string spriteName) {
		if (m_pAtlasDic.ContainsKey(spriteAtlasPath)) {
			Object[] atlas = m_pAtlasDic [spriteAtlasPath];
			Sprite sprite = SpriteFromAtlas (atlas, spriteName);
			return sprite;
		}
		return null;
	}


	// 加载图集上的一个精灵
	public Sprite LoadAtlasSprite(string spriteAtlasPath, string spriteName) {
		Sprite sprite = FindSpriteFromBuffer (spriteAtlasPath, spriteName);
		if (sprite == null) {
			Object[] atlas = Resources.LoadAll (spriteAtlasPath);
			m_pAtlasDic.Add (spriteAtlasPath, atlas);
			sprite = SpriteFromAtlas (atlas, spriteName);
		}
		return sprite;
	}

	void Awake() {
		initData ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
