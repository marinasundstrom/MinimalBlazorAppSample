using Asp.Versioning;

namespace BlazorApp1.Server;

public static class ApiVersions
{
    private static ApiVersion[]? _versions;

    public static readonly ApiVersion V1 = new(1, 0);

    public static readonly ApiVersion V2 = new(2, 0);

    public static IEnumerable<ApiVersion> Versions => _versions ?? (_versions = new[] {
            V1,
            V2
        });
}