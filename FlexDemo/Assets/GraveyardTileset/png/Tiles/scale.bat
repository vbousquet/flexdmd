for %%f in (*.png) do (
	convert "%%~nf.png" -filter Point -resize 6%% +antialias "Scaled/%%~nf.png"
)