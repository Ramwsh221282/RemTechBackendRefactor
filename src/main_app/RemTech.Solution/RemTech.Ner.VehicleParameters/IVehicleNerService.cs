namespace RemTech.Ner.VehicleParameters;

public interface IVehicleNerService
{
    IReadOnlyList<VehicleNerOutput> DetectParameters(string input);
}