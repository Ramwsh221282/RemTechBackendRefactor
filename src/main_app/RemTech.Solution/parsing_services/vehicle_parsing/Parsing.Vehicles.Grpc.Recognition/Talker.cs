using Google.Protobuf.Collections;
using Recognizer;

namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class Talker
{
    private readonly Recognizer.Recognizer.RecognizerClient _client;

    public Talker(Recognizer.Recognizer.RecognizerClient client)
    {
        _client = client;
    }

    public async Task<RecognizeResponse> Tell(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text));
        RecognizeRequest request = new() { Text = text };
        return await _client.RecognizeAsync(request);
    }
}