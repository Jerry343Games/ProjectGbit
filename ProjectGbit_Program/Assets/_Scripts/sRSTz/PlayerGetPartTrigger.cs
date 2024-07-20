using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetPartTrigger : MonoBehaviour
{
    private PlayerBot _playerBot;

    private void Awake()
    {
        _playerBot = transform.parent.GetComponent<PlayerBot>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_playerBot.currentPart)
        {
            return;
        }
        if (other.gameObject.tag == "Part")
        {
            _playerBot.GetPart(other.gameObject.transform.parent.GetComponent<Part>());
            //Ïú»ÙÁã¼þ
            Destroy(other.gameObject);
            


        }
    }
}
