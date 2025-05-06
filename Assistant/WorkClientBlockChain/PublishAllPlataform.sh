#!/bin/sh

platforms="linux-x64 win-x64 osx-x64"
outputDir="publish"

echo "Compilando para as plataformas: $platforms"
echo "------------------------------------------"

for platform in $platforms; do
    echo "Compilando para: $platform"
    dotnet publish -c Release -r "$platform" -p:PublishSingleFile=true --self-contained true
done

echo "------------------------------------------"
echo "Publicações concluídas!"
