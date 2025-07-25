namespace Parsing.Vehicles.Grpc.Recognition;

public interface ICharacteristicRecognition
{
    Task<Characteristic> Recognize(string text);
}