using System;

class PlayerAmmoAttribute : AbstractAttribute
{
    private const int           MAX_VALUE = 80;
    private int                 value = MAX_VALUE;

    public override int         GetSubdivisionCount()   { return 3; }
    public override int         GetValue()              { return value; }
    public override int         GetMaxValue()           { return MAX_VALUE; }
    public override string      GetName()               { return "PlayerAmmo"; }

    public void changeBy(int difference)
    {
        if      (value + difference <= 0)           { value = 0; }
        else if (value + difference >= MAX_VALUE)   { value = MAX_VALUE; }
        else                                        { value += difference; }
        hasChanged = true;
    }

    public override int GetState()
    {
        // special case
        if (value == MAX_VALUE) { return 2; }
        else
        {
            float divideBy = (float) MAX_VALUE / GetSubdivisionCount();
            int result = (int) (GetValue() / divideBy);
            return result;
        }
    }

    public override string GetStateName(int state)
    {
        String valueName = "";

        switch (state)
        {
            case (2):
                valueName = "High";
                break;
            case (1):
                valueName = "Medium";
                break;
            case (0):
                valueName = "Low";
                break;
        }

        return valueName;
    }
}
