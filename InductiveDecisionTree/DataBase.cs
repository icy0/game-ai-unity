using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

class DataBase
{
    // Contains all AbstractAttributes once;
    private List<AbstractAttribute> allAttributes;

    // Contains all AbstractAction, most likely multiple times, 
    // in the order of the possibleSituationsTable.
    // Each AbstractAction is on the index of the situation
    // inside possibleSituationsTable it is relating to.
    private List<AbstractAction>    situationalActions      = new List<AbstractAction>();

    // Contains each AbstractAction's extending classes' instance
    // once. The Count is equal to the amount of classes extending
    // from AbstractAction.
    private List<AbstractAction>    eachUniqueActionInstance   = new List<AbstractAction>();
    [SerializeField]
    private String                  initialTableFilePath = "Assets/Scripts/InductiveDecisionTree/InitialActionTable/InitialActionTable.xml";
    private int                     amountOfAttributes;
    private int                     amountOfPossibleSituations;
    private int[][]                 possibleSituationsTable;
    private XmlDocument             xmlDocument;
    private InductiveDecisionTree   inductiveDecisionTree;
    private GameObject              bulletPrefab;

    /*
     * This is the constructor.
     */
    public DataBase(List<AbstractAttribute> allAttributes, GameObject bulletPrefab)
    {
        this.bulletPrefab = bulletPrefab;
        this.allAttributes = allAttributes;

        amountOfAttributes = allAttributes.Count;
        amountOfPossibleSituations = 1;

        // define amount of possible situations:
        for (int i = 0; i < amountOfAttributes; i++)
            { amountOfPossibleSituations *= allAttributes[i].GetSubdivisionCount(); }

        possibleSituationsTable = new int[amountOfPossibleSituations][];

        InitPossibleSituationsTable();
        LoadOrCreateXML();
        FillSituationalActionsList();

        inductiveDecisionTree = new InductiveDecisionTree(possibleSituationsTable, 
                                                          allAttributes, 
                                                          situationalActions, 
                                                          eachUniqueActionInstance);
    }

    public InductiveDecisionTree GetInductiveDecisionTree() { return inductiveDecisionTree; }

    /*
     * The id3 - (tree creation) algorithm needs to be given a
     * previously determined collection of all possible situations
     * and the action they result in.
     *
     * For performance reasons, we will not create a new instance per
     * attribute per possible situation, as this would rise exponentially
     * with every new attribute being added.
     *
     * We will instead create a two-dimensional array of all
     * possible attribute state combinations.
     */
    private void InitPossibleSituationsTable()
    {
        // treat every single possibility.
        for (int i = 0; i < amountOfPossibleSituations; i++)
        {
            /*
             * To create a table with all possible situations (so every combination
             * of attribute values has to exist once), we will proceed like this:
             * 
             * i indicates the iteration through all possible situations,
             * j indicates the iteration through all attributes per situation.
             * 
             * For every attribute, starting at the first and ending at the last,
             * we will save the current so-called stepsize, which is just an
             * integer indicating how often one possible value will be repeated 
             * throughout the situations until we swap to the next possible value 
             * and start over again.
             * 
             * The stepsize of an attribute is relative to the preceding attributes stepsize.
             * 
             * A simple example:
             * 
             * Imagine:
             *      => the attribute "difficulty" with 3 possible value subdivisions (EASY, MEDIUM, HARD)
             *      => the attribute "health" with 2 possible subdivisions (STABLE, LOW)
             *      
             * The resulting table of possible situations would look like this:
             * 
             *  amountOfPossibleSituations = 3 * 2 = 6
             *  
             *  stepsize for first attribute = 6 / 3 = 2       
             *  
             *      the first attribute will fill two rows with the same value
             *      and will then increment the value with modulo to start on the
             *      lowest value if we reach the highest possible value.
             *      
             *  stepsize for second attribute = (6 / 3) / 2 = 1
             *      
             *      the second attribute will fill one row with the same value
             *      and will then increment the value with modulo to start on the
             *      lowest value if we reach the highest possible value.
             *  
             * 
             *               =========================================
             *              ||    difficulty     |       health      ||
             *              |====================|====================|
             *              | EASY               | STABLE             |
             *              | EASY               | LOW                |
             *              | MEDIUM             | STABLE             |
             *              | MEDIUM             | LOW                |
             *              | HARD               | STABLE             |
             *              | HARD               | LOW                |
             *              |____________________|____________________|
             *              
             * This algorithm can be used for any amount of attributes with any 
             * subdivision counts.
             * 
             */
            // create an array for the current situation i
            int[] situation =   new int[amountOfAttributes];
            int stepsize =      amountOfPossibleSituations;
            for (int j = 0; j < amountOfAttributes; j++)
            {
                stepsize /= allAttributes[j].GetSubdivisionCount();
                situation[j] = (i / stepsize) % (allAttributes[j].GetSubdivisionCount());
            }

            possibleSituationsTable[i] = situation;
        }
    }

    /*
     * This method is for either loading the existing XML or calling the
     * CreateCleanXMLPossibilityTable()-method and loading it afterwards.
     */
    private void LoadOrCreateXML()
    {
        xmlDocument = new XmlDocument();

        try
        {
            xmlDocument.Load(initialTableFilePath);
            Debug.Log("Loaded the initial table successfully and didn't create a clean version.");
        }
        catch (System.IO.FileNotFoundException e)
        {
            Debug.Log(e.StackTrace);
            CreateCleanXMLPossibilityTable();
            xmlDocument.Load(initialTableFilePath);
            // Now missing desired actions though!
            // Have to be added manually.
        }
    }

    /*
     * This method will create an XML-document filled with all possible situations.
     * The developer then has to write in the actions for each possible situation by hand
     * (as there is no way to calculate, what the correct action for each situation would be).
     * This is the downside of using decision trees.
     */
    private void CreateCleanXMLPossibilityTable()
    {
        XmlWriterSettings xmlWriterSettings     = new XmlWriterSettings();
        xmlWriterSettings.Indent                = true;
        xmlWriterSettings.OmitXmlDeclaration    = true;

        XmlWriter xmlWriter = XmlWriter.Create(initialTableFilePath, xmlWriterSettings);

        xmlWriter.WriteStartElement("table");

        for (int i = 0; i < amountOfPossibleSituations; i++)
        {
            xmlWriter.WriteStartElement("situation");
            xmlWriter.WriteAttributeString("index", i.ToString());
            xmlWriter.WriteAttributeString("action", "");

            for (int j = 0; j < amountOfAttributes; j++)
            {
                xmlWriter.WriteStartElement("attribute");
                xmlWriter.WriteAttributeString("name", allAttributes[j].GetName());
                xmlWriter.WriteAttributeString("subdivisions", allAttributes[j].GetSubdivisionCount().ToString());
                xmlWriter.WriteAttributeString("value", possibleSituationsTable[i][j].ToString());
                xmlWriter.WriteAttributeString("valueName", allAttributes[j].GetStateName(possibleSituationsTable[i][j]));
                xmlWriter.WriteEndElement(); // attribute end-tag
            }

            xmlWriter.WriteEndElement(); // situation end-tag
        }

        xmlWriter.WriteEndElement(); // table end-tag

        xmlWriter.Flush();
        xmlWriter.Close();

        try { xmlDocument.Load(initialTableFilePath); }
        catch (System.IO.FileNotFoundException e) { Debug.Log(e.StackTrace); }
    }

    /*
     * This method fills the actiontype list with all actions
     * by reading the actions from the xml document.
     */
    private void FillSituationalActionsList()
    {
        XmlNode root = xmlDocument.FirstChild;

        // explicitly create one instance of each action:
        AttackFromAboveAction   attackFromAboveAction = new AttackFromAboveAction(bulletPrefab);
        DirectAttackAction      directAttackAction    = new DirectAttackAction(bulletPrefab);
        GenerateHPAction        generateHPAction      = new GenerateHPAction();

        eachUniqueActionInstance.Add(attackFromAboveAction);
        eachUniqueActionInstance.Add(directAttackAction);
        eachUniqueActionInstance.Add(generateHPAction);

        foreach(XmlNode child in root.ChildNodes)
        {
            switch (child.Attributes[1].Value)
            {
                case ("AttackFromAbove"):
                    situationalActions.Add(attackFromAboveAction);
                    break;
                case ("DirectAttack"):
                    situationalActions.Add(directAttackAction);
                    break;
                case ("GenerateHP"):
                    situationalActions.Add(generateHPAction);
                    break;
                default:
                    Debug.Log("You forgot to create an instance of your new action and/or create a case for it."
                        + "Go to FillSituationalActionsList() in the Database-class.");
                    break;
            }
        }
    }
}