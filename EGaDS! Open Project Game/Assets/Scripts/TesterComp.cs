using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterComp : MonoBehaviour
{
    public Ability ability;
    public AbilityManager abilityMan;

    public int slotIndex = 1;

    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void printList(List<Ability> list)
    {
        string str = "[";
        foreach (Ability ability in list)
        {
            str += ability + ", ";
        }
        str = str.Substring(0, str.Length) + "]";

        Debug.Log(str);
    }

    // Update is called once per frame
    void Update()
    {
        //if(count == 0)
        //{
        //    abilityMan.SetAbilityCount(2);
        //    abilityMan.AddAbility(ability, slotIndex);

        //    Debug.Log(AbilityManager.ABILITY_KEY_CODES[0]);

        //    List<int> testList = new List<int>(3);
        //    //testList[1] = 5;

        //    Debug.Log(testList.ToString());

        //    List<int> testList2 = new List<int>();

        //    Debug.Log(testList2.ToString());

        //    testList2.AddRange(testList);
        //    Debug.Log(testList2.Count);
        //}
        //count++;

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Ability removedAbility = abilityMan.RemoveAbility(0);
        //    if (removedAbility == ability)
        //    {
        //        Debug.Log("Remove sucessful");
        //    }

            
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    abilityMan.AddAbility(ability, slotIndex);

        //    List<Ability> removedAbilities = abilityMan.SetAbilityCount(1);
        //    Debug.Log(removedAbilities.Count);
        //    printList(removedAbilities);
        // }
    }
}
