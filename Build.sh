# Github workflows thing
# the size of this program is gonna be huge xd
dotnet publish "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/Meme Downloader 2016.csproj" --output "build\\" --arch x64 --os win -c release --self-contained true # win build

dotnet publish "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/Meme Downloader 2016.csproj" --output "build-debug\\" --arch x64 --os win -c debug --self-contained true # win build


ls "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/build"
