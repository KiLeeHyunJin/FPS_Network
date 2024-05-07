using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static Define;

public class StatData
{
	[XmlAttribute]
	public int ID;
	
}

[Serializable, XmlRoot("ArrayOfStatData")]
public class StatDataLoader : ILoader<int, StatData>
{
	[XmlElement("StatData")]
	public List<StatData> _statData = new List<StatData>();

	public Dictionary<int, StatData> MakeDic()
	{
		Dictionary<int, StatData> dic = new Dictionary<int, StatData>();

		foreach (StatData data in _statData)
			dic.Add(data.ID, data);

		return dic;
	}

	public bool Validate()
	{
		return true;
	}
}