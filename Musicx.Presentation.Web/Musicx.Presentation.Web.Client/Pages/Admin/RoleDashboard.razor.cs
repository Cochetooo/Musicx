using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Security;

namespace Musicx.Presentation.Web.Client.Pages.Admin;

public partial class RoleDashboard
{
    private ILogger _logger = null!;

    private List<OutPermission> _permissions = [];
    private List<OutRole> _roles = [];
    private OutRole? _selectedRole;

    private bool _addMode;
    private bool _isModified;
    
    private readonly List<BreadcrumbItem> _breadcrumb =
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
        
        _roles = 
            (await Api.FindAsync<InRole, OutRole>(joins: new RoleJoinSpecification
            {
                IncludePermissions = true
            }, pagingOptions: new PagingOptions(Take: 10_000, Skip: 0)))
            .Items;
        
        _permissions = 
            (await Api.FindAsync<InPermission, OutPermission>(pagingOptions: new PagingOptions(Take: 10_000, Skip: 0)))
            .Items;

        await InvokeAsync(StateHasChanged);
    }

    private async Task Save()
    {
        if (_selectedRole is null)
        {
            _logger.LogWarning("⚠️ RoleDashboard : Cannot save role when selected role is null");
            Snackbar.Add("Cannot save role when selected role is null", Severity.Warning);
            return;
        }
        
        await Api.SaveAsync(_selectedRole.ToRaw());
        
        _isModified = false;
        await InvokeAsync(StateHasChanged);
    }

    private void AddPermission(OutPermission perm)
    {
        if (_selectedRole is null)
        {
            _logger.LogWarning("⚠️ RoleDashboard : Cannot add permission when selected role is null");
            Snackbar.Add("Cannot add permission when selected role is null", Severity.Warning);
            return;
        }

        _isModified = true;

        _selectedRole.Permissions ??= new List<OutPermission>();
        _selectedRole.Permissions.Add(perm);
    }

    private void RemovePermission(OutPermission perm)
    {
        if (_selectedRole is null)
        {
            _logger.LogWarning("⚠️ RoleDashboard : Cannot remove permission when selected role is null");
            Snackbar.Add("Cannot remove permission when selected role is null", Severity.Warning);
            return;
        }

        _isModified = true;
        _selectedRole.Permissions!.Remove(perm);
        StateHasChanged();
    }
}