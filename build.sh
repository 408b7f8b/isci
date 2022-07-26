#/bin/bash
mkdir publish

cd integration
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/integration ../publish/
cd ..

cd msb
dotnet publish --self-contained true -r linux-arm -c Release
cp bin/Release/netcoreapp3.1/linux-arm/publish/msb ../publish/
cd ..

cd link
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/link ../publish/
cd ..

cd revpiea
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/revpiea ../publish/
cd ..

cd abbild
dotnet publish --self-contained true -r linux-arm -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true
cp bin/Release/netcoreapp3.1/linux-arm/publish/abbild ../publish/
cd ..
