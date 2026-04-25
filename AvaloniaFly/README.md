# Avalonia FLY (beta)

Перший крок поступової міграції FLY UI на **Avalonia UI**.

## Що є зараз
- Окремий Avalonia-host застосунок (`AvaloniaFly.csproj`) з базовим FLY layout (HUD/Map placeholder).
- У legacy FLY вкладці додано чекбокс `Avalonia FLY (beta)`, який запускає host (`AvaloniaFly.exe` або `dotnet AvaloniaFly.dll`).

## Як зібрати
```bash
dotnet build AvaloniaFly/AvaloniaFly.csproj
```

> Проєкт налаштований без `ImplicitUsings` і без file-scoped namespace у коді, щоб не ламати сумісність зі старішими toolchain/аналізаторами в основному репозиторії.

Після білду покладіть `AvaloniaFly.exe`/`AvaloniaFly.dll` в папку `AvaloniaFly` поруч з основним MissionPlanner executable.
