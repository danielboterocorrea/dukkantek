using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace dukkantek.Api.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class FromRouteAndBodyAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource? BindingSource => new FromRouteAndBodyBindingSource();
}

public class FromRouteAndBodyBindingSource : BindingSource
{
    public FromRouteAndBodyBindingSource()
        : base("FromModelBinding", "FromModelBinding", true, true)
    {
    }

    public override bool CanAcceptDataFrom(BindingSource bindingSource) => bindingSource == ModelBinding;
}