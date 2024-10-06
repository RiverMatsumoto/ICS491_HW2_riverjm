# ICS491_HW2_RIVERJM

#### Notes for working with ppm format
Can use ffmpeg and imagemagick to convert between .mp4 video and .ppm P3 format

.mp4 -> .ppm
`ffmpeg -i <video>.mp4 -t 5 -r 24 frame_%04d.ppm`
you can change the output format as needed
then you need to convert the ppm into p3
run `<workspace>/images/video/convert_ppm_to_p3.bat` to convert to p3

### Memory Optimizations
The data structures used for the images matter a lot since images take up a lot of memory


