using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Core.Common;

public static class MessageBodyValidation
{
    extension(MessageBody body)
    {
        public void Validate()
        {
            string value = body.Value;
            const string valueName = "Тело письма";
            if (string.IsNullOrWhiteSpace(value)) throw ErrorException.ValueNotSet(valueName);
        }
    }
}