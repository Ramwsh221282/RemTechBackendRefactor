namespace Mailers.Communication.State;

public interface IMailerStatisticsStateSource
{
    void Accept(int sendLimit, int currentAmount);
}