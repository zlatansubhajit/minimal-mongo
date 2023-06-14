namespace minimal_api
{
    public record Company(ObjectId Id, string Name, IReadOnlyCollection<Office> Offices);
    public record Office(string Id, Address Address);
    public record Address(string Line1, string Line2, string PostalCode, string Country);
    public record Subscription(DateOnly SubStart, DateOnly SubEnd, double PaidAmount, double DueAmount, string PtTrainer);
    public record Member(ObjectId Id, string Name, long Phone, string Email,string Address, string GymName,Subscription ActiveSubscription);
    public record Employee(ObjectId Id, string Name, long Phone, string Email, string Address, string OfficeLocation, double salary, int leaves );
    public record Payment(ObjectId Id, DateTime dateTime, double Amount, string ReceivedBy, string ReceivedFrom, string PaymentMethod, string PaymentFor, string PtTrainer, string GymName);
}