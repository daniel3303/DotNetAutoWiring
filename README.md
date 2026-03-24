# Equibles.Core.AutoWiring

[![NuGet Version](https://img.shields.io/nuget/v/Equibles.Core.AutoWiring.svg)](https://www.nuget.org/packages/Equibles.Core.AutoWiring)
[![CI](https://github.com/daniel3303/DotNetAutoWiring/actions/workflows/ci.yml/badge.svg)](https://github.com/daniel3303/DotNetAutoWiring/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/daniel3303/DotNetAutoWiring/graph/badge.svg)](https://codecov.io/gh/daniel3303/DotNetAutoWiring)

A lightweight attribute-based service registration library for .NET. Automatically discovers and wires services into the `Microsoft.Extensions.DependencyInjection` container by scanning assemblies for classes decorated with `[Service]`, reducing boilerplate and improving maintainability.

## Requirements

- .NET 8.0, 9.0, or 10.0

## Installation

```bash
dotnet add package Equibles.Core.AutoWiring
```

## Usage

### 1. Decorate your services with `[Service]`

```csharp
using Equibles.Core.AutoWiring;
using Microsoft.Extensions.DependencyInjection;

// Registers as itself with Scoped lifetime (default)
[Service]
public class OrderProcessor { }

// Registers as INotificationService with Singleton lifetime
[Service(ServiceLifetime.Singleton, typeof(INotificationService))]
public class EmailNotificationService : INotificationService { }

// Registers without replacing existing registrations
[Service(ServiceLifetime.Transient, typeof(INotificationService), replaceExisting: false)]
public class SmsNotificationService : INotificationService { }
```

### 2. Wire services from an assembly

```csharp
using Equibles.Core.AutoWiring;

var builder = WebApplication.CreateBuilder(args);

// Scan the assembly containing Program for [Service] classes
builder.Services.AutoWireServicesFrom<Program>();

var app = builder.Build();
app.Run();
```

The `AutoWireServicesFrom<T>()` method scans the assembly containing `T` and registers all classes decorated with `[Service]`.

## `[Service]` Attribute

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `lifetime` | `ServiceLifetime` | `Scoped` | The DI lifetime (`Singleton`, `Scoped`, `Transient`) |
| `interfaceType` | `Type` | `null` | The service type to register as. If `null`, registers as the concrete class itself |
| `replaceExisting` | `bool` | `true` | If `true`, removes any existing registration for the same service type before adding |

## Multi-Assembly Registration

For projects with multiple assemblies, add a marker class to each assembly and call `AutoWireServicesFrom` for each:

```csharp
// In each assembly, add an empty marker class
public class MyAssemblyMarker { }

// In Program.cs
builder.Services.AutoWireServicesFrom<MyAssemblyMarker>();
builder.Services.AutoWireServicesFrom<AnotherAssemblyMarker>();
```

## License

MIT License - see [LICENSE](LICENSE) for details.

## Author

[Daniel Oliveira](https://github.com/daniel3303) / [Equibles](https://equibles.com)
