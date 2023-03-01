using System;

public abstract class AbstractAttribute
{
    protected bool hasChanged = false;

    public abstract int GetSubdivisionCount();
    public abstract int GetValue();
    public abstract int GetMaxValue();
    public abstract int GetState();
    public abstract String GetStateName(int value);
    public abstract String GetName();

    /*
     * save the boolean in a temporary variable,
     * set it to false and return the temporary
     * variable.
     */
    public bool HasChanged()
    {
        bool hadChanged = hasChanged;
        hasChanged = false;
        return hadChanged;
    }
}