using System.Collections.Generic;

interface IProperty
{
    void AddAttribute(AbstractAttribute newAttribute);
    List<AbstractAttribute> GetAllAttributes();
}