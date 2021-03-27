for %%f in (*.png) do (
	convert "%%~nf.png" -filter Point -resize 3%% +antialias "Scaled/%%~nf.png"
)