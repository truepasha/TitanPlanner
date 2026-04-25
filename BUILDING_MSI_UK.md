# Як зібрати MSI для Windows 7–11 у вашому форку

Цей репозиторій уже має WiX-пакування для `TitanPlanner.msi`.
Найпростіший і найстабільніший шлях для форку — збірка через GitHub Actions на Windows runner.

## Варіант 1 (рекомендовано): збірка в GitHub Actions

1. Запуште ваші зміни у гілку форку.
2. Відкрийте **Actions** → **Build MSI (Fork)**.
3. Натисніть **Run workflow** та задайте параметри:
   - `branch`: ваша гілка (наприклад `feature/my-fix`)
   - `configuration`: `Release`
   - `upload_release`: `false` (щоб лише отримати артефакти) або `true` (щоб одразу створити Release)
   - якщо `upload_release=true`, заповніть `release_tag` і `release_name`
4. Після завершення джоби скачайте артефакти:
   - `TitanPlanner-MSI-Release` (MSI)
   - `TitanPlanner-ZIP-Release` (portable zip)
   - `TitanPlanner-checksums-Release`

## Варіант 2: локальна збірка у Visual Studio (Windows)

Потрібно:
- Visual Studio 2022 з MSBuild
- WiX Toolset 3.x (щоб існувала змінна середовища `WIX`)

Кроки:
1. Зібрати `MissionPlanner.sln` у `Release`.
2. Перейти в теку `Msi`.
3. Згенерувати `installer.wxs`:
   ```powershell
   .\net472\wix.exe ..\bin\Release\net461
   ```
4. Зібрати WiX-об'єкт:
   ```powershell
   "$env:WIX\bin\candle.exe" installer.wxs -ext WiXNetFxExtension -ext WixDifxAppExtension -ext WixUIExtension.dll -ext WixUtilExtension -ext WixIisExtension
   ```
5. Лінкувати MSI:
   ```powershell
   "$env:WIX\bin\light.exe" installer.wixobj "$env:WIX\bin\difxapp_x86.wixlib" -sval -o TitanPlanner.msi -ext WiXNetFxExtension -ext WixDifxAppExtension -ext WixUIExtension.dll -ext WixUtilExtension -ext WixIisExtension
   ```

## Сумісність Windows 7–11

- Цільова платформа збірки тут: `.NET Framework 4.6.1` (`net461`).
- Це дозволяє запуск на Windows 7 SP1 (за наявності необхідних оновлень/рантайму) і новіших Windows 8.1/10/11.
- Для реальних користувачів Windows 7 краще окремо протестувати «чисту» інсталяцію MSI на VM.

## Публікація «закинути на гіт»

Є два варіанти:
- **Artifacts only**: просто скачати MSI з Actions.
- **GitHub Release**: у workflow ввімкнути `upload_release=true`, вказати tag/name, і файл прикріпиться до Release автоматично.
