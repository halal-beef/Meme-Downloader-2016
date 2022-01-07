# Github workflows thing
# the size of this program is gonna be huge xd
dotnet publish "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/Meme Downloader 2016.csproj" --output "build\\" --arch x64 --os win -c release --self-contained true # win build

dotnet publish "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/Meme Downloader 2016.csproj" --output "build-debug\\" --arch x64 --os win -c debug --self-contained true # win build

# Move debug build
mv "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/build-debug/Meme Dwonloader 2016.exe" "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/build/[DEBUG] Meme Downloader 2016.exe"

ls "/home/runner/work/Meme-Downloader-2016/Meme-Downloader-2016/build"
