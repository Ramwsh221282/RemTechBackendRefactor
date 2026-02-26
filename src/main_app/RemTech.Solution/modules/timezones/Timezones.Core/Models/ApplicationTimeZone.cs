namespace Timezones.Core.Models;

public sealed class ApplicationTimeZone
{    
    public string Name { get; }    

    public ApplicationTimeZone(string name)
    {        
        Name = name;
    }
}


