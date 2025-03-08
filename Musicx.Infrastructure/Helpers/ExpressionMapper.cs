using System.Linq.Expressions;

namespace Musicx.Infrastructure.Helpers;

public class ExpressionMapper<TFrom, TTo> : ExpressionVisitor
{
    private readonly ParameterExpression _parameter;

    public ExpressionMapper(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parameter; // Remplace l'ancien paramètre par le nouveau
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
        {
            var newProperty = typeof(TTo).GetProperty(node.Member.Name);
            if (newProperty == null)
                throw new InvalidOperationException($"Property {node.Member.Name} not found in {typeof(TTo).Name}");

            return Expression.Property(_parameter, newProperty);
        }
        return base.VisitMember(node);
    }

    public static Expression<Func<TTo, bool>>? Convert(Expression<Func<TFrom, bool>>? expression)
    {
        if (expression == null) return null;

        var parameter = Expression.Parameter(typeof(TTo), expression.Parameters[0].Name);
        var visitor = new ExpressionMapper<TFrom, TTo>(parameter);
        var newBody = visitor.Visit(expression.Body);

        return Expression.Lambda<Func<TTo, bool>>(newBody!, parameter);
    }
}