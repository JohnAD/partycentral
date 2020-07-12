using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using partycentral;

public class CharacterBehaviourScript : MonoBehaviour
{
	CharacterBehavior ThisCharacterBehavior;
    // Start is called before the first frame update
    void Start()
    {
    	ThisCharacterBehavior = new CharacterBehavior(CharacterRole.RandomGuest, "name");        
    }

    // Update is called once per frame
    void Update()
    {
        ThisCharacterBehavior.UpdateDecision();
    }
}
