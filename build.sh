#/bin/bash
mkdir publish

cd example
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/example ../publish/
cd ..

cd example2
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/example2 ../publish/
cd ..

cd integration
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/integration ../publish/
cd ..

cd tracker
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/tracker ../publish/
cd ..
