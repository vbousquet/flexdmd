@echo off
echo This script prepare assets for use in VPX
echo It requires ImageMagick with legacy tools to be installed beforehand

cd "./Adventure Girl/png"
for %%f in (*.png) do (
	echo %%~nf
	convert "%%~nf.png" -filter Point -resize 3%% +antialias "Scaled/%%~nf.png"
)
montage "Run (1).png" "Run (2).png" "Run (3).png" "Run (4).png" "Run (5).png" "Run (6).png" "Run (7).png" "Run (8).png" "Jump (1).png" "Jump (2).png" "Jump (3).png" "Jump (4).png" "Jump (5).png" "Jump (6).png" "Jump (7).png" "Jump (8).png" "Jump (5).png" "Jump (10).png" "Dead (1).png" "Dead (2).png" "Dead (3).png" "Dead (4).png" "Dead (5).png" "Dead (6).png" "Dead (7).png" "Dead (8).png" "Dead (5).png" "Dead (10).png" "Idle (1).png" "Idle (2).png" "Idle (3).png" "Idle (4).png" "Idle (5).png" "Idle (6).png" "Idle (7).png" "Idle (8).png" "Idle (5).png" "Idle (10).png" -tile 8x5 -geometry 19x19+0+0 -background transparent ../../girl.png
cd ../..

cd ./GraveyardTileset/png/Tiles
for %%f in (*.png) do (
	echo %%~nf
	convert "%%~nf.png" -filter Point -resize 6%% +antialias "Scaled/%%~nf.png"
)
cd ../../..

cd ./GraveyardTileset/png/Objects
for %%f in (*.png) do (
	echo %%~nf
	convert "%%~nf.png" -filter Point -resize 10%% +antialias "Scaled/%%~nf.png"
)
cd ../../..

