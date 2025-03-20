# Location Validation Rules

## Country Validation
```csharp
public class CountryValidator : AbstractValidator<Country>
{
    public CountryValidator()
    {
        RuleFor(x => x.CountryName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z\s-]+$")
            .WithMessage("Country name can only contain letters, spaces, and hyphens");

        RuleFor(x => x.Regions)
            .Must(regions => regions == null || regions.Select(r => r.RegionName.ToLower()).Distinct().Count() == regions.Count)
            .WithMessage("Region names must be unique within a country");
    }
}
```

## Region Validation
```csharp
public class RegionValidator : AbstractValidator<Region>
{
    public RegionValidator()
    {
        RuleFor(x => x.RegionName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z\s-]+$")
            .WithMessage("Region name can only contain letters, spaces, and hyphens");

        RuleFor(x => x.CountryID)
            .NotEmpty();

        RuleFor(x => x.Localities)
            .Must(localities => localities == null || 
                localities.Select(l => l.LocalityName.ToLower()).Distinct().Count() == localities.Count)
            .WithMessage("Locality names must be unique within a region");
    }
}
```

## Locality Validation
```csharp
public class LocalityValidator : AbstractValidator<Locality>
{
    public LocalityValidator()
    {
        RuleFor(x => x.LocalityName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z\s-]+$")
            .WithMessage("Locality name can only contain letters, spaces, and hyphens");

        RuleFor(x => x.RegionID)
            .NotEmpty();
    }
}