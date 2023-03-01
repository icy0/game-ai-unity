using System.Collections.Generic;

class Question : AbstractNode
{
    AbstractAttribute   askedAttribute;
    int                 amountOfAttributes;
    private int         indexOfAttribute;
    private int         recursionDepth;
    private int[]       indexTraces;
    private int[]       valueTraces;

    public Question(AbstractAttribute askedAttribute, int indexOfAttribute, int amountOfAttributes, int recursionDepth)
    {
        base.children = new Dictionary<int, AbstractNode>();

        this.askedAttribute = askedAttribute;
        this.indexOfAttribute = indexOfAttribute;
        this.amountOfAttributes = amountOfAttributes;
        this.recursionDepth = recursionDepth;

        this.indexTraces = new int[this.amountOfAttributes];
        this.valueTraces = new int[this.amountOfAttributes];
    }

    public void         AddChild(int attrValue, AbstractNode child) { children[attrValue] = child; }
    public int          GetAttributesSubdivision()                  { return askedAttribute.GetSubdivisionCount(); }
    public int          GetAttributeIndex()                         { return indexOfAttribute; }
    public void         SetIndexTrace(int[] indexTraces)            { this.indexTraces = indexTraces; }
    public void         SetValueTrace(int[] valueTraces)            { this.valueTraces = valueTraces; }
    public int[]        GetIndexTrace()                             { return indexTraces; }
    public int[]        GetValueTrace()                             { return valueTraces; }
    public int          GetRecursionDepth()                         { return recursionDepth; }
    public AbstractNode GetChild(int valueKey)                      { return children[valueKey]; }
}
