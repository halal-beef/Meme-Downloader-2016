# Github workflows thing
# the size of this program is gonna be huge xd
ls /home/runner #gotta do this i need to reverse engineer my way to the path

dotnet publish --output build\ --arch x64 --os linux -c release --self-contained true # linux build
dotnet publish --output build\ --arch x86 --os win -c release --self-contained true # win build
