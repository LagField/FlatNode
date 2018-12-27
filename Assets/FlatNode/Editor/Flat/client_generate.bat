echo off
cd /d D:\UnityProjects\FlatNode\FlatNode\Assets\FlatNode\Editor\Flat\

set flatdirectory=D:\UnityProjects\FlatNode\FlatNode\Assets\FlatNode\Editor\Flat\flat\
set csharpoutputdirectory=D:\UnityProjects\FlatNode\FlatNode\Assets\FlatNode\Runtime\FlatGenerate\

flatc -n -o %csharpoutputdirectory%^
 %flatdirectory%FlatNodeConfig.fbs^
 --gen-onefile

echo "Done!"

@pause