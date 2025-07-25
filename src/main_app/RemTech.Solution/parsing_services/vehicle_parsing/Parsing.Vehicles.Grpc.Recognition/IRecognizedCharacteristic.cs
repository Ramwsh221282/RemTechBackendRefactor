using Parsing.Vehicles.Grpc.Recognition;

namespace Parsing.Vehicles.Grpc.Recognition;

public interface IRecognizedCharacteristic
{
    string ReadName();
    string ReadValue();
}