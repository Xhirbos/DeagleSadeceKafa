using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using System.Runtime.InteropServices;

namespace DeagleSadeceKafa;

[MinimumApiVersion(100)]
public class DeagleHeadshot : BasePlugin
{
    public override string ModuleName => "Deagle Sadece Kafa";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Xhirbos";
    public override string ModuleDescription => "Deagle silahı ile sadece kafadan vuruş yapılmasını sağlar ve tek atar.";

    public override void Load(bool hotReload)
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage, HookMode.Pre);
    }

    public override void Unload(bool hotReload)
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage, HookMode.Pre);
    }

    private HookResult OnTakeDamage(DynamicHook hook)
    {
        CTakeDamageInfo info = hook.GetParam<CTakeDamageInfo>(1);
        
        if (info.Ability.Value?.DesignerName.Contains("weapon_deagle") is true)
        {
            if (GetHitGroup(hook) == HitGroup_t.HITGROUP_HEAD)
            {
                // Kafa vuruşunda anında öldür
                info.Damage = 1000;
                return HookResult.Changed;
            }
            else
            {
                // Kafa harici vuruşlarda hasarı engelle
                hook.SetReturn(false);
                return HookResult.Handled;
            }
        }

        return HookResult.Continue;
    }

    private static unsafe HitGroup_t GetHitGroup(DynamicHook hook)
    {
        nint info = hook.GetParam<nint>(1);
        nint v4 = *(nint*)(info + 0x78);

        if (v4 == nint.Zero)
        {
            return HitGroup_t.HITGROUP_INVALID;
        }

        nint v1 = *(nint*)(v4 + 16);

        HitGroup_t hitgroup = HitGroup_t.HITGROUP_GENERIC;

        if (v1 != nint.Zero)
        {
            hitgroup = (HitGroup_t)(*(uint*)(v1 + 56));
        }

        return hitgroup;
    }
} 