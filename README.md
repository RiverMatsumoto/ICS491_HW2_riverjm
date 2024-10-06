# ICS491_HW2_RIVERJM

#### Notes for working with ppm format
Can use ffmpeg and imagemagick to convert between .mp4 video and .ppm P3 format

.mp4 -> .ppm
`ffmpeg -i video.mp4 -t 5 -r 24 frame_%04d.ppm`
you can change the output format as needed
then you need to convert the ppm into p3, cd into the images/video directory and run
`ffmpeg -framerate 24 -i video_final/frame_%04d.ppm -c:v libx264 -pix_fmt yuv420p output_p3.mp4`


