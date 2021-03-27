for %%f in (*.png) do (
	convert "%%~nf.png" -filter Point -resize 10%% +antialias "Scaled/%%~nf.png"
)