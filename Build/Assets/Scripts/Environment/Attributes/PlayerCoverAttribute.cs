using System;

class PlayerCoverAttribute : AbstractAttribute
{
    public bool inCover = true;

    public override int     GetMaxValue()           { return 1; }
    public override int     GetSubdivisionCount()   { return 2; }
    public override string  GetName()               { return "PlayerCover"; }
    public override int     GetState()              { return GetValue(); }

    public void change(bool inCover)
    {
        this.inCover = inCover;
        hasChanged = true;
    }

    public override int GetValue()
    {
        if (inCover) { return 1; }
        else { return 0; }
    }

    public override string GetStateName(int state)
    {
        String valueName = "";

        switch (state)
        {
            case (1):
                valueName = "In cover";
                break;
            case (0):
                valueName = "In the open";
                break;
        }

        return valueName;
    }
}
