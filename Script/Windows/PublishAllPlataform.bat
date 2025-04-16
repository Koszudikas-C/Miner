@echo off
setlocal enabledelayedexpansion

set platforms=win-x64 linux-x64 osx-x64
set outputDir=publish

echo Compilando para as plataformas: %platforms%
echo ------------------------------------------

for %%p in (%platforms%) do (
    echo Compilando para: %%p
    dotnet publish -c Release -r %%p -p:PublishSingleFile=true --self-contained true -o %outputDir%\%%p
)

echo ------------------------------------------
echo Publicações concluídas em %outputDir%
pause
