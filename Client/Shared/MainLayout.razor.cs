using BlazorApp1.Client.Services;
using BlazorApp1.Client.Themes;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace BlazorApp1.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [Inject] protected LayoutService LayoutService { get; set; } = default!;

        private MudThemeProvider _mudThemeProvider = default!;

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

        public void Dispose()
        {
            LayoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        }

        private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
    }
}