
namespace DO
{
    public interface ICall1
    {
        string? Adress { get; init; }
        string? Description { get; init; }
        int Id { get; init; }
        double Latitude { get; init; }
        double Longitude { get; init; }
        DateTime MaxTimeToFinish { get; init; }
        DateTime TimeOfOpen { get; init; }
        TypeOfReading TypeOfReading { get; init; }

        void Deconstruct(out int Id, out TypeOfReading TypeOfReading, out string? Description, out string? Adress, out double Longitude, out double Latitude, out DateTime TimeOfOpen, out DateTime MaxTimeToFinish);
        bool Equals(Call? other);
        bool Equals(object? obj);
        int GetHashCode();
        string ToString();
    }
}