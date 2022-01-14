# Github workflows thing
# the size of this program is gonna be huge xd
dotnet publish "Meme Downloader 2016.csproj" --output build\ --arch x64 --os win -c release --self-contained true # win build

dotnet publish "Meme Downloader 2016.csproj" --output build-debug\ --arch x64 --os win -c debug --self-contained true # win build

# Move debug build
mv "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/build-debug/Meme Downloader 2016.exe" "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/build/DEBUG-Meme Downloader 2016.exe"

dotnet publish "Meme Downloader 2016.csproj" --output Linux-Test\ -r linux-x64

cd Linux-Test\

chmod +x Meme\ Downloader\ 2016

./Meme\ Downloader\ 2016