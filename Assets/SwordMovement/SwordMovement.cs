using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SwordMovement
{
    public abstract void MoveSword(bool cursorInCircle);

    public abstract void RotateSwordXY(float angle = 0);

    public abstract void MoveSwordXY(bool cursorInCircle);

    public abstract void Extend();

    public abstract void Retract();
}
