namespace Tests;

public sealed class ElapsedSecondsFromDateTimesDifferenceTest
{
    [Fact]
    private void Test_Elapsed_Seconds_From_Dates()
    {
        DateTime startDate = new(2023, 1, 1);
        DateTime endDate = new(2023, 1, 2);
        TimeSpan difference = endDate - startDate;
        Assert.Equal(86400, difference.TotalSeconds);
    }
}