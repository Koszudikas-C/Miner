#!/bin/bash
set -e

platforms=("win-x64" "linux-x64" "osx-x64")
outputDir="publish"

echo "Compilando para as plataformas: ${platforms[*]}"
echo "------------------------------------------"

for platform in "${platforms[@]}"; do
    echo "Compilando para: $platform"
    dotnet publish -c Release -r "$platform" -p:PublishSingleFile=true --self-contained true -o "$outputDir/$platform"
done

echo "------------------------------------------"
echo "Publicações concluídas em $outputDir"
