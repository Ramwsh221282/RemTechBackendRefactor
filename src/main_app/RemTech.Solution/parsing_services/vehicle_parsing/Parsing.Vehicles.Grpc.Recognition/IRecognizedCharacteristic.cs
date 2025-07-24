using Parsing.Vehicles.Grpc.Recognition.Measurements;

namespace Parsing.Vehicles.Grpc.Recognition;

public interface IRecognizedCharacteristic
{
    string ReadName();
    string ReadValue();
    IMeasurement Measurement();
}