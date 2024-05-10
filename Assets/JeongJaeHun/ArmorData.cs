using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorData : MonoBehaviour
{
    // 아머는 인벤토리에 들어가지 않고 그냥 방어도랑 내구도만 있으면된다. 
    // 장착된 상태인지 확인해야 하고 그 장착된 방어도에 따라 받는 데미지가 달라짐 
    // 결국 핵심은 
    // 1. 상점에서 price 비교에 따라 구입 성공/실패를 구현한다.
    // 2. 구입에 성공하면 자동으로 장착된다.
    // 3. 방어구의 종류에 따라 내구도와 방어력이 달라진다. --> 이를 플레이어에게 반영해준다.
    // 4. 방어구 관련된 이미지 등 ui를 업데이트 해준다. 
    // 5. 인벤토리에 직접 들어가지는 않으나 장착중이라는 표시를 해야하기 때문에 따로 저장을 해줘야한다. 



}
