# Github workflows thing
# the size of this program is gonna be huge xd
dotnet restore # get thy dependencies
dotnet publish --output build\ --framework net6.0 --arch x64 --os linux -c release --self-contained true # linux build
dotnet publish --output build\ --framework net6.0 --arch x86 --os win -c release --self-contained true # win build
