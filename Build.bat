:: Github workflows thing
:: the size of this program is gonna be huge xd

echo 'Making Build Directories!'
mkdir build\
mkdir build-debug\

echo 'Building Meme Downloader 2016 x64 debug-release'
dotnet publish "Meme Downloader 2016.csproj" --output "build\\" --arch x64 --os win -c release --self-contained true

dotnet publish "Meme Downloader 2016.csproj" --output "build-debug\\" --arch x64 --os win -c debug --self-contained true

:: Move debug build
move "\build-debug\Meme Downloader 2016.exe" "\build\DEBUG-Meme Downloader 2016.exe"

echo 'Executing Meme Downloader 2016 in CI mode'
".\build\Meme Downloader 2016.exe" -ci

echo 'Compiling Zipper.exe and making all downloaded content onto a zip!'

git clone https://github.com/usrDottik/Zipper.git

cd Zipper\

mkdir Zipper\build\

dotnet publish "Zipper.csproj" --output build\ --arch x64 --os win -c release --self-contained true

mv build\Zipper.exe ..\build\

cd ..\build

Zipper.exe -Output=Downloaded-Content.zip -LocalFile -Folder="\Downloaded Content\" 
