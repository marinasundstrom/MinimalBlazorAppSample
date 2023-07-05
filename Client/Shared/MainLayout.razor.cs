using BlazorApp1.Client.Services;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace BlazorApp1.Client.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject] protected LayoutService LayoutService { get; set; } = default!;

    private MudThemeProvider _mudThemeProvider = default!;

    private bool _disposedValue;

    protected override void OnInitialized()
    {
        LayoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await ApplyUserPreferences();
            await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
            StateHasChanged();
        }
    }

    private async Task ApplyUserPreferences()
    {
        var defaultDarkMode = await _mudThemeProvider.GetSystemPreference();
        await LayoutService.ApplyUserPreferences(defaultDarkMode);
    }

    private async Task OnSystemPreferenceChanged(bool newValue)
    {
        await LayoutService.OnSystemPreferenceChanged(newValue);
    }

    private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _mudThemeProvider.Dispose();
                LayoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}