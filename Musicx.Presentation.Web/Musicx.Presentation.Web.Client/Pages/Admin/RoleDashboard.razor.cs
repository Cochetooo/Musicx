using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudBlazor;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Pages.Admin;

public partial class RoleDashboard
{
    private ILogger _logger = null!;
    
    private List<OutRole> _roles = [];
    private OutRole? _selectedRole;
    
    private List<BreadcrumbItem> _breadcrumb =
    [
        new("Musicx", href: "/"),
        new("Admin", href: "#"),
        new("Role Management", href: "#")
    ];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerFactory.CreateLogger(nameof(RoleDashboard));

        await LoadData();
    }

    private async Task LoadData()
    {
        _logger.LogInformation("🔄️ RoleDashboard : UPDATE Data");
        
        _roles = await UcList.ExecuteAsync(query: "permission");

        await InvokeAsync(StateHasChanged);
    }
}