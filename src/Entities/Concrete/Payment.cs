using Core.Entities;

namespace Entities.Concrete;

public class Payment : IEntity
{
    public string CardNumber { get; set; }
    public string HolderName { get; set; }
    public string MounthOfExp { get; set; }
    public string YearOfExp { get; set; }
    public string CVC { get; set; }
    public int Amount { get; set; }
}
