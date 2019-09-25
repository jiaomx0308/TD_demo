using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : Enemy
{
    // Update is called once per frame
    new void Update()
    {
        base.Update();
        Fly();
    }

    void Fly()
    {
        float flySpeed = 0;
        if (this.transform.position.y < 2.0f)
        {
            flySpeed = 1f;
        }

        this.transform.Translate(new Vector3(0, flySpeed * Time.deltaTime, 0));
    }
}
