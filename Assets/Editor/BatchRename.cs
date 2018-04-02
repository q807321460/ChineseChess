using UnityEngine;
using UnityEditor;
using System.Collections;

public class BatchRename : ScriptableWizard {

	//名称的前缀
	public string BaseName = "MyObject_";
	//起始号码
	public int StartNumber = 0;
	//递增值
	public int Increment = 1;

	[MenuItem("Edit/Batch Rename...")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard ("Batch Rename", typeof(BatchRename), "Rename");
	}
	//窗口第一次出现的时候，此函数被调用
	void OnEnable()
	{
		UpdateSelectionHelper ();
	}
	//当场景中的选择发生改变的时候，此函数被调用
	void OnSelectionChange()
	{
		UpdateSelectionHelper ();
	}
	//更新对象个数的计数信息
	void UpdateSelectionHelper()
	{
		helpString = "";
		if (Selection.objects != null)
			helpString = "Number of objects selected:" + Selection.objects.Length;
	}
	//实现重命名
	void OnWizardCreate()
	{
		//如果一个对象都没选，函数直接返回
		if (Selection.objects == null)
			return;
		//使用者输入的起始数
		int PostFix = StartNumber;
		//使用循环依次命名
		foreach(Object o in Selection.objects)
		{
			o.name = BaseName + PostFix;
			PostFix += Increment;
		}
	}


}
