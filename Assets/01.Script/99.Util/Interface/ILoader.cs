
using System.Collections.Generic;

public interface ILoader<Key, Item>
{
    Dictionary<Key, Item> MakeDic();
    bool Validate();
}