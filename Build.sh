# Github workflows thing
# the size of this program is gonna be huge xd
dotnet publish --output build\ --arch x64 --os linux -c release --self-contained true # linux build
dotnet publish --output build\ --arch x86 --os win -c release --self-contained true # win build
