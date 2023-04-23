using Asp.Versioning;

namespace BlazorApp1.Server;

public static class ApiVersions
{
    private static ApiVersion[]? _versions;

    public static readonly ApiVersion V1 = new ApiVersion(1, 0);

    public static readonly ApiVersion V2 = new ApiVersion(2, 0);

    public static IEnumerable<ApiVersion> Versions => _versions ?? (_versions = new ApiVersion[] {
            V1,
            V2
        });
}