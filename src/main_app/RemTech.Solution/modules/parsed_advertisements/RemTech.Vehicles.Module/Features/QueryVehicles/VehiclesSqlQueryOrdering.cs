using System.Text;

namespace RemTech.Vehicles.Module.Features.QueryVehicles;

internal sealed class VehiclesSqlQueryOrdering
{
    private string _orderingString = string.Empty;
    private string _orderingMode = string.Empty;
    private bool _vectorSearchAccepted;

    public void WithOrdering(string orderingField, string mode)
    {
        _orderingString = orderingField;
        _orderingMode = mode;
    }

    public void WithVectorSearch()
    {
        _vectorSearchAccepted = true;
    }

    public StringBuilder Apply(StringBuilder sb)
    {
        if (_vectorSearchAccepted && !string.IsNullOrEmpty(_orderingString))
        {
            string sql = $" ORDER BY {_orderingString} {_orderingMode}, v.embedding <=> @embedding";
            return sb.AppendLine(sql);
        }
        if (!string.IsNullOrEmpty(_orderingString))
        {
            string sql = $" ORDER BY {_orderingString} {_orderingMode} ";
            return sb.AppendLine(sql);
        }
        if (_vectorSearchAccepted)
        {
            string sql = $" ORDER BY v.embedding <=> @embedding ";
            return sb.AppendLine(sql);
        }
        return sb;
    }
}
