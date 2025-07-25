using System.Text;
using RemTech.Logging.Library;
using RemTech.Result.Library;

namespace RemTech.Core.Shared.Functional;

public sealed class LoggingMaybeBag<T>(ICustomLogger logger, MaybeBag<T> maybeBag, string template)
{
    public MaybeBag<T> Logged()
    {
        StringBuilder sb = new(template);
        if (maybeBag.Any())
        {
            sb = sb.AppendFormat(" Найден: {0}.", true);
            logger.Info(sb.ToString());
        }
        else
        {
            sb = sb.AppendFormat(" Найден: {0}.", false);
            logger.Warn(sb.ToString());
        }

        return maybeBag;
    }
}
