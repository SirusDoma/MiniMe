# MiniMe
A simple self-hosted AllNet/Aime server, written in .NET Core 3.1

## Features
| Name | Server | Protocol | Database |
| --- | --- | --- | -- |
| ALLNet | ASP.NET Core, Kestrel | Http | |
| Aime | ASP.NET Core, Kestrel | Http | [EF Core](https://github.com/dotnet/efcore) (SQLite) |
| Billing | ASP.NET Core, Kestrel | Https | |
| Chunithm | System.Net.Socket | TCP | [EF Core](https://github.com/dotnet/efcore) (SQLite) |

## How to run

### Requirements
- .NET Core SDK

### Setup repository
```sh
git clone https://github.com/evan-choi/MiniMe.git
cd MiniMe
```

### Run
```sh
dotnet run
```

## How to change DB provider
1. Setup [AimeContext](https://github.com/evan-choi/MiniMe/blob/642bbbe322d107221cd4ff0bc9383de023eeb125/MiniMe.Aime/Data/AimeContext.cs#L18) Configuring

2. Setup [ChunithmContext](https://github.com/evan-choi/MiniMe/blob/642bbbe322d107221cd4ff0bc9383de023eeb125/MiniMe.Chunithm/Data/ChunithmContext.cs#L18) Configuring 

```cs
protected override void OnConfiguring(DbContextOptionsBuilder options)
{
    // Setup to the db provider you want
    options.UseMySql(/* mysql connection string */);
}
```
