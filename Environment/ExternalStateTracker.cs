using System.Collections.Generic;
using UnityEngine;

/* 
 * This class could be seen as the "AI-senses".
 * It is supposed to keep track of the environment.
 * This class is then used by the "EnemyAI"-class to determine,
 * whether or not to traverse its inductive decision tree again.
 * 
 * There is supposed to be one ExternalStateTracker per EnemyAI.
 */
class ExternalStateTracker
{
    private List<IProperty>         allProperties = new List<IProperty>();
    private List<AbstractAttribute> allAttributes = new List<AbstractAttribute>();
    private int                     amountOfAttributes;
    private int[]                   oldSituation;

    public int[]                    GetSituation()      { return oldSituation; }
    public List<AbstractAttribute>  GetAllAttributes()  { return allAttributes; }

    public void AddProperty(IProperty newProperty)
    {
        allProperties.Add(newProperty);
        ListAllAttributes();

        oldSituation = TrackCurrentSituation();
    }

    public bool HasChanged()
    {
        int[] newSituation = TrackCurrentSituation();
        if (oldSituation != null && !HasSameStates(oldSituation, newSituation))
        {
            oldSituation = newSituation;
            return true;
        }
        else { return false; }
    }

    public int[] TrackCurrentSituation()
    {
        int[] newSituation = new int[amountOfAttributes];

        for (int i = 0; i < amountOfAttributes; i++)
            newSituation[i] = allAttributes[i].GetState();

        return newSituation;
    }

    private void ListAllAttributes()
    {
        allAttributes.Clear();
        foreach (IProperty property in allProperties)
            foreach (AbstractAttribute attr in property.GetAllAttributes())
                allAttributes.Add(attr);

        amountOfAttributes = allAttributes.Count;
    }

    private bool HasSameStates(int[] oldSituation, int[] newSituation)
    {
        for (int i = 0; i < oldSituation.Length; i++)
            if (!(oldSituation[i] == newSituation[i])) { return false; }

        return true;
    }

}
