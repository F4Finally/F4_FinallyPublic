using System.Collections.Generic;

public class GachaResult
{
    public List<GachaItem> Items { get; private set; }

    public GachaResult(List<GachaItem> items)
    {
        Items = items;
    }
}
