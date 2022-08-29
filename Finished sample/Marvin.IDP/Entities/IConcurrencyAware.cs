namespace Marvin.IDP.Entities
{
    public interface IConcurrencyAware
    {
        string ConcurrencyStamp { get; set; }
    }
}
