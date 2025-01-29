using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLiter;
public class DeleteTable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SQLiter.SQLite.Instance.DropTable("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
