for %%f in (video_original\frame_*.ppm) do (
    magick "%%f" -compress none "video_original/p3_%%~nf.ppm"
    del "%%f"
    )
echo Conversion to PPM3 completed.
pause