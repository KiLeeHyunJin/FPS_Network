using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class TextData
{
	[XmlAttribute]
	public int ID;
	[XmlAttribute]
	public string kor;
}

[Serializable, XmlRoot("ArrayOfTextData")]
public class TextDataLoader : ILoader<int, TextData>
{
	[XmlElement("TextData")]
	public List<TextData> _textData = new List<TextData>();

	public Dictionary<int, TextData> MakeDic()
	{
		Dictionary<int, TextData> dic = new Dictionary<int, TextData>();

		foreach (TextData data in _textData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}