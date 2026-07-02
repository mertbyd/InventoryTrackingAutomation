using System;
using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using InventoryTrackingAutomation.ExceptionCodes;
using Volo.Abp;

namespace InventoryTrackingAutomation.Search;

/// <summary>
/// Repository'ye gelen lambda predicate'i Elasticsearch query nesnesine cevirir.
/// </summary>
// islevi: ==, Equals, Contains, &&, ||, x => true ifadelerini term/match/bool sorgularina cevirir; yeni ifade gerektiginde switch'e case eklenir.
// sistemdeki gorevi: ES sorgu DSL'i yalnizca burada kurulur; cagiran taraf EF repository'deki gibi lambda yazar.
internal static class ElasticsearchPredicateTranslator
{
    public static Query Translate<TDocument>(Expression<Func<TDocument, bool>> predicate, string fuzziness)
    {
        return TranslateNode(predicate.Body, fuzziness);
    }

    private static Query TranslateNode(Expression node, string fuzziness)
    {
        return node switch
        {
            BinaryExpression { NodeType: ExpressionType.AndAlso } b => new BoolQuery
            {
                Must = new[] { TranslateNode(b.Left, fuzziness), TranslateNode(b.Right, fuzziness) }
            },
            BinaryExpression { NodeType: ExpressionType.OrElse } b => new BoolQuery
            {
                Should = new[] { TranslateNode(b.Left, fuzziness), TranslateNode(b.Right, fuzziness) },
                MinimumShouldMatch = 1
            },
            BinaryExpression { NodeType: ExpressionType.Equal } b => Term(FieldOf(b.Left), b.Right),
            MethodCallExpression { Method.Name: nameof(Equals), Object: { } field, Arguments: [var value] } => Term(FieldOf(field), value),
            MethodCallExpression { Method.Name: nameof(string.Contains), Object: { } field, Arguments: [var text] } => Match(FieldOf(field), text, fuzziness),
            ConstantExpression { Value: true } => new MatchAllQuery(),
            _ => throw Unsupported(node)
        };
    }

    // Birebir eslesme; string/Guid alanlar text+keyword indexlendiginden analiz edilmemis ".keyword" alt alani hedeflenir.
    private static Query Term(MemberExpression field, Expression valueExpression)
    {
        var value = Evaluate(valueExpression) ?? throw Unsupported(valueExpression);
        var type = Nullable.GetUnderlyingType(field.Type) ?? field.Type;
        var isKeywordField = type == typeof(string) || type == typeof(Guid);

        return new TermQuery
        {
            Field = FieldNameOf(field) + (isKeywordField ? ".keyword" : string.Empty),
            Value = value switch
            {
                bool boolean => FieldValue.Boolean(boolean),
                float or double or decimal => FieldValue.Double(Convert.ToDouble(value)),
                byte or short or int or long => FieldValue.Long(Convert.ToInt64(value)),
                _ => FieldValue.String(value.ToString()!)
            }
        };
    }

    // Fuzzy metin aramasi; analiz edilen alanin kendisi hedeflenir.
    private static Query Match(MemberExpression field, Expression textExpression, string fuzziness)
    {
        return new MatchQuery
        {
            Field = FieldNameOf(field),
            Query = (string?)Evaluate(textExpression) ?? string.Empty,
            Fuzziness = new Fuzziness(fuzziness)
        };
    }

    // Nullable karsilastirmalarinda derleyicinin ekledigi Convert katmanlari acilarak dokuman alanina ulasilir.
    private static MemberExpression FieldOf(Expression expression)
    {
        while (expression is UnaryExpression { NodeType: ExpressionType.Convert } convert)
        {
            expression = convert.Operand;
        }

        return expression is MemberExpression { Expression: ParameterExpression } field ? field : throw Unsupported(expression);
    }

    // Sabit veya closure degiskeni fark etmeksizin ifadenin calisma zamani degerini cikarir.
    private static object? Evaluate(Expression expression)
    {
        return Expression.Lambda<Func<object?>>(Expression.Convert(expression, typeof(object))).Compile()();
    }

    // Dokumanlar camelCase serialize edilir; alan adi ayni kuralla uretilir.
    private static string FieldNameOf(MemberExpression field)
    {
        return char.ToLowerInvariant(field.Member.Name[0]) + field.Member.Name[1..];
    }

    private static BusinessException Unsupported(Expression node)
    {
        return new BusinessException(GeneralExceptionCodes.SearchPredicateNotSupported);
    }
}
