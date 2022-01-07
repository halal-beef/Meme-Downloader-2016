:: Github workflows thing
:: the size of this program is gonna be huge xd

echo 'Making Build Directories!'
mkdir build\
mkdir build-debug\

echo 'Building Meme Downloader 2016 x64 debug-release'
dotnet publish "Meme Downloader 2016.csproj" --output "build\\" --arch x64 --os win -c release --self-contained true # win build

dotnet publish "Meme Downloader 2016.csproj" --output "build-debug\\" --arch x64 --os win -c debug --self-contained true # win build

:: Move debug build
move \build-debug\Meme Downloader 2016.exe \build\DEBUG-Meme Downloader 2016.exe

:: Test build in CI mode
cd build\

echo 'Executing Meme Downloader 2016 in CI mode'
"Meme Downloader 2016.exe -ci"
