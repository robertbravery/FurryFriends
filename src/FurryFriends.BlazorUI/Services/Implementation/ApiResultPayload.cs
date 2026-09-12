using System.Text.Json;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Deserializes API payloads that the server's <c>BaseEndpoint</c>-derived endpoints return
/// wrapped in an Ardalis <c>Result&lt;T&gt;</c> envelope
/// (<c>{ "value": { ... }, "status": ..., "isSuccess": true }</c>).
/// <para>
/// Endpoints that do not derive from <c>BaseEndpoint</c> return the payload directly, so both
/// shapes are accepted. This matters because deserializing a flat DTO straight against an
/// enveloped response does not fail: every property binds to its default, so the UI silently
/// shows a zero GUID and 0001-01-01 timestamps instead of the created record.
/// </para>
/// </summary>
internal static class ApiResultPayload
{
    /// <summary>
    /// Reads <typeparamref name="T"/> from <paramref name="json"/>, unwrapping the
    /// <c>Result&lt;T&gt;</c> envelope first when one is present.
    /// </summary>
    public static T? Deserialize<T>(string json, JsonSerializerOptions options) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Object && TryGetEnvelopedValue(root, out var value))
        {
            return value.ValueKind == JsonValueKind.Null
                ? null
                : value.Deserialize<T>(options);
        }

        return root.Deserialize<T>(options);
    }

    /// <summary>
    /// True when the object looks like a <c>Result&lt;T&gt;</c> envelope rather than a bare
    /// payload. Property names are matched case-insensitively because the server's naming
    /// policy is not guaranteed here. A bare payload is only mistaken for an envelope if it
    /// carries both a "value" member and a "status"/"isSuccess" member.
    /// </summary>
    private static bool TryGetEnvelopedValue(JsonElement element, out JsonElement value)
    {
        value = default;

        var hasValue = false;
        var hasEnvelopeMarker = false;

        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, "value", StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                hasValue = true;
            }
            else if (string.Equals(property.Name, "status", StringComparison.OrdinalIgnoreCase)
                  || string.Equals(property.Name, "isSuccess", StringComparison.OrdinalIgnoreCase))
            {
                hasEnvelopeMarker = true;
            }
        }

        return hasValue && hasEnvelopeMarker;
    }
}
