using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class InductiveDecisionTree
{
    private int[][]                 possibleSituations;
    private List<AbstractAction>    situationalActions;
    private List<AbstractAttribute> allAttributes;
    private List<AbstractAction>    eachUniqueAbstractAction;
    private int                     amountOfPossibleSituations;
    private int                     amountOfAttributes;
    private int                     amountOfActions;
    private Question                rootQuestion;
    private float                   entropyOfAllActions;
    private int[]                   attributeIndicesOrderedByMaxInfoGain;

    public InductiveDecisionTree(int[][] possibleSituations,
                                List<AbstractAttribute> allAttributes,
                                List<AbstractAction> situationalActions, 
                                List<AbstractAction> eachUniqueAbstractAction)
    {
        this.possibleSituations = possibleSituations;
        this.allAttributes = allAttributes;
        this.situationalActions = situationalActions;
        this.eachUniqueAbstractAction = eachUniqueAbstractAction;

        amountOfPossibleSituations = possibleSituations.Length;
        amountOfAttributes = allAttributes.Count;
        amountOfActions = eachUniqueAbstractAction.Count;

        CalculateEntropyAndInfoGain();
        BuildDecisionTree();
    }

    /*
     * This method initializes the decision tree from the given possible situations 
     * and their actions.
     * 
     * This is the id3-algorithm, essentially.
     */
    private void CalculateEntropyAndInfoGain()
    {
        // entropy of the entire action-set
        entropyOfAllActions = 0.0f;

        // The informational gain of each single attribute to determine, how to
        // build the decision tree.
        float[] infoGainOfEachAttribute = new float[amountOfAttributes];

        // calculate entropy of all actions
        foreach (AbstractAction action in eachUniqueAbstractAction)
        {
            entropyOfAllActions += EntropyOfAction(situationalActions, action);
        }

        // calculate information gain of each attribute
        for (int i = 0; i < amountOfAttributes; i++)
        {
            infoGainOfEachAttribute[i] = InformationGainOfAttribute(i);
        }

        // list the indices of the attributes (starting with the one with the highest
        // informational gain).
        attributeIndicesOrderedByMaxInfoGain = new int[amountOfAttributes];

        for (int i = 0; i < amountOfAttributes; i++)
        {
            int attributeWithMaxInfoGain = Array.IndexOf<float>(infoGainOfEachAttribute, infoGainOfEachAttribute.Max());
            attributeIndicesOrderedByMaxInfoGain[i] = attributeWithMaxInfoGain;
            infoGainOfEachAttribute[attributeWithMaxInfoGain] = 0.0f;
        }
    }

    /*
     * This method calculates the entropy of the specified action.
     */
    private float EntropyOfAction(List<AbstractAction> list, AbstractAction action)
    {
        int occurrenceOfValue = 0;
        foreach (AbstractAction entry in list)
            if(entry.Equals(action))
                occurrenceOfValue++;

        // p(x)
        float chanceOfOccurrenceOfValue = (float) occurrenceOfValue / amountOfPossibleSituations;

        // -(p(x) * Math.log(p(x), 2))
        float entropy = -(chanceOfOccurrenceOfValue * (float) (Math.Log(chanceOfOccurrenceOfValue, 2)));

        return entropy;
    }

    /*
     * Information Gain is the difference of the entropy of the entire dataset
     * and the entropy of the specified attribute inside the dataset.
     * 
     * In other words, it is the amount of information we can retrieve from
     * this exact attribute.
     */
    private float InformationGainOfAttribute(int attributeIndex)
    {
        AbstractAttribute attribute = allAttributes[attributeIndex];
        int amountOfSubdivisions = attribute.GetSubdivisionCount();

        // The informational gain of an attribute is its entropy minus the entropy of all actions.
        // That is why we initialize it with the entropy of all actions and iteratively reduce it
        // with the entropy of each value of the attribute.
        float infoGain = entropyOfAllActions;

        // iterate through the subdivisions of our attribute and reduce the entropy of all actions
        // with each entropy of each subdivision of our attribute multiplied with the occurrence of the 
        // subdivision (which is always guaranteed to be constant, since we cover every possible situation).
        for (int i = 0; i < amountOfSubdivisions; i++)
        {
            float chanceOfOccurrenceOfSubdivision = (float) 1 / amountOfSubdivisions;
            infoGain -= chanceOfOccurrenceOfSubdivision * EntropyOfValueOfAttribute(i, attributeIndex);
        }

        // the result is our informational gain.
        return infoGain;
    }

    /*
     * This method is a helper method for the information gain method.
     * It provides the entropy of a specified value of a specified attribute.
     */
    private float EntropyOfValueOfAttribute(int value, int attributeIndex)
    {
        float entropy = 0.0f;

        // for each action there is a chance of occurrence of each subdivision of the attribute
        for(int i = 0; i < amountOfActions; i++)
        {
            float chanceOfOccurrence = ChanceOfOccurrenceOfValueOfAttributeWithAction(value, attributeIndex, eachUniqueAbstractAction[i]);

            // if chanceOfOccurrence is 0, the Math.Log(0, 2) would return an undefined value.
            // so if chanceOfOccurrence is 0, we can just ignore its iteration.
            if(chanceOfOccurrence != 0)
            { entropy -= chanceOfOccurrence * (float) Math.Log(chanceOfOccurrence, 2); }
        }

        return entropy;
    }

    /*
     * This method returns the chance of occurrence of a value of an attribute with a specific resulting action inside the
     * list of all possible situations.
     * (Couldn't figure out a better name ...)
     */ 
    private float ChanceOfOccurrenceOfValueOfAttributeWithAction(int value, int attributeIndex, AbstractAction action)
    {
        int occurrencesOfValueWithAction = 0;

        // iterate over every possible situation.
        for (int i = 0; i < amountOfPossibleSituations; i++)
        {
            // if the current situation results in our specified action AND
            // if the value of our specified attribute equals the value of the attribute in the current situation,
            // THEN increment the occurrence of the value with the specified action.
            if(situationalActions[i].Equals(action) && possibleSituations[i][attributeIndex] == value)
            { occurrencesOfValueWithAction++; }
        }

        // This is essentially how often our value exists in all possible situations,
        // which is always amount-of-possible-situations / subdivisioon-of-attribute.
        float occurrencesOfValue = (float) amountOfPossibleSituations / allAttributes[attributeIndex].GetSubdivisionCount();

        // return the occurrence of the specific value with the specified action, divided by the 
        // general occurrences of the value, independent of which action.
        return occurrencesOfValueWithAction / occurrencesOfValue;
    }

    /*
     * This method initializes the rootQuestion and creates the childnodes iteratively.
     */
    private void BuildDecisionTree()
    {
        rootQuestion = new Question(allAttributes[attributeIndicesOrderedByMaxInfoGain[0]], 
                                    attributeIndicesOrderedByMaxInfoGain[0], 
                                    amountOfAttributes,
                                    0);

        CreateChildNodes(rootQuestion);
    }

    /*
     * This method creates the childnodes for a specific question node iteratively.
     */
    private void CreateChildNodes(Question question)
    {
        for (int i = 0; i < question.GetAttributesSubdivision(); i++)
        {
            // if this question's attribute's index is the last one 
            // inside our ordered list of indices of the attribute's information gain
            // (if it is the last attribute we ask for in our tree, before we
            // get the action), then return the desired action.
            if (question.GetAttributeIndex() == attributeIndicesOrderedByMaxInfoGain[attributeIndicesOrderedByMaxInfoGain.Length - 1])
            {
                SetValueAndIndexTrace(question, question.GetRecursionDepth(), i, question);
                AbstractAction action = FindAction(question.GetValueTrace(), question.GetIndexTrace());
                question.AddChild(i, action);
            }
            // else, find the next attribute in our ordered list of
            // indices of the attributes and create one question per
            // own subdivision.
            else
            {
                // find own index in list of indices and get the following index.
                int nextAttributeIndex = attributeIndicesOrderedByMaxInfoGain[
                    Array.IndexOf<int>(attributeIndicesOrderedByMaxInfoGain, question.GetAttributeIndex()) + 1];

                AbstractAttribute nextAttribute = allAttributes[nextAttributeIndex];

                Question child = new Question(nextAttribute,
                                            nextAttributeIndex,
                                            amountOfAttributes,
                                            question.GetRecursionDepth() + 1);

                SetValueAndIndexTrace(question, question.GetRecursionDepth(), i, child);
                question.AddChild(i, child);
                CreateChildNodes(child);
            }
        }
    }
    
    /*
     * This method returns the action of the specified situation.
     */
    private AbstractAction FindAction(int[] valueTrace, int[] indexTrace)
    {
        int[] situationRecreation = new int[valueTrace.Length];

        for (int i = 0; i < situationRecreation.Length; i++)
            situationRecreation[i] = valueTrace[indexTrace[i]];

        int indexOfSituation = 0;

        for(int i = 0; i < possibleSituations.Length; i++)
        {
            if(HasSameValues(possibleSituations[i], situationRecreation))
            {
                indexOfSituation = i;                
                break;
            }
        }

        return situationalActions[indexOfSituation];
    }

    /*
     * This method compares both arrays.
     * If they have the same values, it returns true.
     */
    private bool HasSameValues(int[] a, int[] b)
    {
        for (int i = 0; i < a.Length; i++)
            if(a[i] != b[i])
                return false;

        return true;
    }
    
    /*
     * This method is responsible for setting the index and value trace arrays.
     * These are necessary to track the question's ancestors so that a resulting
     * action can be determined.
     */
    private void SetValueAndIndexTrace(Question question, int recursionDepth, int value, Question target)
    {
        int[] valueTraceClone = new int[question.GetValueTrace().Length];
        int[] indexTraceClone = new int[question.GetIndexTrace().Length];

        Array.Copy(question.GetValueTrace(), valueTraceClone, question.GetValueTrace().Length);
        Array.Copy(question.GetIndexTrace(), indexTraceClone, question.GetIndexTrace().Length);

        valueTraceClone[recursionDepth] = value;
        indexTraceClone[recursionDepth] = question.GetAttributeIndex();

        target.SetIndexTrace(indexTraceClone);
        target.SetValueTrace(valueTraceClone);
    }

    /*
     * This method is called upon the inductive decision tree,
     * to retrieve an action from a given situation.
     * Its complexity is equal to the amount of attributes, out of
     * which the decision tree is created, log(n).
     */
    public AbstractAction traverse(int[] newSituation)
    {
        int value = 0;
        Question nextNode = rootQuestion;

        for (int i = 0; i < newSituation.Length - 1; i++)
        {
            value = newSituation[nextNode.GetAttributeIndex()];
            nextNode = (Question) nextNode.GetChild(value);
        }

        return (AbstractAction) nextNode.GetChild(value);
    }
}
