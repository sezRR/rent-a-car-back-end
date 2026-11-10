using Rentar.Shared.Kernel.Entities;

namespace Rentar.Modules.Payments.Domain;

/// <summary>
/// Card details of a payment. Never stored: the module keeps no database of its own.
/// </summary>
public class Payment : IEntity
{
    public string CardNumber { get; set; }
    public string HolderName { get; set; }
    public string MounthOfExp { get; set; }
    public string YearOfExp { get; set; }
    public string CVC { get; set; }
    public int Amount { get; set; }
}
