using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UInfo : MonoBehaviour
{
    public Text top;
    public Text bot;
    public Text rig;
    public Text lef;
    public Text climb;
    public Text desc;
    public Text ang;

    public MovementCollisionSolver solver;

    // Update is called once per frame
    void Update()
    {
        top.text = "TOP:" + solver.collisionInfo.top;
        bot.text = "BOT:" + solver.collisionInfo.bottom;
        rig.text = "RIG:" + solver.collisionInfo.right;
        lef.text = "LEFT:" + solver.collisionInfo.left;
        climb.text = "CLIMB:" + solver.collisionInfo.climbingSlope;
        desc.text = "DESC:" + solver.collisionInfo.descendigSlope;
        ang.text = "ANG:" + solver.collisionInfo.slopeAngle;
    }
}
