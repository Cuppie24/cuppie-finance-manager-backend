namespace Application.DateTimeProvider;

public interface IDateTimeProvider
{
    DateTime Now();
    DateTime UtcNow();
}