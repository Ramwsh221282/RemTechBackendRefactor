using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Mailing.Core.Common;

public static class MessageSubjectValidation
{
    private const int MaxLength = 128;

    extension(MessageSubject subject)
    {
        public void Validate()
        {
            string value = subject.Value;
            const string valueName = "Тема письма";
            if (string.IsNullOrEmpty(value)) throw ErrorException.ValueNotSet(valueName);
            if (value.Length > MaxLength) throw ErrorException.ValueExcess(valueName, MaxLength);
        }
    }
}