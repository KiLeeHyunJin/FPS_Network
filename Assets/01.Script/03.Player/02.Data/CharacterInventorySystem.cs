using System.Collections.Generic;
using UnityEngine;
public class ItemNode
{
    public ItemBase item;
    public int count;
    public int idx;
    public void SetData(ItemBase _item, int _count)
    {
        item = _item;
        count = _count;
    }
}

public class CharacterInventorySystem
{
    LinkedList<ItemNode> invenList;
    ItemNode[] bullets;
    public CharacterInventorySystem()
    {
        invenList = new LinkedList<ItemNode>();
        bullets = new ItemNode[(int)Define.Bullet.END];
    }
    public void AddItem(int count, ItemBase itemBase)
    {
        bool state = false;
        int idx = 0;
        foreach (ItemNode item in invenList)
        {
            if (item.item == null)
            {
                item.SetData(itemBase, count);
                state = true;
                break;
            }
            else if (
                itemBase.ItemType != Define.Item.Part &&
                itemBase.ItemType == item.item.ItemType &&
                itemBase.ItemId == item.item.ItemId)
            {
                item.count += count;
                state = true;
                break;
            }
            if (item.idx != idx)
                item.idx = idx;
            idx++;
        }

        if (state == false)
        {
            ItemNode item = new ItemNode();
            item.SetData(itemBase, count);
            item.idx = idx;
            LinkedListNode<ItemNode> node = new LinkedListNode<ItemNode>(item);
            invenList.AddLast(node);
        }
    }

    public int CheckBullet(Define.Bullet bulletType)
    {
        int count = GetBulletCount(bulletType);
        if (count > 0)
            return count;
        FindBullet();
        return GetBulletCount(bulletType);
    }

    public int RemoveBullet(Define.Bullet bulletType, int count)
    {
        return RemoveItem(bullets[(int)bulletType].idx, count);
    }

    public int RemoveItem(int idx, int count = 1)
    {
        LinkedListNode<ItemNode> node = FindNode(idx);
        int answard = count;
        if (node.Value.count >= count)
            node.Value.count -= count;
        else
        {
            answard = node.Value.count;
            node.Value.count = 0;
        }
        RemoveCheckNode(node);
        return answard;
    }
    public void MoveItem(int from, int to)
    {
        LinkedListNode<ItemNode> moveNode = FindNode(from);
        LinkedListNode<ItemNode> destNode = FindNode(to);
        ItemNode moveValue = moveNode.Value;
        invenList.AddAfter(destNode, new LinkedListNode<ItemNode>(moveValue));
    }

    bool RemoveCheckNode(LinkedListNode<ItemNode> node)
    {
        if (node.Value.count == 0 || node.Value.item != null)
        {
            LinkedListNode<ItemNode> newNode = node.Next;
            int idxNum = node.Value.idx;
            if (node.Value.item != null)
                node.Value.item = null;
            invenList.Remove(node);
            if (newNode != invenList.Last)
                ReIdxNum(newNode, idxNum);
            return true;
        }
        return false;
    }
    void ReIdxNum(LinkedListNode<ItemNode> node, int idxNum)
    {
        for (; node != invenList.Last; node = node.Next)
        {
            node.Value.idx = idxNum;
            idxNum++;
        }
    }
    int GetBulletCount(Define.Bullet bulletType)
    {
        if (bullets[(int)bulletType].item != null)
        {
            if (bullets[(int)bulletType].count > 0)
                return bullets[(int)bulletType].count;
        }
        return 0;
    }
    void FindBullet()
    {
        int idx = 0;
        foreach (ItemNode item in invenList)
        {
            if (item.item.ItemType == Define.Item.Bullet)
            {
                BulletItem bullet = item.item as BulletItem;
                if (bullet != null)
                {
                    if (
                        bullets[(int)bullet.bulletType].count < bullet.count ||
                        bullets[(int)bullet.bulletType].item == null
                        )
                        bullets[(int)bullet.bulletType] = item;
                }

            }
            if (item.idx != idx)
                item.idx = idx;
            idx++;
        }
    }

    LinkedListNode<ItemNode> FindNode(int idx)
    {
        LinkedListNode<ItemNode> node = invenList.First;
        for (int i = 0; i <= idx; i++)
        {
            if (node.Next != invenList.Last)
            {
                node = node.Next;
            }
            else
            {
                Debug.Log("뭔가 문제 있음");
                return null;
            }
        }
        return node;
    }


}
